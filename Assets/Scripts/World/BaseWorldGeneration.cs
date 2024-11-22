using System.Collections.Generic;
using Minefactory.World.Tiles;
using Minefactory.Storage.Items;
using Minefactory.Common;
using Minefactory.Player.Inventory;
using Minefactory.World.Tiles.Behaviour;
using UnityEngine;

namespace Minefactory.World
{
    public abstract class BaseWorldGeneration : MonoBehaviour
    {
        [Header("Base Settings")]
        public Inventory playerInventory;
        public TileRegistry tileRegistry;
        public int worldSize = 100;

        public TileData backgroundTileData;

        protected float seed;
        protected readonly List<Vector2> tiles = new();

        // Event delegates for tile placement
        public delegate bool CanPlace(Vector2 position);
        public static CanPlace canPlace;

        public delegate void OnItemSelect(ItemData item);
        public static OnItemSelect onItemSelect;

        public delegate bool OnPlaceTile(Vector2 position, ItemData item, Orientation orientation);
        public static OnPlaceTile onPlaceTile;

        public delegate bool OnTileRemoved(Vector2 position);
        public static OnTileRemoved onTileRemoved;

        protected virtual void Start()
        {
            InitializeWorld();
            GenerateWorld();
        }

        protected virtual void InitializeWorld()
        {
            seed = Random.Range(-10_000, 10_000);
            onItemSelect = SpawnGhostTile;
            onPlaceTile = OnPlaceTileHandler;
            canPlace = CanPlaceOnTile;
            onTileRemoved = OnRemoveTile;
            GenerateNoiseTextures();
        }

        protected abstract void GenerateNoiseTextures();
        protected abstract void GenerateWorld();

        protected Texture2D GenerateNoiseTexture(float limit, float noiseFrequency)
        {
            var noiseTexture = new Texture2D(worldSize, worldSize) { filterMode = FilterMode.Point };
            for (int x = 0; x < noiseTexture.width; x++)
            {
                for (int y = 0; y < noiseTexture.height; y++)
                {
                    float noiseValue = Mathf.PerlinNoise(
                        (x + seed) * noiseFrequency,
                        (y + seed) * noiseFrequency
                    );
                    noiseTexture.SetPixel(x, y, noiseValue > limit ? Color.white : Color.black);
                }
            }
            noiseTexture.Apply();
            return noiseTexture;
        }

        private bool OnPlaceTileHandler(Vector2 position, ItemData item, Orientation orientation)
        {
            var tile = tileRegistry.GetTileByItem(item);
            if (tile && CanPlaceOnTile(position))
            {
                return PlaceTile(tile, position, orientation);
            }
            return false;
        }

        // private void RotateSpriteByOrientation(SpriteRenderer spriteRenderer, rotation=)
        // {
        //     spriteRenderer.transform.Rotate(0, 0, (int)orientation * -90);
        // }

        protected bool OnRemoveTile(Vector2 position)
        {
            foreach (var tile in tiles)
            {
                if (tile.x == Mathf.Round(position.x) && tile.y == Mathf.Round(position.y))
                {
                    tiles.Remove(tile);
                    return true;
                }
            }
            return false;
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



        private Quaternion GetRotationByOrientation(Orientation orientation)
        {
            return Quaternion.Euler(0, 0, (int)orientation * -90);
        }

        protected virtual bool PlaceTile(TileData tileData, Vector2 position, Orientation orientation = Orientation.Up)
        {
            GameObject tilePrefab = GetTilePrefab(tileData);
            if (!tilePrefab)
            {
                Debug.LogError($"Tile prefab for {tileData.GetName()} not found.");
                return false;
            }
            GameObject newTile = Instantiate(
                tilePrefab,
                new Vector2(Mathf.Round(position.x), Mathf.Round(position.y)),
                GetRotationByOrientation(orientation)
            );
            BaseTileBehaviour tileBehaviour = newTile.GetComponent<BaseTileBehaviour>();
            newTile.transform.parent = transform;
            if (tileBehaviour)
            {
                tileBehaviour.orientation = orientation;
                tileBehaviour.playerInventory = playerInventory;
            }

            return true;
        }
        protected abstract GameObject GetTilePrefab(TileData tileData);

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
    }
}