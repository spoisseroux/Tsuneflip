using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelX", menuName = "LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField] public string levelName;
    [SerializeField] public int tileSize, rows, columns;
    [SerializeField] public FlipCode2DArray goalDataArray;
    [SerializeField] public WorldData associatedWorldData;
    public float bestTime = float.MaxValue;

    [HideInInspector] public Cubemap cubemap;
    [HideInInspector] public Color cubemapColor, tileColorTop, tileColorBottom;
    
    // probably need to add a respawn transform here

    private void OnValidate()
    {
        cubemap = associatedWorldData.worldCubemap;
        cubemapColor = associatedWorldData.worldCubemapColor;
        tileColorTop = associatedWorldData.tileColorTop;
        tileColorBottom = associatedWorldData.tileColorBottom;

        if (goalDataArray == null || goalDataArray.array == null || goalDataArray.rows != rows || goalDataArray.columns != columns)
        {
            goalDataArray = new FlipCode2DArray(rows, columns);
        }
        else if (goalDataArray.rows != rows || goalDataArray.columns != columns)
        {
            goalDataArray.rows = rows;
            goalDataArray.columns = columns;
            goalDataArray.array = new FlipCode[rows * columns];
        }
    }
}
