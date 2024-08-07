using UnityEngine;
using System;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    // the Grid being rendered
    private GameObject[,] grid;

    // level data
    [SerializeField]
    private LevelData goal;

    // Tile creator
    [SerializeField] private GameObject tilePrefab; // for instantiating new Tiles upon level load

    // boards
    private Dictionary<Tuple<int, int>, FlipCode> goalBoard;
    //private Dictionary<Tuple<int, int>, Tile> currentBoard = new Dictionary<Tuple<int, int>, Tile>();
    // do we need a Grid object to avoid weird monobehavior initializations??? how to deal with level initializations
    // maybe keep the goal grid in another class? like a level manager or game manager?
    // current grid is just the grid[,] object?

    // sizing variables
    //private int gridSize; // square grid, one value only
    //[SerializeField]
    //private float tileSize; // tiles are constant size

    #region Monobehaviour Functions
    private void Awake()
    {
        InitializeGrid(goal);
    }
    #endregion

    // instantiate Grid
    // pass in positioning data
    public void InitializeGrid(LevelData goal)
    {
        // create Grid & GoalGrid
        grid = new GameObject[goal.gridSize, goal.gridSize];
        goalBoard = new Dictionary<Tuple<int, int>, FlipCode>();
        for (int row = 0; row < goal.gridSize; row++)
        {
            for (int col = 0; col < goal.gridSize; col++)
            {
                // set GoalBoard data
                Tuple<int, int> currentIndices = new Tuple<int, int>(row, col);
                goalBoard[currentIndices] = goal.goalData[(row * goal.gridSize) + col];

                // instantiate Tile
                grid[row, col] = Instantiate(tilePrefab,
                    new Vector3(row * (goal.tileSize /*+ 0.05f*/), 0, col * (goal.tileSize /*+ 0.05f*/)),
                    Quaternion.identity);

                // set Tile data
                grid[row, col].GetComponent<Tile>().InitializeTile(goalBoard[currentIndices]);
            }
        }
    }

    // resolve a given position in world space to a Tile in the Grid
    public Tile GetTileFromWorldSpace(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / goal.gridSize);
        int z = Mathf.FloorToInt(worldPos.z / goal.gridSize);

        x = Mathf.Clamp(x, 0, goal.gridSize);
        z = Mathf.Clamp(z, 0, goal.gridSize);

        return grid[x,z].GetComponent<Tile>();
    }
}
