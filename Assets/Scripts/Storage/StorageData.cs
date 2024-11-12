using System.Collections.Generic;
using Minefactory.Storage.Items;
using UnityEngine;

namespace Minefactory.Storage
{
    [CreateAssetMenu(fileName = "StorageData", menuName = "Storage/Storage Data")]
    public class StorageData : ScriptableObject
    {
        public List<ItemStack> items = new();
        public int maxItems = 20;


        public void AddItem(ItemData item)
        {
            if (items.Count < maxItems)
            {
                var stack = items.Find(i => i.item == item);
                if (stack == null)
                {
                    stack = new ItemStack(item, 0);
                    items.Add(stack);
                }
                stack.amount++;
            }
        }

        public int RemoveItem(ItemData item)
        {
            var stack = items.Find(i => i.item == item);
            if (stack != null)
            {
                stack.amount--;
                if (stack.amount <= 0)
                {
                    items.Remove(stack);
                }
            }
            return stack.amount;
        }

        public ItemStack GetItemStack(int index)
        {
            return items.Count > index ? items[index] : null;
        }
    }
}
