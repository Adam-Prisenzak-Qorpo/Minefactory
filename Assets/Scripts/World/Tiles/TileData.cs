using UnityEngine;

[CreateAssetMenu(fileName = "newtiledata", menuName = "Tiles/Tile Data")]
public class TileData : ScriptableObject
{
    public string tileName;
    public Sprite tileSprite;
    public Sprite topTileSprite;
    public ItemData item;
}
