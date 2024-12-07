using UnityEngine;
using System.Collections.Generic;
using Minefactory.World.Tiles;
using Minefactory.Common;
using Minefactory.Storage.Items;
using Minefactory.Player.Inventory;
using Minefactory.World.Tiles.Behaviour;
using System.Linq;
using Minefactory.Save;

namespace Minefactory.World
{
    public abstract class BaseWorldGeneration : MonoBehaviour
    {
        [Header("Base Settings")]
        public Inventory playerInventory;
        public TileRegistry tileRegistry;
        public TileData backgroundTileData;
        
        [Header("Chunk Settings")]
        public Transform playerTransform;
        [SerializeField] protected int chunkSize = 16;
        [SerializeField] protected int renderDistance = 1;

        protected float seed;
        protected Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();
        protected WorldModificationManager modificationManager;

        // Event delegates for tile placement
        public delegate bool CanPlace(Vector2 position);
        public CanPlace canPlace;

        public delegate void OnItemSelect(ItemData item);
        public OnItemSelect onItemSelect;

        public delegate bool OnPlaceTile(Vector2 position, ItemData item, Orientation orientation);
        public OnPlaceTile onPlaceTile;

        public delegate bool OnTileRemoved(Vector2 position);
        public OnTileRemoved onTileRemoved;

        private bool initialized = false;

        public GameObject furnaceUI;


        protected virtual void Awake()
        {
            onItemSelect = SpawnGhostTile;
            onPlaceTile = HandlePlaceTile;
            canPlace = CanPlaceOnTile;
            onTileRemoved = OnRemoveTile;
        }

        public  virtual void InitializeWorld(float seed, List<ChunkData> modifications = null)
        {
            this.seed = seed;
            modificationManager = gameObject.AddComponent<WorldModificationManager>();
            modificationManager.Initialize(tileRegistry, chunkSize);
            modificationManager.LoadModifications(modifications);
            Debug.Log("World Initialized with seed: " + this.seed);
            initialized = true;
            UpdateChunks();
        }

        public float getSeed()
        {
            return seed;
        }

        protected void FixedUpdate()
        {
            if (!initialized) return;
            if (playerTransform == null)
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
                if (playerTransform == null)
                {
                    Debug.LogError("No player found! Make sure your player has the 'Player' tag.");
                }
            }
            UpdateChunks();
        }

