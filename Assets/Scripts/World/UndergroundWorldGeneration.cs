using Minefactory.World.Ores;
using Minefactory.World.Tiles;
using UnityEngine;

namespace Minefactory.World
{
    public class UndergroundWorldGeneration : BaseWorldGeneration
    {
        [Header("Underground Settings")]
        public float heightMultiplier = 2f;
        public float dirtHeight = 10f;
        public bool generateCaves = true;
        public float caveFrequency = 0.05f;
        public float terrainFrequency = 0.05f;
        
        [Header("Ore Settings")]
        public OreRegistry oreRegistry;

        private Texture2D caveNoiseTexture;

        protected override void GenerateNoiseTextures()
        {
            if (generateCaves)
            {
                caveNoiseTexture = GenerateNoiseTexture(0.25f, caveFrequency);
                if (oreRegistry != null)
                {
                    foreach (var ore in oreRegistry.list)
                    {
                        ore.SetNoiseTexture(GenerateNoiseTexture(ore.rarity, ore.size));
                    }
                }
            }
        }
        protected override GameObject GetTilePrefab(TileData tileData) => tileData.underGroundTilePrefab;

        protected override void GenerateWorld()
        {
            for (int x = 0; x < worldSize; x++)
            {
                var height = Mathf.PerlinNoise((x + seed) * terrainFrequency, seed * terrainFrequency) 
                    * heightMultiplier + dirtHeight;
                
                for (int y = 0; y < worldSize; y++)
                {
                    var position = new Vector2(x, y);
                    
                    PlaceTile(backgroundTileData, position);
                    
                    if (IsSpawnArea(position)) continue;

                    if (y > (worldSize - height))
                    {
                        PlaceTile(tileRegistry.GetItem("dirt"), position);
                        continue;
                    }

                    if (generateCaves && caveNoiseTexture.GetPixel(x, y).r < 0.5f)
                        continue;

                    bool orePlaced = false;
                    if (oreRegistry != null)
                    {
                        foreach (var ore in oreRegistry.list)
                        {
                            if (ore.CanPlace(worldSize, x, y))
                            {
                                PlaceTile(ore.tile, position);
                                orePlaced = true;
                                break;
                            }
                        }
                    }

                    if (!orePlaced)
                        PlaceTile(tileRegistry.GetItem("stone"), position);
                }
            }
        }

        private bool IsSpawnArea(Vector2 position)
        {
            return position.y >= 90 && position.y <= 92 && position.x >= 49 && position.x <= 51;
        }
    }
}
