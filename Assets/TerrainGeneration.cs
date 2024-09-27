using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    [Header("Tile Settings")]
    public TileAtlas tileAtlas;

    [Header("Terrain Generation Settings")]
    public int worldSize = 100;
    public float heightMultiplier = 2f;
    public float heightAddition = 10f;

    [Header("Noise Settings")]
    public float caveFrequency = 0.05f;
    public float terrainFrequency = 0.05f;
    public float seed;
    public Texture2D caveNoiseTexture;
    private readonly List<Vector2> tiles = new();

    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(-10_000, 10_000);
        GenerateNoiseTexture(caveNoiseTexture, caveFrequency);
        GenerateTerrain();
    }

    private void GenerateNoiseTexture(Texture2D noiseTexture, float noiseFrequency)
    {
        noiseTexture = new Texture2D(worldSize, worldSize) { filterMode = FilterMode.Point };
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float noiseValue = Mathf.PerlinNoise(
                    (x + seed) * noiseFrequency,
                    (y + seed) * noiseFrequency
                );
                noiseTexture.SetPixel(x, y, new Color(noiseValue, noiseValue, noiseValue));
            }
        }

        noiseTexture.Apply();
    }

    private void GenerateTerrain()
    {
        for (int x = 0; x < worldSize; x++)
        {
            var height =
                Mathf.PerlinNoise((x + seed) * terrainFrequency, seed * terrainFrequency)
                    * heightMultiplier
                + heightAddition;
            for (int y = 0; y < worldSize; y++)
            {
                if (y > height)
                {
                    PlaceTile(tileAtlas.dirt, new Vector2(x, y));
                    continue;
                }

                float noiseValue = caveNoiseTexture.GetPixel(x, y).r;
                if (noiseValue > 0.2f)
                {
                    PlaceTile(tileAtlas.stone, new Vector2(x, y));
                }
            }
        }
    }

    private void PlaceTile(TileClass tile, Vector2 position)
    {
        var newTile = new GameObject(tile.tileName);
        newTile.transform.parent = this.transform;
        newTile.transform.position = position + new Vector2(0.5f, 0.5f);
        newTile.AddComponent<SpriteRenderer>().sprite = tile.tileSprite;
        tiles.Add(position);
    }
}
