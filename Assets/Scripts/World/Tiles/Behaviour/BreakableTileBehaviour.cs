using Minefactory.Storage;
using Minefactory.Storage.Items;
using UnityEngine;
namespace Minefactory.World.Tiles.Behaviour
{
    public class BreakableTileBehaviour : BaseTileBehaviour
    {
        void OnMouseDown()
        {
            if (item != null)
            {
                playerInventory.AddItem(item);
            }
            Destroy(gameObject);
            WorldGeneration.onTileRemoved(transform.position);
        }
    }
}