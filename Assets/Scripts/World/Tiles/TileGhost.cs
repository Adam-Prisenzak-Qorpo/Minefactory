using System;
using Minefactory.Common;
using Minefactory.Player.Inventory;
using UnityEngine;
using Minefactory.Game;
using Minefactory.World.Tiles.Behaviour;
using Minefactory.Storage;
using Minefactory.Storage.Items;

namespace Minefactory.World.Tiles
{
    public class TileGhost : MonoBehaviour
    {
        public TileData tileData;
        public Orientation orientation = Orientation.Up;

        public Vector3 hoveredLocation;
        public bool placeable = true;
        public ItemData itemData;

        private bool canPlace = true;

        void Update()
        {
            var mousePos = Input.mousePosition;
            var rawPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = new Vector3(Mathf.Round(rawPos.x), Mathf.Round(rawPos.y), 0);
            bool unselect = Input.GetKey(KeyCode.Escape);
            if (unselect)
            {
                Destroy(gameObject);
            }
            if (placeable)
            {
                bool rotate = Input.GetKeyDown(KeyCode.R);
                if (rotate)
                {
                    Rotate();
                }
            }
            bool drop = Input.GetKeyDown(KeyCode.Q);
            if (drop)
            {
                DropItem();
            }
        }

        private void DropItem()
        {

            // Remove the item from the inventory
            Inventory.useItem(itemData);
            // Put item on ground with prefab "Item"
            var prefab = itemData.prefab;

            if (!prefab)
            {
                Debug.LogError($"Item prefab for {itemData.name} not found.");
                return;
            }
            prefab.GetComponent<ItemBehaviour>().item = itemData;

            Instantiate(prefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        public void Rotate()
        {
            transform.Rotate(0, 0, -90);
            orientation = orientation switch
            {
                Orientation.Up => Orientation.Right,
                Orientation.Right => Orientation.Down,
                Orientation.Down => Orientation.Left,
                Orientation.Left => Orientation.Up,
                _ => throw new NotImplementedException()
            };
        }

        void OnMouseOver()
        {
            if (!placeable)
            {
                return;
            }
            var sprite = GetComponent<SpriteRenderer>();
            var behavior = GetComponent<BaseTileBehaviour>();
            canPlace = behavior.CanBePlaced(transform.position);
            var currentAlpha = sprite.color.a;
            if (!canPlace)
            {
                sprite.color = new Color(1f, 0f, 0f, currentAlpha);
            }
            else
            {
                sprite.color = new Color(1f, 1f, 1f, currentAlpha);
            }
        }

        void OnMouseDown()
        {
            if (!placeable)
            {
                return;
            }
            var placed = WorldManager.activeBaseWorld.onPlaceTile(transform.position, tileData.item, orientation);
            if (placed)
            {
                Inventory.useItem(tileData.item);
                Destroy(gameObject);
            }
        }
    }
}