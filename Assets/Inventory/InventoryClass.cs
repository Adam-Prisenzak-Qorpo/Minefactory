using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObjects/Inventory")]
public class InventoryClass : ScriptableObject
{
    public List<ItemStack> items = new();
    public int maxItems = 20;

    public void AddItem(Item item)
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
            Inventory.onItemAdded();
        }
    }

    public void RemoveItem(Item item)
    {
        var stack = items.Find(i => i.item == item);
        if (stack != null)
        {
            stack.amount--;
            if (stack.amount <= 0)
            {
                items.Remove(stack);
            }
            Inventory.onItemAdded();
        }
    }

    public ItemStack GetItemStack(int index)
    {
        return items.Count > index ? items[index] : null;
    }
}
