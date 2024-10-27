using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    [Header("Inventory Settings")]
    public StorageData playerInventory;

    [Header("Tile Settings")]
    public TileRegistry tileRegistry;

    // public OreContainer oreContainer;


    [Header("Terrain Generation Settings")]
    public int worldSize = 100;
    public float heightMultiplier = 2f;
    public float dirtHeight = 10f;
    public bool generateCaves = true;

    [Header("Noise Settings")]
    public float caveFrequency = 0.05f;
    public float terrainFrequency = 0.05f;
    public float seed;
    public Texture2D caveNoiseTexture;
    private readonly List<Vector2> tiles = new();

    [Header("Ore Generation Settings")]
    public OreRegistry oreRegistry;

    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(-10_000, 10_000);
        if (generateCaves)
        {
            caveNoiseTexture = GenerateNoiseTexture(0.25f, caveFrequency);
        }
        foreach (var ore in oreRegistry.list)
        {
            ore.SetNoiseTexture(GenerateNoiseTexture(ore.rarity, ore.size));
        }
        GenerateTerrain();
    }

    private bool isSpawnArea(Vector2 position)
    {
        return position.y >= 90 && position.y <= 92 && position.x >= 49 && position.x <= 51;
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
            var height =
                Mathf.PerlinNoise((x + seed) * terrainFrequency, seed * terrainFrequency)
                    * heightMultiplier
                + dirtHeight;
            for (int y = 0; y < worldSize; y++)
            {
                PlaceTile(tileRegistry.GetItem("mine_background"), new Vector2(x, y), false);
                if (isSpawnArea(new Vector2(x, y)))
                {
                    continue;
                }
                if (y > (worldSize - height))
                {
                    PlaceTile(tileRegistry.GetItem("dirt"), new Vector2(x, y));
                    continue;
                }

                if (generateCaves && caveNoiseTexture.GetPixel(x, y).r < 0.5f)
                    continue;



                bool orePlaced = false;
                foreach (var ore in oreRegistry.list)
                {
                    if (ore.CanPlace(worldSize, x, y))
                    {
                        PlaceTile(ore.tile, new Vector2(x, y));
                        orePlaced = true;
                        break;
                    }
                }
                if (!orePlaced)
                    PlaceTile(tileRegistry.GetItem("stone"), new Vector2(x, y));
            }
        }
    }

    private void PlaceTile(TileData tile, Vector2 position, bool isSolid = true)
    {
        var newTile = new GameObject(tile.GetName());
        newTile.transform.parent = transform;
        newTile.transform.position = position;
        var spriteRenderer = newTile.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = tile.tileSprite;
        if (isSolid)
        {
            var entityClass = newTile.AddComponent<TileEntityClass>();
            entityClass.playerInventory = playerInventory;
            entityClass.item = tile.item;
            newTile.AddComponent<BoxCollider2D>();
            newTile.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
            newTile.tag = "Solid";
            spriteRenderer.sortingLayerName = "Solid";
            tiles.Add(position);
        }
        else
        {
            if (tiles.Contains(position))
                return;
            newTile.tag = "Background";
            spriteRenderer.sortingLayerName = "Background";
        }
    }

}
