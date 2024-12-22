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


        void OnMouseDown()
        {
            // Check if the player is not in place mode
            if (gameState.interaction == Interaction.Place || itemStack == null)
            {
                return;
            }

            // Spawn a ghost tile
            WorldManager.activeBaseWorld.onItemSelect(itemStack.item);
        }

    }
}