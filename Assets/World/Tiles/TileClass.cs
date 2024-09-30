using UnityEngine;

#nullable enable
[CreateAssetMenu(fileName = "newtileclass", menuName = "ScriptableObjects/Tile Class")]
public class TileClass : ScriptableObject
{
    public string tileName;
    public Sprite tileSprite;
    public Sprite topTileSprite;
    public Item? item;
}
