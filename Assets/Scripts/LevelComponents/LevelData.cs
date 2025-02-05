using UnityEngine;
using System;

[Serializable]
public class LevelData
{
    public int floorCount;
    public Vector2 floorDimensions = new Vector2(40f, 40f);
    public TagGroup[] tagGroups;

    [Serializable]
    public class TagGroup
    {
        public int floorNumber;
        public string[] tags;
        public int[] prefilledIndices;
    }
}