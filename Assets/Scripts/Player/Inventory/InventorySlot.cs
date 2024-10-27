using Minefactory.Common;
using Minefactory.Storage;
using Minefactory.Storage.Items;
using Minefactory.World;
using Minefactory.World.Tiles;
using Unity.VisualScripting;
using UnityEngine;

namespace Minefactory.Player.Inventory
{
    public class InventorySlot : MonoBehaviour
    {
        public StorageData inventory;
        public TileRegistry tileRegistry;
        public ItemStack stack;
        public Orientation orientation = Orientation.Up;
        private bool selected = false;
        private bool canPlace = true;

        // Start is called before the first frame update
        void Start()
        {
            var itemRenderer = this.AddComponent<SpriteRenderer>();
            itemRenderer.sprite = stack.item.sprite;
            itemRenderer.sortingOrder = 2;

            // Add text
            var textObject = new GameObject("Text");
            textObject.transform.parent = transform;
            textObject.transform.localScale = new Vector2(0.1f, 0.1f);
            textObject.transform.localPosition = new Vector2(-0.25f, 0.25f);

            var text = textObject.AddComponent<TextMesh>();
            text.text = stack.amount.ToString();
            text.fontSize = 100;
            text.characterSize = 0.3f;
            text.anchor = TextAnchor.MiddleCenter;
            text.color = Color.white;

            var textRenderer = textObject.GetComponent<Renderer>();
            textRenderer.sortingOrder = 3;

            var collider = this.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1, 1);
            collider.isTrigger = true;
        }

        void Update()
        {
            if (selected)
            {
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                bool unselect = Input.GetKey(KeyCode.Escape);
                if (unselect)
                {
                    transform.localPosition = Vector2.zero;
                    selected = false;
                }
            }
        }

        Vector2 TransformedPosition => transform.position - new Vector3(.5f, -.5f);

        void Rotate()
        {
            transform.Rotate(0, 0, -90);
            orientation = orientation switch
            {
                Orientation.Up => Orientation.Right,
                Orientation.Right => Orientation.Down,
                Orientation.Down => Orientation.Left,
                Orientation.Left => Orientation.Up,
                _ => throw new System.NotImplementedException()
            };
        }

        void OnMouseOver()
        {
            if (selected)
            {
                var sprite = GetComponent<SpriteRenderer>();
                if (!WorldGeneration.canPlace(TransformedPosition))
                {
                    sprite.color = Color.red;
                    canPlace = false;
                }
                else
                {
                    sprite.color = Color.white;
                    canPlace = true;
                }

                // Check if pressing 'R' to rotate
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Rotate();
                }
            }
        }

        void OnMouseDown()
        {
            // Bind position on mouse
            if (!selected)
            {
                selected = true;

                // Change sprite to tile sprite
                var tile = tileRegistry.GetTileByItem(stack.item);
                if (tile)
                {
                    GetComponent<SpriteRenderer>().sprite = tile.topTileSprite;
                }
            }
            else if (canPlace)
            {
                var tile = tileRegistry.GetTileByItem(stack.item);
                var didPlace = WorldGeneration.onTilePlaced(TransformedPosition, stack.item, tile.solid, orientation);
                if (!didPlace)
                {
                    return;
                }
                var amount = inventory.RemoveItem(stack.item);
                if (amount <= 0)
                {
                    Destroy(gameObject);
                    return;
                }
                selected = false;
                orientation = Orientation.Up;
                transform.rotation = Quaternion.identity;
                GetComponent<SpriteRenderer>().sprite = stack.item.sprite;

            }
        }
    }
}