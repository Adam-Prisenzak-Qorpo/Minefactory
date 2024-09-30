using UnityEngine;

#nullable enable

public class TileEntityClass : MonoBehaviour
{
    public InventoryClass playerInventory;
    public Item? item;

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
    }
}
