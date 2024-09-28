using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    private FMOD.Studio.EventInstance playTileFlip;

    // the Grid being rendered
    private GameObject[,] grid;
    // could make another object here just storing tiles? then we can avoid (row x col) number of GetComponent<Tile>() calls

    // level data
    [SerializeField]
    private LevelData goal;

    // Tile creator
    [SerializeField] private GameObject tilePrefab; // for instantiating new Tiles upon level load

    // board for reference
    private FlipCode2DArray goalBoard;
    //private Dictionary<Tuple<int, int>, Tile> currentBoard = new Dictionary<Tuple<int, int>, Tile>();
    // do we need a Grid object to avoid weird monobehavior initializations??? how to deal with level initializations
    // maybe keep the goal grid in another class? like a level manager or game manager?
    // current grid is just the grid[,] object?

    // PlayerMovement Object for reference
    [SerializeField] PlayerMovement player;

    public delegate void HandleGridMatch();
    public event HandleGridMatch OnGridMatch;
    public List<Vector2Int> tilesCorrect = new List<Vector2Int>();
    private FlipCode[,] currentGridStates;
    public Vector2Int currCoord;

    #region Monobehaviour Functions
    private void Awake()
    {
        playTileFlip = FMODUnity.RuntimeManager.CreateInstance("event:/PlayTileFlip");
        playTileFlip.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        //InitializeGrid(); // for debug
        player.JumpEvent += RequestTileFlip;
    }

    private void OnDisable()
    {
        player.JumpEvent -= RequestTileFlip;
    }
    #endregion

    #region Gizmos
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < goal.rows; i++)
        {
            for (int j = 0; j < goal.columns; j++)
            {
                Gizmos.DrawSphere(new Vector3(i, 1, j), 0.05f);
            }
        }
    }
    */
    #endregion

    // feed Grid its necessary LevelData
    public void InitializeLevelData(LevelData readThis)
    {
        if (readThis != null)
        {
            goal = readThis;
        }
    }

    // instantiate Grid
    // pass in positioning data
    public void InitializeGrid()
    {
        currentGridStates = new FlipCode[goal.rows, goal.columns];
        // create Grid object & create + set GoalGrid object
        grid = new GameObject[goal.rows, goal.columns];
        goalBoard = goal.goalDataArray;

        // set GoalGrid data
        //goalBoard = goal.goalDataArray;

        // fill Grid with Tile prefabs
        for (int row = 0; row < goal.rows; row++)
        {
            for (int col = 0; col < goal.columns; col++)
            {
                // instantiate Tile, add offset to make position mapping work properly
                grid[row, col] = Instantiate(tilePrefab,
                    new Vector3(row * goal.tileSize, 0, col * goal.tileSize) + new Vector3(goal.tileSize * 0.5f, 0f, goal.tileSize * 0.5f),
                    Quaternion.identity);

                // set Tile data
                grid[row, col].GetComponent<Tile>().InitializeGameTile(goalBoard.GetValue(row, col), this);

                //set current grid data(intermediate grid)
                if (goalBoard.GetValue(row, col) == FlipCode.Empty) {
                    currentGridStates[row, col] = FlipCode.Empty;
                } else {
                    currentGridStates[row, col] = FlipCode.Unflipped;
                }
                
            }
        }
    }

    // pushing a flip down to a given Tile in our grid
    public void RequestTileFlip(Vector3 position)
    {
        Tile tileToFlip = GetTileFromWorldSpace(position);
        tileToFlip.FlipTile();
        playTileFlip.start();
        //playTileFlip.release();
    }

    // resolve a given position in world space to a Tile in the Grid, helper function for determining where on Grid events should happen
    private Tile GetTileFromWorldSpace(Vector3 worldPos)
    {
        // Debug.Log(worldPos);
        int x = Mathf.FloorToInt(worldPos.x / goal.tileSize);
        int z = Mathf.FloorToInt(worldPos.z / goal.tileSize);

        x = Mathf.Clamp(x, 0, goal.rows - 1);
        z = Mathf.Clamp(z, 0, goal.columns - 1);

        //Debug.Log(x + " " + z);
        return grid[x, z].GetComponent<Tile>();
    }

    // TODO:
    // 1) A method to check current grid against the goal grid
    // 2) A method of determing WHEN to do checks
    // 3) A way of communicating when a level has been finished
    //      - possible error situation when player flips the last needed tile but dies right after jumping 
    //      - in this case we need a way to interrupt the tile flip OR the board check and proceed to Death sequence
    // 4) Either a barrier around empty tiles and side of level OR a fall zone & respawn zone

    // Check for whether each Tile's flipcode is the same between current Grid and goal Grid

    private Vector2Int updateCurrCoord() {
        currCoord = new Vector2Int(int.MinValue, int.MinValue);
        for (int i = 0; i < goal.rows; i++)
        {
            for (int j = 0; j < goal.columns; j++)
            {
                //get coord we are currently at 
                if (grid[i, j].GetComponent<Tile>().GetFlipCode() != currentGridStates[i, j]) {
                    currCoord = new Vector2Int(i, j);
                    Debug.Log("Added correct tile: " + currCoord);

                    //if its already in correct list, remove because it was unflipped
                    if (tilesCorrect.Contains(currCoord)){
                        tilesCorrect.Remove(currCoord);
                        Debug.Log("Removed correct tile: " + currCoord);
                    }
                    //check if tiled flipped was correct
                    if (grid[i, j].GetComponent<Tile>().GetFlipCode() == goalBoard.GetValue(i, j) && grid[i, j].GetComponent<Tile>().GetFlipCode() == FlipCode.Flipped){
                        tilesCorrect.Add(new Vector2Int(i, j));
                    }
                    Debug.Log("Tiles correct: " + tilesCorrect.Count);
                    currentGridStates[i, j] = grid[i, j].GetComponent<Tile>().GetFlipCode();
                }
            }
        }
        return currCoord;
    }

    private bool CheckForMatchingGrids()
    {
        for (int i = 0; i < goal.rows; i++)
        {
            for (int j = 0; j < goal.columns; j++)
            {
                if (grid[i, j].GetComponent<Tile>().GetFlipCode() != goalBoard.GetValue(i, j))
                {
                    return false;
                }
            }
        }
        return true;
    }


    // Check whether the current grid matches the goal grid, and then hand off this info to the current LevelManager
    public void RequestLevelWinCheck()
    {
        updateCurrCoord();
        bool win = CheckForMatchingGrids();
        if (win)
        {
            //Debug.Log("Win!");
            OnGridMatch?.Invoke();
        }
    }
}