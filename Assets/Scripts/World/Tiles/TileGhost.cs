using System;
using Minefactory.Common;
using Minefactory.Player.Inventory;
using UnityEngine;
using Minefactory.Game;
using Minefactory.Storage;

namespace Minefactory.World.Tiles
{
    public class TileGhost : MonoBehaviour
    {
        public TileData tileData;
        public Orientation orientation = Orientation.Up;

        public Vector3 hoveredLocation;

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
            bool rotate = Input.GetKeyDown(KeyCode.R);
            if (rotate)
            {
                Rotate();
            }
            bool drop = Input.GetKeyDown(KeyCode.Q);
            if (drop)
            {
                DropItem();
            }
        }

        private void DropItem()
        {

            var item = tileData.item;
            // Remove the item from the inventory
            Inventory.useItem(item);
            // Put item on ground with prefab "Item"
            var prefab = item.prefab;

            if (!prefab)
            {
                Debug.LogError($"Item prefab for {item.name} not found.");
                return;
            }
            prefab.GetComponent<ItemBehaviour>().item = item;

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
            var sprite = GetComponent<SpriteRenderer>();
            canPlace = WorldManager.activeBaseWorld.canPlace(transform.position);
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
            if (!canPlace)
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