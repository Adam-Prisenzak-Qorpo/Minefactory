using Minefactory.Common;
using Minefactory.Storage;
using Minefactory.Storage.Items;
using UnityEngine;
namespace Minefactory.World.Tiles.Behaviour
{
    public class BaseTileBehaviour : MonoBehaviour
    {
        public StorageData playerInventory;
        public ItemData item;
        public Orientation orientation = Orientation.Up;
    }
}