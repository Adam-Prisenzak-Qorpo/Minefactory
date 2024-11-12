using System;
using Minefactory.Common;
using Minefactory.Player.Inventory;
using UnityEngine;

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
            canPlace = WorldGeneration.canPlace(transform.position);
            if (!canPlace)
            {
                sprite.color = Color.red;

            }
            else
            {
                sprite.color = Color.white;
            }
        }

        void OnMouseDown()
        {
            if (!canPlace)
            {
                return;
            }
            var placed = WorldGeneration.onPlaceTile(transform.position, tileData.item, tileData.solid, orientation);
            if (placed)
            {
                Inventory.useItem(tileData.item);
                Destroy(gameObject);
            }
        }
    }
}