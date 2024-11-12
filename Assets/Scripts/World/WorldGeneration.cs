using System.Collections.Generic;
using Minefactory.Common;
using Minefactory.Player.Inventory;
using Minefactory.Storage;
using Minefactory.Storage.Items;
using Minefactory.World.Tiles;
using Minefactory.World.Tiles.Behaviour;
using UnityEditor;
using UnityEngine;


namespace Minefactory.World
{
    public class WorldGeneration : MonoBehaviour
    {
        [Header("Inventory Settings")]
        public Inventory playerInventory;

        [Header("Tile Settings")]
        public TileRegistry tileRegistry;

        // public OreContainer oreContainer;


        [Header("World Generation Settings")]
        public int worldSize = 100;
        public int safeRadius = 10;

        [Header("Noise Settings")]
        public float seed;
        public float woodFrequency = 0.05f;
        public Texture2D woodNoiseTexture;

        public float stoneFrequency = 0.05f;
        public Texture2D stoneNoiseTexture;
        public readonly List<Vector2> tiles = new();

        private Vector2 MapCenter => new Vector2(worldSize / 2, worldSize / 2);

        public delegate void OnItemSelect(ItemData item);
        public static OnItemSelect onItemSelect;

        public delegate bool CanPlace(Vector2 position);
        public static CanPlace canPlace;

        public delegate bool OnPlaceTile(Vector2 position, ItemData item, bool isSolid, Orientation orientation);
        public static OnPlaceTile onPlaceTile;

        public delegate bool OnTileRemoved(Vector2 position);
        public static OnTileRemoved onTileRemoved;

        // Start is called before the first frame update
        void Start()
        {
            onItemSelect = SpawnGhostTile;
            onPlaceTile = OnPlaceTileHandler;
            canPlace = CanPlaceOnTile;
            onTileRemoved = OnRemoveTile;
            seed = Random.Range(-10_000, 10_000);
            woodNoiseTexture = GenerateNoiseTexture(0.25f, woodFrequency);
            stoneNoiseTexture = GenerateNoiseTexture(0.5f, stoneFrequency);
            GenerateTerrain();
        }

        private Texture2D GenerateNoiseTexture(float limit, float noiseFrequency)
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
                    if (noiseValue > limit)
                        noiseTexture.SetPixel(x, y, Color.white);
                    else
                        noiseTexture.SetPixel(x, y, Color.black);
                }
            }

            noiseTexture.Apply();
            return noiseTexture;
        }

        private void GenerateTerrain()
        {
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    var position = new Vector2(x, y);
                    PlaceBackgroundTile(tileRegistry.GetItem("dirt"), position);

                    if (Vector2.Distance(position, MapCenter) < safeRadius)
                    {
                        continue;
                    }

                    var woodValue = woodNoiseTexture.GetPixel(x, y).grayscale;
                    if (woodValue < 0.5f)
                    {
                        PlaceTile(tileRegistry.GetItem("wood"), position);
                    }
                    var stoneValue = stoneNoiseTexture.GetPixel(x, y).grayscale;
                    if (stoneValue < 0.5f)
                    {
                        PlaceTile(tileRegistry.GetItem("stone"), position);
                    }
                }
            }
        }

        private bool OnPlaceTileHandler(Vector2 position, ItemData item, bool isSolid, Orientation orientation)
        {
            var tile = tileRegistry.GetTileByItem(item);
            if (tile)
            {
                return PlaceTile(tile, position, isSolid, orientation);
            }
            return false;
        }

        private bool OnRemoveTile(Vector2 position)
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

        private bool CanPlaceOnTile(Vector2 position)
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

        private void RotateSpriteByOrientation(SpriteRenderer spriteRenderer, Orientation orientation)
        {
            spriteRenderer.transform.Rotate(0, 0, (int)orientation * -90);
        }

        private bool PlaceTile(TileData tile, Vector2 position, bool isSolid = true, Orientation orientation = Orientation.Up)
        {
            if (!CanPlaceOnTile(position))
                return false;
            var newTile = new GameObject(tile.GetName());

            newTile.transform.position = new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));

            var spriteRenderer = newTile.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = tile.topTileSprite;
            spriteRenderer.sortingLayerID = SortingLayer.NameToID(tile.sortingLayer);
            RotateSpriteByOrientation(spriteRenderer, orientation);

            if (tile.animator)
            {
                var animator = newTile.AddComponent<Animator>();
                animator.runtimeAnimatorController = tile.animator.runtimeAnimatorController;
            }

            BreakableTileBehaviour tileBehaviour = tile.GetName() switch
            {
                "belt" => newTile.AddComponent<BeltTileBehaviour>(),
                _ => newTile.AddComponent<BreakableTileBehaviour>(),
            };

            tileBehaviour.orientation = orientation;
            tileBehaviour.playerInventory = playerInventory;
            tileBehaviour.item = tile.item;
            newTile.AddComponent<BoxCollider2D>();
            newTile.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
            if (!isSolid)
            {
                newTile.GetComponent<BoxCollider2D>().isTrigger = true;
            }
            tiles.Add(newTile.transform.position);

            return true;
        }

        private bool PlaceBackgroundTile(TileData tile, Vector2 position)
        {
            if (!CanPlaceOnTile(position))
                return false;
            var newTile = new GameObject(tile.GetName());
            newTile.transform.parent = transform;
            newTile.transform.position = new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
            var spriteRenderer = newTile.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = tile.topTileSprite;
            newTile.layer = LayerMask.NameToLayer("Background");
            spriteRenderer.sortingLayerName = "Background";
            spriteRenderer.sortingOrder = -1;

            return true;
        }

        private void SpawnGhostTile(ItemData item)
        {
            var ghostTile = new GameObject("GhostTile");

            var tileGhost = ghostTile.AddComponent<TileGhost>();
            tileGhost.tileData = tileRegistry.GetTileByItem(item);
            tileGhost.transform.parent = transform;

            var spriteRenderer = ghostTile.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = tileGhost.tileData.topTileSprite;
            spriteRenderer.sortingLayerID = SortingLayer.NameToID(tileGhost.tileData.sortingLayer);

            // Add opacity to the sprite
            var color = spriteRenderer.color;
            color.a = 0.5f;
            spriteRenderer.color = color;

            var boxCollider = ghostTile.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(1, 1);
            boxCollider.isTrigger = true;
        }
    }

}