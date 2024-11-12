using UnityEngine;
using Minefactory.World.Tiles;

namespace Minefactory.World
{
    public class TopWorldGeneration : BaseWorldGeneration
    {
        [Header("Surface Settings")]
        public int safeRadius = 10;
        public float woodFrequency = 0.05f;
        public float stoneFrequency = 0.05f;

        private Texture2D woodNoiseTexture;
        private Texture2D stoneNoiseTexture;

        private Vector2 MapCenter => new Vector2(worldSize / 2, worldSize / 2);

        protected override void GenerateNoiseTextures()
        {
            woodNoiseTexture = GenerateNoiseTexture(0.25f, woodFrequency);
            stoneNoiseTexture = GenerateNoiseTexture(0.5f, stoneFrequency);
        }

        protected override GameObject GetTilePrefab(TileData tileData) => tileData.topTilePrefab;

        protected override void GenerateWorld()
        {
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    var position = new Vector2(x, y);
                    
                    PlaceTile(backgroundTileData, position);

                    if (Vector2.Distance(position, MapCenter) < safeRadius)
                        continue;

                    if (woodNoiseTexture.GetPixel(x, y).grayscale < 0.5f)
                    {
                        PlaceTile(tileRegistry.GetItem("wood"), position);
                        continue;
                    }

                    if (stoneNoiseTexture.GetPixel(x, y).grayscale < 0.5f)
                    {
                        PlaceTile(tileRegistry.GetItem("stone"), position);
                    }
                }
            }
        }
    }
}
