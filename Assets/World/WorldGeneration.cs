using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [Header("Inventory Settings")]
    public InventoryClass playerInventory;

    [Header("Tile Settings")]
    public TileAtlas tileAtlas;

    // public OreContainer oreContainer;


    [Header("World Generation Settings")]
    public int worldSize = 100;
    public int safeRadius = 10;

    [Header("Noise Settings")]
    public float seed;
    public float woodFrequency = 0.05f;
    public Texture2D woodNoiseTexture;

    public float stoneFrequency = 0.05f;
    public Texture2D stoneNoiseTexture;
    public readonly List<Vector2> tiles = new();

    private Vector2 MapCenter => new Vector2(worldSize / 2, worldSize / 2);

    public delegate bool CanPlace(Vector2 position);
    public static CanPlace canPlace;

    public delegate bool OnTilePlaced(Vector2 position, Item item);
    public static OnTilePlaced onTilePlaced;

    public delegate bool OnTileRemoved(Vector2 position);
    public static OnTileRemoved onTileRemoved;

    // Start is called before the first frame update
    void Start()
    {
        onTilePlaced = OnPlaceTile;
        canPlace = CanPlaceOnTile;
        onTileRemoved = OnRemoveTile;
        seed = Random.Range(-10_000, 10_000);
        woodNoiseTexture = GenerateNoiseTexture(0.25f, woodFrequency);
        stoneNoiseTexture = GenerateNoiseTexture(0.5f, stoneFrequency);
        GenerateTerrain();
    }

    private Texture2D GenerateNoiseTexture(float limit, float noiseFrequency)
    {
        var noiseTexture = new Texture2D(worldSize, worldSize) { filterMode = FilterMode.Point };
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float noiseValue = Mathf.PerlinNoise(
                    (x + seed) * noiseFrequency,
                    (y + seed) * noiseFrequency
                );
                if (noiseValue > limit)
                    noiseTexture.SetPixel(x, y, Color.white);
                else
                    noiseTexture.SetPixel(x, y, Color.black);
            }
        }

        noiseTexture.Apply();
        return noiseTexture;
    }

    private void GenerateTerrain()
    {
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                var position = new Vector2(x, y);
                PlaceTile(tileAtlas.dirt, position, false);

                if (Vector2.Distance(position, MapCenter) < safeRadius)
                {
                    continue;
                }

                var woodValue = woodNoiseTexture.GetPixel(x, y).grayscale;
                if (woodValue < 0.5f)
                {
                    PlaceTile(tileAtlas.wood, position);
                }
                var stoneValue = stoneNoiseTexture.GetPixel(x, y).grayscale;
                if (stoneValue < 0.5f)
                {
                    PlaceTile(tileAtlas.stone, position);
                }
            }
        }
    }

    private bool OnPlaceTile(Vector2 position, Item item)
    {
        var tile = tileAtlas.GetTile(item);
        if (tile)
        {
            return PlaceTile(tile, position);
        }
        return false;
    }

    private bool OnRemoveTile(Vector2 position)
    {
        foreach (var tile in tiles)
        {
            if (tile.x == Mathf.Round(position.x) && tile.y == Mathf.Round(position.y))
            {
                tiles.Remove(tile);
                return true;
            }
        }
        return false;
    }

    private bool CanPlaceOnTile(Vector2 position)
    {
        foreach (var tile in tiles)
        {
            if (tile.x == Mathf.Round(position.x) && tile.y == Mathf.Round(position.y))
            {
                return false;
            }
        }
        return true;
    }

    private bool PlaceTile(TileClass tile, Vector2 position, bool isSolid = true)
    {
        if (!CanPlaceOnTile(position))
            return false;
        var newTile = new GameObject(tile.tileName);
        newTile.transform.parent = transform;
        newTile.transform.position = new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
        var spriteRenderer = newTile.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = tile.topTileSprite;
        if (isSolid)
        {
            var entityClass = newTile.AddComponent<TileEntityClass>();
            entityClass.playerInventory = playerInventory;
            entityClass.item = tile.item;
            newTile.AddComponent<BoxCollider2D>();
            newTile.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
            tiles.Add(newTile.transform.position);
        }
        else
        {
            newTile.layer = LayerMask.NameToLayer("Background");
            spriteRenderer.sortingLayerName = "Background";
            spriteRenderer.sortingOrder = -1;
        }
        return true;
    }
}
