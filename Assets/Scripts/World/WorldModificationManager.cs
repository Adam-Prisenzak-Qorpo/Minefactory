using UnityEngine;
using System.Collections.Generic;
using Minefactory.World.Tiles;
using Minefactory.Common;
using Minefactory.Save;
using System.Linq;
using Minefactory.World.Tiles.Behaviour;

namespace Minefactory.World
{
    public class WorldModificationManager : MonoBehaviour
    {
        private Dictionary<Vector2Int, Dictionary<Vector2, TileModification>> chunkModifications = 
            new Dictionary<Vector2Int, Dictionary<Vector2, TileModification>>();
        
        // Tracking for persistent chunks and tiles
        private HashSet<Vector2Int> persistentChunks = new HashSet<Vector2Int>();
        private Dictionary<Vector2, PersistentTileBehaviour> persistentTiles = 
            new Dictionary<Vector2, PersistentTileBehaviour>();

        private TileRegistry tileRegistry;
        private int chunkSize;

        public void Initialize(TileRegistry registry, int size)
        {
            tileRegistry = registry;
            chunkSize = size;
            Debug.Log("WorldModificationManager initialized");
        }

        public void RegisterPersistentTile(Vector2 position, PersistentTileBehaviour tile)
        {
            Debug.Log($"Registering persistent tile at {position}");
            Vector2Int chunkPos = WorldToChunkPosition(position);
            persistentTiles[position] = tile;
            persistentChunks.Add(chunkPos);

            // Check if we're near a chunk boundary (within 0.1 units)
            float epsilon = 0.1f;
            
            // Calculate position within chunk
            float xInChunk = position.x - (chunkPos.x * chunkSize);
            float yInChunk = position.y - (chunkPos.y * chunkSize);

            // Check boundaries and add adjacent chunks if necessary
            if (xInChunk < epsilon) // Near left boundary
            {
                persistentChunks.Add(new Vector2Int(chunkPos.x - 1, chunkPos.y));
            }
            else if (xInChunk > (chunkSize - epsilon)) // Near right boundary
            {
                persistentChunks.Add(new Vector2Int(chunkPos.x + 1, chunkPos.y));
            }

            if (yInChunk < epsilon) // Near bottom boundary
            {
                persistentChunks.Add(new Vector2Int(chunkPos.x, chunkPos.y - 1));
            }
            else if (yInChunk > (chunkSize - epsilon)) // Near top boundary
            {
                persistentChunks.Add(new Vector2Int(chunkPos.x, chunkPos.y + 1));
            }

            Debug.Log($"Registered persistent tile at position {position} in chunk {chunkPos}");
        }

        public void UnregisterPersistentTile(Vector2 position)
        {
            if (persistentTiles.Remove(position))
            {
                // Recalculate all needed chunks
                persistentChunks.Clear();
                
                // Rebuild the persistent chunks set from remaining tiles
                foreach (var kvp in persistentTiles)
                {
                    Vector2Int chunkPos = WorldToChunkPosition(kvp.Key);
                    persistentChunks.Add(chunkPos);
                    
                    // Re-apply boundary checks for remaining tiles
                    float xInChunk = kvp.Key.x - (chunkPos.x * chunkSize);
                    float yInChunk = kvp.Key.y - (chunkPos.y * chunkSize);
                    float epsilon = 0.1f;

                    if (xInChunk < epsilon)
                    {
                        persistentChunks.Add(new Vector2Int(chunkPos.x - 1, chunkPos.y));
                    }
                    else if (xInChunk > (chunkSize - epsilon))
                    {
                        persistentChunks.Add(new Vector2Int(chunkPos.x + 1, chunkPos.y));
                    }

                    if (yInChunk < epsilon)
                    {
                        persistentChunks.Add(new Vector2Int(chunkPos.x, chunkPos.y - 1));
                    }
                    else if (yInChunk > (chunkSize - epsilon))
                    {
                        persistentChunks.Add(new Vector2Int(chunkPos.x, chunkPos.y + 1));
                    }
                }
            }
        }

        public bool IsPersistentChunk(Vector2Int chunkPos)
        {
            return persistentChunks.Contains(chunkPos);
        }

        public HashSet<Vector2Int> GetPersistentChunks()
        {
            return persistentChunks;
        }

