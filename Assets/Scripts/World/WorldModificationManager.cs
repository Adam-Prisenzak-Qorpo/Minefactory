using UnityEngine;
using System.Collections.Generic;
using Minefactory.World.Tiles;
using Minefactory.Common;
using Minefactory.Save;
using System.Linq;

namespace Minefactory.World
{
    public class WorldModificationManager : MonoBehaviour
    {
        private Dictionary<Vector2Int, Dictionary<Vector2, TileModification>> chunkModifications = 
            new Dictionary<Vector2Int, Dictionary<Vector2, TileModification>>();
        
        private TileRegistry tileRegistry;
        private int chunkSize;

        public void Initialize(TileRegistry registry, int size)
        {
            tileRegistry = registry;
            chunkSize = size;
            Debug.Log("WorldModificationManager initialized");
        }

        public void SetModification(Vector2 worldPos, TileData tile, Orientation orientation, Dictionary<string, string> metadata = null)
        {
            Vector2Int chunkPos = WorldToChunkPosition(worldPos);
            Debug.Log($"Setting modification at {worldPos} with metadata: {(metadata != null ? string.Join(", ", metadata.Select(kvp => $"{kvp.Key}={kvp.Value}")) : "null")}");

            if (!chunkModifications.ContainsKey(chunkPos))
            {
                chunkModifications[chunkPos] = new Dictionary<Vector2, TileModification>();
            }

            if (tile == null)
            {
                chunkModifications[chunkPos][worldPos] = new TileModification("", Orientation.Up);
                return;
            }

            var modification = new TileModification(tile.tileName, orientation, metadata);
            chunkModifications[chunkPos][worldPos] = modification;
            
            // Verify the metadata was set correctly
            var savedMod = chunkModifications[chunkPos][worldPos];
            Debug.Log($"Verification - Metadata list count: {savedMod.metadataList?.Count ?? 0}");
            if (savedMod.metadataList != null)
            {
                foreach (var entry in savedMod.metadataList)
                {
                    Debug.Log($"Saved metadata entry: {entry.key}={entry.value}");
                }
            }
        }

        public void UpdateModificationMetadata(Vector2 worldPos, Dictionary<string, string> metadata)
        {
            Vector2Int chunkPos = WorldToChunkPosition(worldPos);
            Debug.Log($"Updating metadata at {worldPos}");
            if (chunkModifications.TryGetValue(chunkPos, out var modifications) &&
                modifications.TryGetValue(worldPos, out var modification))
            {
                modification.SetMetadata(metadata);
                modifications[worldPos] = modification;
                Debug.Log($"Updated metadata: {string.Join(", ", metadata.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
            }
        }

        public bool HasModification(Vector2 worldPos, out TileModification modification)
        {
            Vector2Int chunkPos = WorldToChunkPosition(worldPos);
            if (chunkModifications.TryGetValue(chunkPos, out var modifications) && 
                modifications.TryGetValue(worldPos, out modification))
            {
                return true;
            }
            modification = new TileModification(null, Orientation.Up);
            return false;
        }

        public Dictionary<string, string> GetModificationMetadata(Vector2 worldPos)
        {
            if (HasModification(worldPos, out TileModification modification))
            {
                var metadata = modification.GetMetadata();
                Debug.Log($"Retrieved metadata at {worldPos}: {string.Join(", ", metadata.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
                return metadata;
            }
            return new Dictionary<string, string>();
        }

        private Vector2Int WorldToChunkPosition(Vector2 worldPosition)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPosition.x / chunkSize),
                Mathf.FloorToInt(worldPosition.y / chunkSize)
            );
        }

        public List<ChunkData> GetModifications()
        {
            var chunkDataList = new List<ChunkData>();

            foreach (var chunkKvp in chunkModifications)
            {
                if (chunkKvp.Value.Count == 0) continue;

                string chunkKey = $"{chunkKvp.Key.x},{chunkKvp.Key.y}";
                var chunkData = new ChunkData(chunkKey);
                
                foreach (var modKvp in chunkKvp.Value)
                {
                    var posMod = new PositionModification(modKvp.Key, modKvp.Value);
                    chunkData.modifications.Add(posMod);
                }

                chunkDataList.Add(chunkData);
            }

            return chunkDataList;
        }

        public void LoadModifications(List<ChunkData> chunks)
        {
            if (chunks == null) return;
            chunkModifications.Clear();

            foreach (var chunkData in chunks)
            {
                if (string.IsNullOrEmpty(chunkData.chunkKey)) continue;

                string[] coords = chunkData.chunkKey.Split(',');
                if (coords.Length != 2) continue;

                if (!int.TryParse(coords[0], out int x) || !int.TryParse(coords[1], out int y))
                {
                    continue;
                }

                Vector2Int chunkPos = new Vector2Int(x, y);
                var modifications = new Dictionary<Vector2, TileModification>();

                foreach (var mod in chunkData.modifications)
                {
                    modifications[mod.Position] = mod.tile;
                }

                if (modifications.Count > 0)
                {
                    chunkModifications[chunkPos] = modifications;
                }
            }
        }

        public void LogAllModifications()
        {
            Debug.Log("Current modifications state:");
            foreach (var chunkKvp in chunkModifications)
            {
                foreach (var tileKvp in chunkKvp.Value)
                {
                    Debug.Log($"Position: {tileKvp.Key}, TileName: {tileKvp.Value.tileDataName}, MetadataCount: {tileKvp.Value.metadataList?.Count ?? 0}");
                    if (tileKvp.Value.metadataList != null)
                    {
                        foreach (var entry in tileKvp.Value.metadataList)
                        {
                            Debug.Log($"  Metadata: {entry.key}={entry.value}");
                        }
                    }
                }
            }
        }

        public void ClearModifications()
        {
            chunkModifications.Clear();
        }
    }
}