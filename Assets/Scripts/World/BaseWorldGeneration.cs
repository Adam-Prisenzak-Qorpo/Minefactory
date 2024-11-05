using System.Collections.Generic;
using Minefactory.Storage;
using Minefactory.World.Tiles;
using Minefactory.Storage.Items;
using UnityEngine;

namespace Minefactory.World
{
    public abstract class BaseWorldGeneration : MonoBehaviour
    {
        [Header("Base Settings")]
        public StorageData playerInventory;
        public TileRegistry tileRegistry;
        public int worldSize = 100;
        
        protected float seed;
        protected readonly List<Vector2> tiles = new();

        // Event delegates for tile placement
        public delegate bool CanPlace(Vector2 position);
        public static CanPlace canPlace;

        public delegate bool OnTilePlaced(Vector2 position, ItemData item);
        public static OnTilePlaced onTilePlaced;

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
            onTilePlaced = OnPlaceTile;
            canPlace = CanPlaceOnTile;
            onTileRemoved = OnRemoveTile;
            GenerateNoiseTextures();
        }

        protected abstract void GenerateNoiseTextures();
        protected abstract void GenerateWorld();
        protected abstract string GetBackgroundTileName();

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

        protected bool OnPlaceTile(Vector2 position, ItemData item)
        {
            var tile = tileRegistry.GetTileByItem(item);
            if (tile)
            {
                return PlaceTile(tile, position);
            }
            return false;
        }

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

        protected bool CanPlaceOnTile(Vector2 position)
        {
            foreach (var tile in tiles)
            {
                if (tile.x == Mathf.Round(position.x) && tile.y == Mathf.Round(position.y))
                {
                    return false;
                }
            }
            return true;
        }

        protected virtual void PlaceBackgroundTile(Vector2 position)
        {
            PlaceTile(tileRegistry.GetItem(GetBackgroundTileName()), position, false);
        }

        protected virtual bool PlaceTile(TileData tile, Vector2 position, bool isSolid = true)
        {
            if (isSolid && !CanPlaceOnTile(position))
                return false;

            var newTile = new GameObject(tile.GetName());
            newTile.transform.parent = transform;
            newTile.transform.position = new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
            
            var spriteRenderer = newTile.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetTileSprite(tile);

            if (isSolid)
            {
                SetupSolidTile(newTile, tile);
            }
            else
            {
                SetupBackgroundTile(newTile, position);
            }
            return true;
        }

        protected abstract Sprite GetTileSprite(TileData tile);
        protected virtual void SetupBackgroundTile(GameObject tile, Vector2 position)
        {
            tile.layer = LayerMask.NameToLayer("Background");
            tile.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
            tile.GetComponent<SpriteRenderer>().sortingOrder = -1;
        }

        protected virtual void SetupSolidTile(GameObject tile, TileData tileData)
        {
            var entityClass = tile.AddComponent<TileEntityClass>();
            entityClass.playerInventory = playerInventory;
            entityClass.item = tileData.item;
            tile.AddComponent<BoxCollider2D>();
            tile.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
            tile.tag = "Solid";
            tile.GetComponent<SpriteRenderer>().sortingLayerName = "Solid";
            tiles.Add(tile.transform.position);
        }
    }
}