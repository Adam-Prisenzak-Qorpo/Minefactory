using Minefactory.Storage;
using Minefactory.Storage.Items;
using UnityEngine;
namespace Minefactory.World.Tiles.Behaviour
{
    public class BreakableTileBehaviour : BaseTileBehaviour
    {
        void OnMouseDown()
        {
            Debug.Log($"Clicked on {name}");
            if (item != null)
            {
                playerInventory.AddItem(item);
            }
            else
            {
                Debug.Log("No item to add");
            }
            Destroy(gameObject);
            WorldGeneration.onTileRemoved(transform.position);
        }
    }
}