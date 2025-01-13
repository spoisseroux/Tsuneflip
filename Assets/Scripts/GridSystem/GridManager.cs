using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    private FMOD.Studio.EventInstance playTileFlip;
    [SerializeField] bool debugMode = false;

    // the Grid being rendered
    private GameObject[,] grid; // can use this directly for pathfinding i think

    // level data
    [SerializeField]
    private LevelData goal;

    // Tile creator
    [SerializeField] private GameObject tilePrefab; // for instantiating new Tiles upon level load

    // goal board for reference
    private FlipCode2DArray goalBoard;

    // PlayerMovement Object for reference
    [SerializeField] PlayerMovement player;

    // List of NPCs to listen to
    [SerializeField] List<NPCMovement> npcList;

    [SerializeField] public Transform startPos;


    /*
     * find a way to use the open stackoverflow articles to poll NPC tile flips
     * maybe we can combine a tile flip event into single composite callback
     * pass the source object (player, npc...) thru up to the gridmanager and react accordingly (check win, sound file, etc.)
     */

    public delegate void HandleGridMatch();
    public event HandleGridMatch OnGridMatch;
    public List<Vector2Int> tilesCorrect = new List<Vector2Int>();
    private FlipCode[,] currentGridStates;
    public Vector2Int currCoord;

    #region Monobehaviour Functions
    private void Awake()
    {
        if (!debugMode)
        {
            playTileFlip = FMODUnity.RuntimeManager.CreateInstance("event:/PlayTileFlip");
            playTileFlip.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        }

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
    public List<Tile> path;
    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position, new Vector3(goal.rows, 1, goal.columns));
        if (grid != null)
        {
            for (int row = 0; row < goal.rows; row++)
            {
                for (int col = 0; col < goal.columns; col++)
                {
                    Tile t = grid[row, col].GetComponent<Tile>();
                    Gizmos.color = (t.walkable) ? Color.white : Color.red;
                    if (path != null)
                    {
                        if (path.Contains(t))
                        {
                            Gizmos.color = Color.black;
                        }
                    }
                    Gizmos.DrawCube(t.worldPosition, new Vector3(1.0f, 0.1f, 1.0f) * goal.tileSize);
                }
            }
        }
        
    }
    */
    #endregion

    #region Initializing Grid
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
                Vector3 tilePosition = new Vector3(row * goal.tileSize, 0, col * goal.tileSize) +
                    new Vector3(goal.tileSize * 0.5f, 0f, goal.tileSize * 0.5f);
                grid[row, col] = Instantiate(tilePrefab, tilePosition, Quaternion.identity);

                // set Tile data, ADD +1 to the tilePosition in order to make pathfinding work?
                grid[row, col].GetComponent<Tile>().InitializeGameTile(goalBoard.GetValue(row, col), this, tilePosition + Vector3.up, row, col);

                //set current grid data(intermediate grid)
                if (goalBoard.GetValue(row, col) == FlipCode.Empty) {
                    currentGridStates[row, col] = FlipCode.Empty;
                } else {
                    currentGridStates[row, col] = FlipCode.Unflipped;
                }
                
            }
        }
    }
    #endregion

    #region Flip Tile
    // pushing a flip down to a given Tile in our grid
    public void RequestTileFlip(Vector3 position, EntityMovement sourceEntity)
    {
        // use source entity to decide sound
        Debug.Log("flip requested!");

        Tile tileToFlip = GetTileFromWorldSpace(position);
        tileToFlip.FlipTile();
        // play sound
        if (!debugMode)
        {
            playTileFlip.start();
        }
        //playTileFlip.release();
    }
    #endregion

    #region Helper functions for Tiles & Grid
    // helper function to resolve a given position in world space to a Tile in the Grid
    public Tile GetTileFromWorldSpace(Vector3 worldPos)
    {
        // Debug.Log(worldPos);
        int x = Mathf.FloorToInt(worldPos.x / goal.tileSize);
        int z = Mathf.FloorToInt(worldPos.z / goal.tileSize);

        x = Mathf.Clamp(x, 0, goal.rows - 1);
        z = Mathf.Clamp(z, 0, goal.columns - 1);

        //Debug.Log(x + " " + z);
        return grid[x, z].GetComponent<Tile>();
    }

    // helper function to get all neighboring Tiles relative to a given Tile
    public List<Tile> GetNeighboringTiles(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // skip given node
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = tile.gridX + x; // need to add x component to Tile
                int checkY = tile.gridY + y; // need to add y component to Tile

                // are we inside the grid
                if (checkX >= 0 && checkX < goal.rows && checkY >= 0 && checkY < goal.columns)
                {
                    // having this getcomponent every time kinda sucks.... refactor probably needed
                    neighbors.Add(grid[checkX, checkY].GetComponent<Tile>());
                }
            }
        }

        return neighbors;
    }

    // return the grid to outside objects
    public GameObject[,] GetGrid()
    {
        return grid;
    }
    #endregion

    #region Win Condition
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
    #endregion
}