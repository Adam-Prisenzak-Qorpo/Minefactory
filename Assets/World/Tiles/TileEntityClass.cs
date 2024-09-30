using UnityEngine;

#nullable enable

public class TileEntityClass : MonoBehaviour
{
    public Inventory playerInventory;
    public Item? item;

    void Start()
    {
        Debug.Log($"Tile Entity Class: {name}");
    }

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
