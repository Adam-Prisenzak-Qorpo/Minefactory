using UnityEngine;
using Minefactory.World.Tiles;
using Minefactory.World.Ores;
using System.Collections.Generic;
using Minefactory.Save;

namespace Minefactory.World
{
    public class TopWorldGeneration : BaseWorldGeneration
    {
        [Header("Surface Settings")]
        public int safeRadius = 10;
        public float siliconeFrequency = 0.05f;
        public float stoneFrequency = 0.05f;

        private Vector2 spawnPoint = Vector2.zero;

        public override void InitializeWorld(float seed, List<ChunkData> modifications = null)
        {
            base.InitializeWorld(seed, modifications);
        }

        public override GameObject GetTilePrefab(TileData tileData) => tileData.topTilePrefab;

        protected override void GenerateDefaultTerrain(Vector2 chunkWorldPos, int localX, int localY)
        {
            Vector2 worldPos = chunkWorldPos + new Vector2(localX, localY);
            Vector2Int chunkPos = WorldToChunkPosition(worldPos);

            PlaceTile(backgroundTileData, worldPos);

            if (Vector2.Distance(worldPos, spawnPoint) < safeRadius)
            {
                return;
            }

            float siliconeNoise = GetPerlinNoiseValue(worldPos, siliconeFrequency);
            if (siliconeNoise > 0.75f)
            {
                PlaceTile(tileRegistry.GetItem("silicone"), worldPos);
                return;
            }

            float stoneNoise = GetPerlinNoiseValue(worldPos, stoneFrequency);
            if (stoneNoise > 0.5f)
            {
                PlaceTile(tileRegistry.GetItem("stone"), worldPos);
            }
        }
    }
}