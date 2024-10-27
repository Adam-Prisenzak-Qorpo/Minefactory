using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewOreRegistry", menuName = "Ores/Ore Registry")]
public class OreRegistry : ScriptableObject
{
    public List<OreData> ores = new();

    private Dictionary<string, OreData> oreDictionary;

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        oreDictionary = new Dictionary<string, OreData>();
        foreach (var ore in ores)
        {
            if (!oreDictionary.ContainsKey(ore.tile.tileName))
            {
                oreDictionary.Add(ore.tile.tileName, ore);
            }
        }
    }

    public OreData GetOre(string key)
    {
        if (oreDictionary == null)
            Initialize();

        oreDictionary.TryGetValue(key, out OreData ore);
        return ore;
    }
}