        public void SetModification(Vector2 worldPos, TileData tile, Orientation orientation, Dictionary<string, string> metadata = null)
        {
            Vector2Int chunkPos = WorldToChunkPosition(worldPos);
            
            // Create a clean copy of the metadata
            Dictionary<string, string> cleanMetadata = new Dictionary<string, string>();
            if (metadata != null)
            {
                foreach (var kvp in metadata)
                {
                    cleanMetadata[kvp.Key] = kvp.Value;
                }
            }

            // Only add isPersistent if it's not already there
            if (persistentTiles.ContainsKey(worldPos) && !cleanMetadata.ContainsKey("isPersistent"))
            {
                cleanMetadata["isPersistent"] = "true";
            }

            Debug.Log($"Setting modification at {worldPos} with metadata: {string.Join(", ", cleanMetadata.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");

            if (!chunkModifications.ContainsKey(chunkPos))
            {
                chunkModifications[chunkPos] = new Dictionary<Vector2, TileModification>();
            }

            if (tile == null)
            {
                chunkModifications[chunkPos][worldPos] = new TileModification("", Orientation.Up);
                return;
            }

            var modification = new TileModification(tile.tileName, orientation, cleanMetadata);
            chunkModifications[chunkPos][worldPos] = modification;
        }

        public void UpdateModificationMetadata(Vector2 worldPos, Dictionary<string, string> metadata)
        {
            Vector2Int chunkPos = WorldToChunkPosition(worldPos);
            Debug.Log($"Updating metadata at {worldPos}");
            
            if (chunkModifications.TryGetValue(chunkPos, out var modifications) &&
                modifications.TryGetValue(worldPos, out var modification))
            {
                // Create a clean copy of the metadata
                Dictionary<string, string> cleanMetadata = new Dictionary<string, string>();
                if (metadata != null)
                {
                    foreach (var kvp in metadata)
                    {
                        cleanMetadata[kvp.Key] = kvp.Value;
                    }
                }

                // Preserve isPersistent flag if the tile is persistent
                if (persistentTiles.ContainsKey(worldPos))
                {
                    cleanMetadata["isPersistent"] = "true";
                }

                modification.SetMetadata(cleanMetadata);
                modifications[worldPos] = modification;
                Debug.Log($"Updated metadata: {string.Join(", ", cleanMetadata.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
            }
        }

        private Vector2Int WorldToChunkPosition(Vector2 worldPosition)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPosition.x / chunkSize),
                Mathf.FloorToInt(worldPosition.y / chunkSize)
            );
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

                // Only add chunks that actually have modifications
                if (chunkData.modifications.Count > 0)
                {
                    chunkDataList.Add(chunkData);
                }
            }

            return chunkDataList;
        }

        public void LoadModifications(List<ChunkData> chunks)
        {
            if (chunks == null) return;
            chunkModifications.Clear();
            persistentChunks.Clear();
            persistentTiles.Clear();

            foreach (var chunkData in chunks)
            {
                if (string.IsNullOrEmpty(chunkData.chunkKey)) continue;

                string[] coords = chunkData.chunkKey.Split(',');
                if (coords.Length != 2) continue;

                if (!int.TryParse(coords[0], out int x) || !int.TryParse(coords[1], out int y))
                    continue;

                Vector2Int chunkPos = new Vector2Int(x, y);
                var modifications = new Dictionary<Vector2, TileModification>();

                foreach (var mod in chunkData.modifications)
                {
                    modifications[mod.Position] = mod.tile;
                    
                    // Check if this is a persistent tile
                    if (mod.tile.metadataList?.Any(m => m.key == "isPersistent" && m.value == "true") ?? false)
                    {
                        persistentChunks.Add(chunkPos);
                    }
                }

                if (modifications.Count > 0)
                {
                    chunkModifications[chunkPos] = modifications;
                }
            }
        }

        public void ClearModifications()
        {
            chunkModifications.Clear();
            persistentChunks.Clear();
            persistentTiles.Clear();
        }

        public void ClearModification(Vector2 worldPos)
        {
            Vector2Int chunkPos = WorldToChunkPosition(worldPos);
            if (chunkModifications.TryGetValue(chunkPos, out var modifications))
            {
                modifications.Remove(worldPos);
                if (modifications.Count == 0)
                {
                    chunkModifications.Remove(chunkPos);
                }
            }
            Debug.Log($"Cleared modification at position {worldPos}");
        }

    }
}