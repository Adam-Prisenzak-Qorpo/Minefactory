using UnityEngine;
using System.Collections.Generic;
using Minefactory.World.Tiles;
using Minefactory.Common;
using Minefactory.World.Tiles.Behaviour;

namespace Minefactory.World
{
    public class Chunk : MonoBehaviour
    {
        private BaseWorldGeneration worldGenerator;
        private Vector2Int chunkPosition;
        private int size;
        private Dictionary<Vector2, GameObject> tiles = new Dictionary<Vector2, GameObject>();
        private WorldModificationManager modificationManager;


        public void Initialize(BaseWorldGeneration generator, Vector2Int pos, int chunkSize)
        {
            worldGenerator = generator;
            chunkPosition = pos;
            size = chunkSize;
            modificationManager = worldGenerator.GetComponent<WorldModificationManager>();
            GenerateChunk();

        }
        

        private void GenerateChunk()
        {
            Vector2 worldStart = new Vector2(
                chunkPosition.x * size,
                chunkPosition.y * size
            );

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    worldGenerator.GenerateTerrainAt(worldStart, x, y);
                }
            }
        }

        public void RegisterTile(Vector2 worldPosition, GameObject tile)
        {
            var existingTile = GetTileAtPosition(worldPosition);
            if (existingTile != null)
            {
                var existingLayer = existingTile.GetComponent<SpriteRenderer>().sortingLayerName;
                var newLayer = tile.GetComponent<SpriteRenderer>().sortingLayerName;
                
                if (existingLayer == newLayer)
                {
                    RemoveTile(worldPosition, existingLayer);
                }
            }

            tiles[worldPosition] = tile;
        }

        public GameObject GetTileAtPosition(Vector2 worldPosition)
        {
            tiles.TryGetValue(worldPosition, out GameObject tile);
            return tile;
        }

        public bool RemoveTile(Vector2 worldPosition, string sortingLayer = null)
        {
            if (tiles.TryGetValue(worldPosition, out GameObject tile))
            {
                if (sortingLayer != null)
                {
                    var tileLayer = tile.GetComponent<SpriteRenderer>().sortingLayerName;
                    if (tileLayer != sortingLayer) return false;
                }

                Destroy(tile);
                tiles.Remove(worldPosition);
                return true;
            }
            return false;
        }

        private void OnDestroy()
        {

            foreach (var tile in tiles.Values)
            {
                if (tile != null)
                {
                    Destroy(tile);
                }
            }
            tiles.Clear();
        }
    }
}