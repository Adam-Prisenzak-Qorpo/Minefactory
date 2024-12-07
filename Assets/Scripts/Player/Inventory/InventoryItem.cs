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
        public ItemStack itemStack;

        private bool isHovered = false;

        void OnMouseDown()
        {
            // Check if the player is not in place mode
            if (gameState.interaction == Interaction.Place || itemStack == null)
            {
                return;
            }
            WorldManager.activeBaseWorld.onItemSelect(stack.item);
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
            if (Input.GetKeyDown(KeyCode.Q) && isHovered && itemStack?.item is not null)
            {
                var item = itemStack.item;
                // Remove the item from the inventory
                Inventory.useItem(item);
                // Put item on ground with prefab "Item"
                Debug.Log(itemStack);
                var prefab = item.prefab;

                if (!prefab)
                {
                    Debug.LogError($"Item prefab for {item.name} not found.");
                    return;
                }
                prefab.GetComponent<ItemBehaviour>().item = item;

                Instantiate(prefab, transform.position, Quaternion.identity);
            }
        }
    }
}