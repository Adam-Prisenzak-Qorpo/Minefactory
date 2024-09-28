using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewOreContainer", menuName = "Ore Container")]
public class OreContainer : ScriptableObject
{
    [SerializeField]
    public List<OreClass> ores = new List<OreClass>();

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
