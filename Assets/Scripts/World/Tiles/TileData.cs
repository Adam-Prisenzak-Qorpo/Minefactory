using Minefactory.Common;
using Minefactory.Storage.Items;
using UnityEngine;

namespace Minefactory.World.Tiles
{
    [CreateAssetMenu(fileName = "newtiledata", menuName = "Tiles/Data")]
    public class TileData : ScriptableObject, IWithName
    {
        public string tileName;
        public Sprite tileSprite;
        public Sprite topTileSprite;
        public ItemData item;

        public string GetName()
        {
            return tileName;
        }
    }
}
