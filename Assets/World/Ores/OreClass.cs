using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "newOre", menuName = "ScriptableObjects/Ore Class")]
public class OreClass : ScriptableObject
{
    public TileClass tile;
    public float rarity;
    public float size;
    public int depth;
    private Texture2D noiseTexture;

    public bool CanPlace(int worldSize, int x, int y)
    {
        if (y < (worldSize - depth) && noiseTexture.GetPixel(x, y).r > 0.5f)
            return true;
        return false;
    }

    public void SetNoiseTexture(Texture2D noiseTexture)
    {
        this.noiseTexture = noiseTexture;
    }
}
