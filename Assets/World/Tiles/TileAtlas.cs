using UnityEngine;

[CreateAssetMenu(fileName = "newtileatlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    public TileClass wood;
    public TileClass stone;
    public TileClass dirt;
    public TileClass iron;
    public TileClass gold;
}
