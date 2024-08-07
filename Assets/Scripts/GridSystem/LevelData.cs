using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelX", menuName = "LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField] public string levelName;
    private float bestTime;
    [SerializeField] public int tileSize;
    [SerializeField] public int rows, columns;
    [SerializeField] public FlipCode2DArray goalDataArray;

    private void OnValidate()
    {
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
