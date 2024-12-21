using Minefactory.Storage;
using Minefactory.Storage.Items;
using UnityEngine;
using Minefactory.Game;
namespace Minefactory.World.Tiles.Behaviour
{
    public class BreakableTileBehaviour : BaseTileBehaviour
    {
        public void OnMouseDown()
        {
            if (item != null)
            {
                WorldManager.activeBaseWorld.playerInventory.AddItem(item);
            }
            Destroy(gameObject);
            WorldManager.activeBaseWorld.onTileRemoved(transform.position);
        }
    }
}