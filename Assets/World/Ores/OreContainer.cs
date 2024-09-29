using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewOreContainer", menuName = "Ore Container")]
public class OreContainer : ScriptableObject
{
    public List<OreClass> ores = new();

    private Dictionary<string, OreClass> oreDictionary;

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        oreDictionary = new Dictionary<string, OreClass>();
        foreach (var ore in ores)
        {
            if (!oreDictionary.ContainsKey(ore.tile.tileName))
            {
                oreDictionary.Add(ore.tile.tileName, ore);
            }
        }
    }

    public OreClass GetOre(string key)
    {
        if (oreDictionary == null)
            Initialize();

        oreDictionary.TryGetValue(key, out OreClass ore);
        return ore;
    }
}
