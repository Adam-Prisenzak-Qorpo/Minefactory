using UnityEngine;
using System.Collections.Generic;
using Minefactory.World.Tiles;
using Minefactory.Common;
using Minefactory.Save;

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
        }

        public void SetModification(Vector2 worldPos, TileData tile, Orientation orientation)
        {
            Vector2Int chunkPos = WorldToChunkPosition(worldPos);

            if (!chunkModifications.ContainsKey(chunkPos))
            {
                chunkModifications[chunkPos] = new Dictionary<Vector2, TileModification>();
            }

            if (tile == null)
            {
                chunkModifications[chunkPos][worldPos] = new TileModification("", Orientation.Up);
                return;
            }

            chunkModifications[chunkPos][worldPos] = new TileModification(tile.tileName, orientation);
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

        public void ClearModifications()
        {
            chunkModifications.Clear();
        }
    }
}