        protected bool CanPlaceOnTile(Vector2 mousePosition)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mousePosition);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Solid"))
                {
                    return false;
                }
            }
            return true;
        }

        protected bool HandlePlaceTile(Vector2 position, ItemData item, Orientation orientation)
        {
            var tile = tileRegistry.GetTileByItem(item);
            if (tile && CanPlaceOnTile(position))
            {
                return PlaceTile(tile, position, orientation);
            }
            return false;
        }

        private void RecordTileModification(Vector2 position)
        {
            Collider2D collider = Physics2D.OverlapPoint(position);
            if (collider != null)
            {
                var tileBehaviour = collider.GetComponent<BaseTileBehaviour>();
                if (tileBehaviour != null && tileBehaviour.item != null)
                {
                    TileData tileData = tileRegistry.GetTileByItem(tileBehaviour.item);
                    if (tileData != null)
                    {
                        modificationManager.SetModification(position, tileData, tileBehaviour.orientation);
                    }
                }
            }
            else
            {
                // If no tile is present, record a null modification to indicate removal
                modificationManager.SetModification(position, null, Orientation.Up);
            }
        }

        protected bool OnRemoveTile(Vector2 position)
        {
            Vector2Int chunkPos = WorldToChunkPosition(position);
            if (loadedChunks.TryGetValue(chunkPos, out Chunk chunk))
            {
                GameObject tileObject = chunk.GetTileAtPosition(position);
                if (tileObject != null)
                {
                    string sortingLayer = tileObject.GetComponent<SpriteRenderer>().sortingLayerName;
                    if (chunk.RemoveTile(position, sortingLayer))
                    {
                        RecordTileModification(position);
                        return true;
                    }
                }
            }
            return false;
        }

        private void SpawnGhostTile(ItemData item)
        {
            TileData tileData = tileRegistry.GetTileByItem(item);
            GameObject tilePrefab = Instantiate(GetTilePrefab(tileData), Vector3.zero, Quaternion.identity);
            tilePrefab.tag = "Ghost";
            
            var tileGhost = tilePrefab.AddComponent<TileGhost>();
            tileGhost.tileData = tileData;
            tileGhost.transform.parent = transform;
            
            var sr = tilePrefab.GetComponent<SpriteRenderer>();
            sr.sortingLayerID = SortingLayer.NameToID("GhostTile");
            var color = sr.color;
            color.a = 0.5f;
            sr.color = color;
            
            tilePrefab.GetComponent<BoxCollider2D>().isTrigger = true;
            Destroy(tilePrefab.GetComponent<BaseTileBehaviour>());
        }

        protected virtual void UpdateChunks()
        {
            if (playerTransform == null) return;

            Vector2Int playerChunkPos = WorldToChunkPosition(playerTransform.position);
            
            // Calculate actual squared distance once
            float maxDistanceSquared = (renderDistance + 1) * (renderDistance + 1);

            // First, unload distant chunks
            List<Vector2Int> chunksToUnload = new List<Vector2Int>();
            foreach (var chunk in loadedChunks)
            {
                // Convert to Vector2 for distance calculation
                Vector2 offset = new Vector2(
                    playerChunkPos.x - chunk.Key.x,
                    playerChunkPos.y - chunk.Key.y
                );
                float distanceSquared = offset.sqrMagnitude;
                
                if (distanceSquared > maxDistanceSquared)
                {
                    chunksToUnload.Add(chunk.Key);
                }
            }

            foreach (var pos in chunksToUnload)
            {
                UnloadChunk(pos);
            }

            // Then load nearby chunks if needed
            int renderDistanceInt = Mathf.FloorToInt(renderDistance);
            for (int x = -renderDistanceInt; x <= renderDistanceInt; x++)
            {
                for (int y = -renderDistanceInt; y <= renderDistanceInt; y++)
                {
                    // Convert offset to Vector2 for distance check
                    Vector2 offset = new Vector2(x, y);
                    Vector2Int checkPos = playerChunkPos + new Vector2Int(x, y);
                    
                    // Check if this chunk is within our circular render distance
                    if (offset.sqrMagnitude <= maxDistanceSquared && !loadedChunks.ContainsKey(checkPos))
                    {
                        GenerateChunk(checkPos);
                    }
                }
            }
        }

        protected Vector2Int WorldToChunkPosition(Vector2 worldPosition)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPosition.x / chunkSize),
                Mathf.FloorToInt(worldPosition.y / chunkSize)
            );
        }

        protected Vector2 ChunkToWorldPosition(Vector2Int chunkPos)
        {
            return new Vector2(
                chunkPos.x * chunkSize,
                chunkPos.y * chunkSize
            );
        }

        protected virtual void GenerateChunk(Vector2Int chunkPos)
        {
            GameObject chunkObj = new GameObject($"Chunk_{chunkPos.x}_{chunkPos.y}");
            chunkObj.transform.parent = transform;
            
            Vector2 worldPos = ChunkToWorldPosition(chunkPos);
            chunkObj.transform.position = worldPos;

            Chunk chunk = chunkObj.AddComponent<Chunk>();
            loadedChunks.Add(chunkPos, chunk);
            chunk.Initialize(this, chunkPos, chunkSize);
        }

        protected virtual void UnloadChunk(Vector2Int chunkPos)
        {
            if (loadedChunks.TryGetValue(chunkPos, out Chunk chunk))
            {
                Destroy(chunk.gameObject);
                loadedChunks.Remove(chunkPos);
            }
        }

        public virtual bool PlaceTile(TileData tileData, Vector2 worldPosition, Orientation orientation = Orientation.Up, bool recordModification = true)
        {
            Vector2Int chunkPos = WorldToChunkPosition(worldPosition);
            if (!loadedChunks.TryGetValue(chunkPos, out Chunk chunk))
            {
                Debug.LogWarning($"Trying to place tile at {worldPosition} but chunk {chunkPos} not loaded");
                return false;
            }

            GameObject tilePrefab = GetTilePrefab(tileData);
            if (!tilePrefab)
            {
                Debug.LogError($"Tile prefab for {tileData.GetName()} not found.");
                return false;
            }

            Vector2 roundedPosition = new Vector2(
                Mathf.Round(worldPosition.x), 
                Mathf.Round(worldPosition.y)
            );
            
            GameObject newTile = Instantiate(
                tilePrefab,
                roundedPosition,
                Quaternion.Euler(0, 0, (int)orientation * -90),
                chunk.transform
            );
            
            BaseTileBehaviour tileBehaviour = newTile.GetComponent<BaseTileBehaviour>();
            if (tileBehaviour)
            {
                tileBehaviour.orientation = orientation;
            }

            chunk.RegisterTile(roundedPosition, newTile);
            
            if (recordModification)
            {
                RecordTileModification(roundedPosition);
            }
            
            return true;
        }

        protected float GetPerlinNoiseValue(Vector2 worldPosition, float frequency)
        {
            return Mathf.PerlinNoise(
                (worldPosition.x + seed) * frequency,
                (worldPosition.y + seed) * frequency
            );
        }

        public virtual void GenerateTerrainAt(Vector2 chunkWorldPos, int localX, int localY)
        {
            Vector2 worldPos = chunkWorldPos + new Vector2(localX, localY);
            
            // Always place background tile first
            PlaceTile(backgroundTileData, worldPos, Orientation.Up, false);

            bool hasModification = modificationManager.HasModification(worldPos, out TileModification modification);

            if (hasModification)
            {
                if (!string.IsNullOrEmpty(modification.tileDataName))
                {
                    // Place the modified tile
                    TileData tileData = tileRegistry.GetItem(modification.tileDataName);
                    if (tileData != null)
                    {
                        PlaceTile(tileData, worldPos, modification.orientation, false);
                    }
                }
                // If modification exists but tileDataName is empty string, it means the tile was explicitly removed
                // In this case, we only keep the background tile and don't generate default terrain
            }
            else
            {
                // Generate default terrain if no modifications exist
                GenerateDefaultTerrain(chunkWorldPos, localX, localY);
            }
        }

        protected abstract void GenerateDefaultTerrain(Vector2 chunkWorldPos, int localX, int localY);
        public abstract GameObject GetTilePrefab(TileData tileData);
    }
}