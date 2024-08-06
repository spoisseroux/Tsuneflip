using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelX", menuName = "LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField]
    public int gridSize, tileSize;
    [SerializeField]
    public List<FlipCode> goalData;
}
