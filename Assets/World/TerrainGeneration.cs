using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    [Header("Tile Settings")]
    public TileAtlas tileAtlas;

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
    public OreContainer oreContainer;

    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(-10_000, 10_000);
        if (generateCaves)
        {
            caveNoiseTexture = GenerateNoiseTexture(0.25f, caveFrequency);
        }
        foreach (var ore in oreContainer.ores)
        {
            ore.SetNoiseTexture(GenerateNoiseTexture(ore.rarity, ore.size));
        }
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

                bool orePlaced = false;
                foreach (var ore in oreContainer.ores)
                {
                    if (ore.CanPlace(worldSize, x, y))
                    {
                        PlaceTile(ore.tile, new Vector2(x, y));
                        orePlaced = true;
                        break;
                    }
                }
                if (!orePlaced)
                    PlaceTile(tileAtlas.stone, new Vector2(x, y));
            }
        }
    }

    private void PlaceTile(TileClass tile, Vector2 position)
    {
        if (tiles.Contains(position))
            return;
        var newTile = new GameObject(tile.tileName);
        newTile.transform.parent = this.transform;
        newTile.transform.position = position;
        newTile.AddComponent<SpriteRenderer>().sprite = tile.tileSprite;
        newTile.AddComponent<BoxCollider2D>();
        newTile.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
        newTile.tag = "Ground";

        tiles.Add(position);
    }
}