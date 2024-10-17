using UnityEngine;

[CreateAssetMenu(fileName = "newtileatlas", menuName = "ScriptableObjects/Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    public TileClass wood;
    public TileClass stone;
    public TileClass dirt;
    public TileClass iron;
    public TileClass gold;

    public TileClass mineBackground;

    public TileClass GetTile(Item item)
    {
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
