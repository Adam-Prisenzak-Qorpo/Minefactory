using Minefactory.Storage;
using Minefactory.Storage.Items;
using UnityEngine;
namespace Minefactory.World.Tiles
{
    public class TileEntityClass : MonoBehaviour
    {
        public StorageData playerInventory;
        public ItemData item;

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
            BaseWorldGeneration.onTileRemoved(transform.position);
        }
    }
}