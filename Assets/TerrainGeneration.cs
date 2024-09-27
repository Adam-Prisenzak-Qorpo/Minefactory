using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;

public class TerrainGeneration : MonoBehaviour
{
    [Header("Tile Settings")]
    public TileAtlas tileAtlas;

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
    public float ironRarity;
    public float ironSize;
    public float ironDepth;
    public float goldRarity,
        goldSize,
        goldDepth;
    public float diamondRarity,
        diamondSize,
        diamondDepth;

    public Texture2D ironNoiseTexture;
    public Texture2D goldNoiseTexture;
    public Texture2D diamondNoiseTexture;

    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(-10_000, 10_000);
        if (generateCaves)
        {
            caveNoiseTexture = GenerateNoiseTexture(0.25f, caveFrequency);
        }
        ironNoiseTexture = GenerateNoiseTexture(ironSize, ironRarity);
        goldNoiseTexture = GenerateNoiseTexture(goldSize, goldRarity);
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
            var height =
                Mathf.PerlinNoise((x + seed) * terrainFrequency, seed * terrainFrequency)
                    * heightMultiplier
                + dirtHeight;
            for (int y = 0; y < worldSize; y++)
            {
                if (y > (worldSize - height))
                {
                    PlaceTile(tileAtlas.dirt, new Vector2(x, y));
                    continue;
                }

                if (generateCaves && caveNoiseTexture.GetPixel(x, y).r < 0.5f)
                    continue;

                if (y < (worldSize - ironDepth) && ironNoiseTexture.GetPixel(x, y).r > 0.5f)
                {
                    PlaceTile(tileAtlas.iron, new Vector2(x, y));
                }
                else if (y < (worldSize - goldDepth) && goldNoiseTexture.GetPixel(x, y).r > 0.5f)
                {
                    PlaceTile(tileAtlas.gold, new Vector2(x, y));
                }
                else
                {
                    PlaceTile(tileAtlas.stone, new Vector2(x, y));
                }
            }
        }
    }

    private void PlaceTile(TileClass tile, Vector2 position)
    {
        if (tiles.Contains(position))
            return;
        var newTile = new GameObject(tile.tileName);
        newTile.transform.parent = this.transform;
        newTile.transform.position = position + new Vector2(0.5f, 0.5f);
        newTile.AddComponent<SpriteRenderer>().sprite = tile.tileSprite;
        tiles.Add(position);
    }
}
