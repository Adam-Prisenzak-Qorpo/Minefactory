using UnityEngine;
using Minefactory.World.Tiles;
using Minefactory.World.Ores;
using System.Collections.Generic;
using Minefactory.Save;
using System.IO;
using System.Linq;

namespace Minefactory.World
{
    public class UndergroundWorldGeneration : BaseWorldGeneration
    {
        [Header("Underground Settings")]
        public float dirtLevel = 50f;
        public bool generateCaves = true;
        public float caveFrequency = 0.05f;
        
        [Header("Spawn Settings")]
        public Vector2 spawnPoint = Vector2.zero;
        public float spawnAreaRadius = 2f;

        [Header("Ore Settings")]
        public OreRegistry oreRegistry;
        
        private Dictionary<string, float> oreSeeds = new Dictionary<string, float>();

        public override void InitializeWorld(float seed, List<ChunkData> modifications = null)
        {
            base.InitializeWorld(seed, modifications);
            InitializeOreSeeds();
        }

        private void InitializeOreSeeds()
        {
            if (oreRegistry == null || oreRegistry.list == null) return;

            foreach (var ore in oreRegistry.list)
            {
                float oreSeed = GenerateOreSeed(ore.GetName());
                oreSeeds[ore.GetName()] = oreSeed;
            }
        }

        private float GenerateOreSeed(string oreName)
        {
            // Create a unique seed based on the ore name
            float nameSeed = 0;
            for (int i = 0; i < oreName.Length; i++)
            {
                nameSeed += oreName[i] * Mathf.Pow(31, i);
            }
            return nameSeed;
        }

        protected float GetOreNoiseValue(Vector2 worldPosition, OreData ore)
        {
            float oreSeed = oreSeeds.GetValueOrDefault(ore.GetName(), 0f);
            float combinedSeed = seed + oreSeed;
            
            return Mathf.PerlinNoise(
                (worldPosition.x + combinedSeed) * ore.frequency,
                (worldPosition.y + combinedSeed) * ore.frequency
            );
        }

        public override GameObject GetTilePrefab(TileData tileData) => tileData.underGroundTilePrefab;

        protected override void GenerateDefaultTerrain(Vector2 chunkWorldPos, int localX, int localY)
        {
            Vector2 worldPos = chunkWorldPos + new Vector2(localX, localY);

            PlaceTile(backgroundTileData, worldPos);

            if (IsSpawnArea(worldPos))
            {
                return;
            }

            if (generateCaves)
            {
                float caveNoise = GetPerlinNoiseValue(worldPos, caveFrequency);
                if (caveNoise < 0.25f)
                {
                    return;
                }
            }

            if (worldPos.y >= dirtLevel)
            {
                PlaceTile(tileRegistry.GetItem("dirt"), worldPos);
                return;
            }
            GenerateOreOrStone(worldPos);
        }

        private void GenerateOreOrStone(Vector2 worldPos)
        {
            if (oreRegistry == null || oreRegistry.list == null)
            {
                PlaceTile(tileRegistry.GetItem("stone"), worldPos);
                return;
            }

            // Sort ores by rarity (most rare first) to ensure rare ores have priority
            var sortedOres = oreRegistry.list
                .Where(ore => ore.depth > worldPos.y) 
                .OrderBy(ore => ore.rarity)          
                .ToList();

            bool orePlaced = false;

            foreach (var ore in sortedOres)
            {
                float depthDifference = ore.depth - worldPos.y;
                float depthFactor = Mathf.Clamp01(depthDifference / ore.depthFalloff);
                
                float noise = GetOreNoiseValue(worldPos, ore);
                
                // Calculate spawn threshold based on rarity and depth influence
                float threshold = ore.rarity + (depthFactor * ore.depthInfluence);
                
                if (noise < threshold)
                {
                    PlaceTile(ore.tile, worldPos);
                    orePlaced = true;
                    break;
                }
            }

            // Place stone if no ore was placed
            if (!orePlaced)
            {
                PlaceTile(tileRegistry.GetItem("stone"), worldPos);
            }
        }

        private bool IsSpawnArea(Vector2 position)
        {
            float distance = Vector2.Distance(position, spawnPoint);
            return distance < spawnAreaRadius;
        }
    }
}