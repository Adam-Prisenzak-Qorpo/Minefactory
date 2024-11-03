using Minefactory.Common;
using Minefactory.Storage.Items;
using UnityEngine;

namespace Minefactory.World.Tiles
{
    [CreateAssetMenu(fileName = "newtiledata", menuName = "Tiles/Data")]
    public class TileData : ScriptableObject, IWithName
    {
        public string tileName;
        public bool solid = true;
        public string sortingLayer = "Default";
        public Sprite tileSprite;
        public Animator animator;
        public Sprite topTileSprite;
        public ItemData item;

        public string GetName()
        {
            return tileName;
        }
    }
}
