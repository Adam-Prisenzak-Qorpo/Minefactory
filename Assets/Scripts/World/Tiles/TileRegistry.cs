using Minefactory.Common;
using Minefactory.Storage.Items;
using UnityEngine;
namespace Minefactory.World.Tiles
{
    [CreateAssetMenu(fileName = "NewTileRegistry", menuName = "Tiles/Registry")]
    public class TileRegistry : Registry<TileData>
    {

        public TileData GetTileByItem(ItemData item)
        {
            foreach (TileData tile in list)
            {
                if (tile.item == item)
                {
                    return tile;
                }
            }
            return null;
        }
    }
}
