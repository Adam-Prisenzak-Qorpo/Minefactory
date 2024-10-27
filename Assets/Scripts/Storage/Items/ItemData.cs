using UnityEngine;

[CreateAssetMenu(fileName = "new_item_data", menuName = "Items/Data")]
public class ItemData : ScriptableObject, IWithName
{
    public Sprite sprite;
    public string itemName;

    public string GetName()
    {
        return itemName;
    }
}
