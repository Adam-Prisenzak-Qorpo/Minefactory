using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Minefactory.Common;

namespace Minefactory.Save
{
    [Serializable]
    public struct MetadataEntry
    {
        public string key;
        public string value;

        public MetadataEntry(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }

    [Serializable]
    public struct TileModification
    {
        public string tileDataName;
        public Orientation orientation;
        public List<MetadataEntry> metadataList;

        public Dictionary<string, string> GetMetadata()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (metadataList != null)
            {
                foreach (var entry in metadataList)
                {
                    dict[entry.key] = entry.value;
                }
            }
            return dict;
        }

        public void SetMetadata(Dictionary<string, string> metadata)
        {
            if (metadata == null)
            {
                metadataList = new List<MetadataEntry>();
                return;
            }

            metadataList = metadata.Select(kvp => new MetadataEntry(kvp.Key, kvp.Value)).ToList();
        }

        public TileModification(string name, Orientation orient)
        {
            tileDataName = name;
            orientation = orient;
            metadataList = new List<MetadataEntry>();
        }

        public TileModification(string name, Orientation orient, Dictionary<string, string> metadata)
        {
            tileDataName = name;
            orientation = orient;
            metadataList = new List<MetadataEntry>();
            SetMetadata(metadata);
        }
    }

    [Serializable]
    public struct PositionModification
    {
        public float x;
        public float y;
        public TileModification tile;

        public PositionModification(Vector2 pos, TileModification tile)
        {
            x = pos.x;
            y = pos.y;
            this.tile = tile;
        }

        public Vector2 Position => new Vector2(x, y);
    }

    [Serializable]
    public class ChunkData
    {
        public string chunkKey;
        public List<PositionModification> modifications = new List<PositionModification>();

        public ChunkData(string key)
        {
            chunkKey = key;
        }
    }
}