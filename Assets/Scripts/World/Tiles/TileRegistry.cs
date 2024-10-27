using UnityEngine;

[CreateAssetMenu(fileName = "newtileregistry", menuName = "Tiles/Tile Registry")]
public class TileRegistry : ScriptableObject
{
    public TileData belt;
    public TileData wood;
    public TileData stone;
    public TileData dirt;
    public TileData iron;
    public TileData gold;

    public TileData mineBackground;

    public TileData GetTile(ItemData item)
    {
        if (belt.item == item)
        {
            return belt;
        }
        if (wood.item == item)
        {
            return wood;
        }
        if (stone.item == item)
        {
            return stone;
        }
        if (dirt.item == item)
        {
            return dirt;
        }
        if (iron.item == item)
        {
            return iron;
        }
        if (gold.item == item)
        {
            return gold;
        }
        return null;
    }
}
