using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [Header("Tile Settings")]
    public TileAtlas tileAtlas;

    // public OreContainer oreContainer;


    [Header("World Generation Settings")]
    public int worldSize = 100;
    public int safeRadius = 10;

    [Header("Noise Settings")]
    public float woodFrequency = 0.05f;
    public float seed;
    public Texture2D woodNoiseTexture;
    private readonly List<Vector2> tiles = new();

    private Vector2 MapCenter => new Vector2(worldSize / 2, worldSize / 2);

    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(-10_000, 10_000);
        woodNoiseTexture = GenerateNoiseTexture(0.25f, woodFrequency);
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
            }
        }
    }

    private void PlaceTile(TileClass tile, Vector2 position, bool isSolid = true)
    {
        if (tiles.Contains(position))
            return;
        var newTile = new GameObject(tile.tileName);
        newTile.transform.parent = transform;
        newTile.transform.position = position;
        newTile.AddComponent<SpriteRenderer>().sprite = tile.topTileSprite;
        if (isSolid)
        {
            newTile.AddComponent<BoxCollider2D>();
            newTile.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
            tiles.Add(position);
        }
        else
        {
            newTile.layer = LayerMask.NameToLayer("Background");
        }
    }
}
