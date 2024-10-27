using UnityEngine;

[CreateAssetMenu(fileName = "new_item_data", menuName = "Items/Item data")]
public class ItemData : ScriptableObject
{
    public Sprite sprite;
    public string itemName;
}
