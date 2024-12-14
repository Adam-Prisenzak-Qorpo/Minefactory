using UnityEngine;
using System;
using Minefactory.Common;

namespace Minefactory.Save
{
    [Serializable]
    public struct TileModification
    {
        public string tileDataName;
        public Orientation orientation;

        public TileModification(string name, Orientation orient)
        {
            tileDataName = name;
            orientation = orient;
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
        public System.Collections.Generic.List<PositionModification> modifications = new System.Collections.Generic.List<PositionModification>();

        public ChunkData(string key)
        {
            chunkKey = key;
        }
    }
}