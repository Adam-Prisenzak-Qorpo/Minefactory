using Minefactory.Common;
using Minefactory.Game;
using Minefactory.Storage;
using Minefactory.Storage.Items;
using Minefactory.World;
using Minefactory.World.Tiles;
using Unity.VisualScripting;
using UnityEngine;

namespace Minefactory.Player.Inventory
{
    public class InventoryItem : MonoBehaviour
    {
        public GameState gameState;
        public StorageData inventory;
        public TileRegistry tileRegistry;
        public ItemStack stack;

        private bool isHovered = false;

        void OnMouseDown()
        {
            // Check if the player is not in place mode
            if (gameState.interaction == Interaction.Place || stack == null)
            {
                return;
            }

            // Spawn a ghost tile
            BaseWorldGeneration.onItemSelect(stack.item);
        }

        void OnMouseEnter()
        {
            isHovered = true;
        }


        void OnMouseExit()
        {
            isHovered = false;
        }

        // Check if Q is pressed and the player is hovering over the item
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q) && isHovered && stack != null)
            {
                // Remove the item from the inventory
                Inventory.useItem(stack.item);
                // Put item on ground with prefab "Item"
                var prefab = stack.item.prefab;

                if (!prefab)
                {
                    Debug.LogError($"Item prefab for {stack.item.name} not found.");
                    return;
                }
                prefab.GetComponent<ItemBehaviour>().item = stack.item;

                Instantiate(prefab, transform.position, Quaternion.identity);
            }
        }
    }
}