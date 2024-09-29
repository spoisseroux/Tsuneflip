using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    // singleton attempt??? maybe have to delet, just trying shit
    //private static GridManager instance;
    //public static GridManager Instance { get { return instance; } }

    private FMOD.Studio.EventInstance playTileFlip;
    [SerializeField] bool debugMode = false;

    // the Grid being rendered
    private GameObject[,] grid;
    // could make another object here just storing tiles? then we can avoid (row x col) number of GetComponent<Tile>() calls

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
        /*
        // singleton thing
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        */

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
    #endregion

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