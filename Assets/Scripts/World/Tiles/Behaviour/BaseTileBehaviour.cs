using Minefactory.Common;
using Minefactory.Player.Inventory;
using Minefactory.Storage;
using Minefactory.Storage.Items;
using UnityEngine;
namespace Minefactory.World.Tiles.Behaviour
{
    public class BaseTileBehaviour : MonoBehaviour
    {
        public Inventory playerInventory;
        public ItemData item;
        public Orientation orientation = Orientation.Up;
    }
}