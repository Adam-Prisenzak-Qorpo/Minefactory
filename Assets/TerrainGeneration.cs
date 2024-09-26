using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public int worldSize = 100;
    public float heightMultiplier = 2f;
    public float heightAddition = 10f;
    public TileClass stone;
    public TileClass dirt;
    public float caveFrequency = 0.05f;
    public float terrainFrequency = 0.05f;
    public float seed;
    public Texture2D noiseTexture;
    private readonly List<Vector2> tiles = new();

    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(-10_000, 10_000);
        GenerateNoiseTexture();
        GenerateTerrain();
    }

    private void GenerateNoiseTexture()
    {
        noiseTexture = new Texture2D(worldSize, worldSize) { filterMode = FilterMode.Point };
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float noiseValue = Mathf.PerlinNoise(
                    (x + seed) * caveFrequency,
                    (y + seed) * caveFrequency
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
                    PlaceTile(dirt, new Vector2(x, y));
                    continue;
                }

                float noiseValue = noiseTexture.GetPixel(x, y).r;
                if (noiseValue > 0.2f)
                {
                    PlaceTile(stone, new Vector2(x, y));
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
