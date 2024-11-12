using Minefactory.Common;
using Minefactory.Storage.Items;
using UnityEngine;

namespace Minefactory.World.Tiles
{
    [CreateAssetMenu(fileName = "newtiledata", menuName = "Tiles/Data")]
    public class TileData : ScriptableObject, IWithName
    {
        public string tileName;
        public string sortingLayer = "Default";
        public  GameObject underGroundTilePrefab;
        public GameObject topTilePrefab;
        public ItemData item;

        public string GetName()
        {
            return tileName;
        }
    }
}
