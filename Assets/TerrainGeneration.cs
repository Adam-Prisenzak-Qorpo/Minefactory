using System.Collections;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public int worldSize = 100;
    public Sprite sprite;
    public float noiseFrequency = 0.05f;
    public float seed;
    public Texture2D noiseTexture;

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
            for (int y = 0; y < worldSize; y++)
            {
                float noiseValue = noiseTexture.GetPixel(x, y).r;
                if (noiseValue > 0.5f)
                {
                    var tile = new GameObject("Tile");
                    tile.transform.parent = this.transform;
                    tile.transform.position = new Vector2(x, y);
                    tile.AddComponent<SpriteRenderer>().sprite = sprite;
                }
            }
        }
    }
}
