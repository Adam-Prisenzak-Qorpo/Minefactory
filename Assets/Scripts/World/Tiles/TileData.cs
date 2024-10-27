using UnityEngine;

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
