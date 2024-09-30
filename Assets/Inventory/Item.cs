using UnityEngine;

[CreateAssetMenu(fileName = "new_item_name", menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    public Sprite sprite;
    public string itemName;
}
