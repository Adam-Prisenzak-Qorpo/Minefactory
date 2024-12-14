using Minefactory.Common;
using Minefactory.World.Tiles;
using UnityEngine;

namespace Minefactory.World.Ores
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "newOre", menuName = "Ores/Data")]
    public class OreData : ScriptableObject, IWithName
    {
        [Header("Basic Settings")]
        public TileData tile;
        [Tooltip("How common the ore is (1.0 = common, 0.0 = very rare)")]
        [Range(0f, 1f)]
        public float rarity;
        
        [Header("Depth Settings")]
        [Tooltip("Minimum depth where ore starts appearing")]
        public int depth;
        [Tooltip("How much depth affects spawn rate (0 = no effect, 1 = maximum effect)")]
        [Range(0f, 1f)]
        public float depthInfluence = 0.2f;
        [Tooltip("Over how many blocks the depth effect is spread")]
        [Range(10f, 1000f)]
        public float depthFalloff = 50f;
        
        [Header("Pattern Settings")]
        [Tooltip("Frequency of noise pattern (lower = larger patterns, higher = smaller patterns)")]
        [Range(0.01f, 1)]
        public float frequency = 0.05f;

        [HideInInspector]
        public float worldSeed;

        public string GetName()
        {
            return tile.name;
        }
    }
}