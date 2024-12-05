using Minefactory.Common;
using UnityEditor;
using UnityEngine;

namespace Minefactory.Storage.Items
{
    [CreateAssetMenu(fileName = "new_item_data", menuName = "Items/Data")]
    public class ItemData : ScriptableObject, IWithName
    {
        public Sprite sprite;
        public GameObject prefab;
        public string itemName;

        public string GetName()
        {
            return itemName;
        }
    }
}