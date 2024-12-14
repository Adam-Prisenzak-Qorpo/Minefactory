using System.Collections.Generic;
using Minefactory.Storage.Items;
using UnityEngine;

namespace Minefactory.Storage
{
    [CreateAssetMenu(fileName = "StorageData", menuName = "Storage/Storage Data")]
    public class StorageData : ScriptableObject
    {
        public int maxItems = 20;
        private List<ItemStack> storageItems = new();


        public void AddItem(ItemData item)
        {
            if (storageItems.Count >= maxItems) return;
            var stack = storageItems.Find(i => i.item == item);
            if (stack == null)
            {
                stack = new ItemStack(item, 0);
                storageItems.Add(stack);
                Debug.Log("Added new item: " + item.itemName);
            }
            stack.amount++;
        }

        public int RemoveItem(ItemData item)
        {
            var stack = storageItems.Find(i => i.item == item);
            if (stack == null) return 0;
            stack.amount--;
            if (stack.amount <= 0)
            {
                storageItems.Remove(stack);
            }

            return stack.amount;
        }

        public ItemStack GetItemStack(int index)
        {
            return storageItems.Count > index ? storageItems[index] : null;
        }
    }
}