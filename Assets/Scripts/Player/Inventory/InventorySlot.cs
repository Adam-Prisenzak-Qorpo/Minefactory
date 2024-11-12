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
        public int slotIndex;
        public TileRegistry tileRegistry;

        public void UpdateItem()
        {
            var itemObject = transform.Find("item");
            var itemRenderer = itemObject.GetComponent<SpriteRenderer>();
            var itemScript = itemObject.GetComponent<InventoryItem>();
            var text = transform.Find("text").GetComponent<TextMesh>();

            itemScript.stack = inventory.GetItemStack(slotIndex);
            if (itemScript.stack == null)
            {
                itemRenderer.sprite = null;
                text.text = "";
                return;
            }

            itemRenderer.sprite = itemScript.stack.item.sprite;
            text.text = itemScript.stack.amount.ToString();
        }
    }
}