using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelData", menuName = "HTML Game/Level Data")]
public class LevelDataSO : ScriptableObject
{
    [System.Serializable]
    public class TagGroup
    {
        public string[] tags;
        public int[] prefilledIndices;
        public int floorNumber;
    }

    [Header("Floor Settings")]
    public int floorCount = 1;
    public float floorHeight = 10f;
    public Vector2 floorDimensions = new Vector2(40f, 40f);
    
    [Header("Level Contents")]
    public TagGroup[] tagGroups;

    public static LevelDataSO[] GetDefaultLevels()
    {
        return new LevelDataSO[]
        {
            CreateLevel1(),
            CreateLevel2(),
            //CreateLevel3(),
            //CreateLevel4(),
            //CreateLevel5()
        };
    }

    private static LevelDataSO CreateLevel1()
    {
        var level = CreateInstance<LevelDataSO>();
        level.name = "Level 1";
        level.floorCount = 1;
        level.tagGroups = new TagGroup[] 
        {
            new TagGroup {
                floorNumber = 0,
                tags = new string[] { "<h1>", "Hello World", "</h1>" },
                prefilledIndices = new int[] { 1 }
            }
        };
        return level;
    }

    private static LevelDataSO CreateLevel2()
    {
        var level = CreateInstance<LevelDataSO>();
        level.name = "Level 2";
        level.floorCount = 2;
        level.tagGroups = new TagGroup[] 
        {
            new TagGroup {
                floorNumber = 0,
                tags = new string[] { "<div>", "<p>", "Welcome", "</p>", "</div>" },
                prefilledIndices = new int[] { 2 }
            },
            new TagGroup {
                floorNumber = 1,
                tags = new string[] { "<span>", "To HTML", "</span>" },
                prefilledIndices = new int[] { 1 }
            }
        };
        return level;
    }

    // ... Implementations for Level3, Level4, Level5 follow same pattern
}