using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* would be a good idea to update this to be better suited for pathfinding and overall lightness
 * calling GetComponent<Tile>() a bunch for every pathfinding algorithm iteration is potentially costly
 * 
 * also a ton of stuff could be shoved into TileData potentially
 */


public enum FlipCode
{
    Unflipped,
    Flipped,
    Empty
}

[System.Serializable]
public class TileData
{
    [SerializeField]
    public FlipCode flipState;

    public Dictionary<FlipCode, int> enumMap = new Dictionary<FlipCode, int>{
        {FlipCode.Unflipped, 0},
        {FlipCode.Flipped, 1},
        {FlipCode.Empty, 2}
    };
}

public class Tile : MonoBehaviour
{
    // reference to the GridManager
    [SerializeField] private GridManager grid;

    // world pos for pathfinding
    public Vector3 nodePosition;

    // can we just serialize this?
    private RotateTile rotateTileScript;
    [SerializeField] private TileData tile;

    // pathfinding node data
    public bool walkable;
    public int gCost; // distance from this Tile to starting Tile
    public int hCost; // distance from this Tile to target Tile
    public int fCost {
        get { return gCost + hCost; }
    }
    public int gridX;
    public int gridY;
    public Tile parent;

    private delegate void OnFlipCompleted();
    private event OnFlipCompleted flip;

    #region Monobehavior
    public void Awake()
    {
        tile = new TileData();
        rotateTileScript = GetComponent<RotateTile>();
        if (rotateTileScript == null)
        {
            Debug.LogError("RotateTile script not found!");
        }
    }
    #endregion

    #region Initializing
    public void InitializeTile(FlipCode flip)
    {
        // set flipcode
        if (flip == FlipCode.Empty)
        {
            tile.flipState = FlipCode.Empty;
        }
        else
        {
            tile.flipState = FlipCode.Unflipped;
        }
        //Debug.Log("Initialized Tile Flip State: " + tile.flipState);
    }

    public void InitializeGameTile(FlipCode flip, GridManager gridArg, Vector3 nodePos, int x, int y)
    {
        // set Grid info
        grid = gridArg;
        nodePosition = nodePos;
        gridX = x;
        gridY = y;

        // set flipcode
        if (flip == FlipCode.Empty)
        {
            tile.flipState = FlipCode.Empty;
            walkable = false;

            // Do not render the Tile, set Collider to false
            Transform tileCollider = this.transform.Find("TileCollider");
            tileCollider.gameObject.SetActive(false);

            // add a nav mesh obstacle component
            //NavMeshObstacle emptyTile = this.gameObject.AddComponent<NavMeshObstacle>();
            //emptyTile.carving = true;
            //emptyTile.size = new Vector3(0.1f, 0.5f, 0.1f);
        }
        else
        {
            tile.flipState = FlipCode.Unflipped;
            walkable = true;
        }
        Debug.Log("Initialized Tile Flip State: " + tile.flipState);
    }
    #endregion

    #region Flip Logic
    // CALL THIS WHEN U WANNA FLIP TILE
    public void FlipTile()
    {
        // maybe it would be nice here to extend this method to determine WHICH type of unit flipped the Tile?
        // that way we only check for certain conditions based on the Entity that decided to flip the Tile
        //      1) Player::Jump -> check for game win
        //      2) NPC::UnflipTile -> no checks
        StartCoroutine(FlipTriggered());
    }


    private IEnumerator FlipTriggered()
    {
        // for safety's sake
        if (tile.flipState == FlipCode.Empty)
        {
            yield break;
        }

        // rotate tile if not already rotating
        if (!rotateTileScript.IsRotating)
        {
            //Add this flip to rank calculator
            RankCalculator.IncrementFlips();
            
            //Debug.Log("Starting Rotation...");
            yield return StartCoroutine(rotateTileScript.Rotate());

            // once rotation is complete, execute FlipEnum and invoke flip event
            FlipEnum();
            //Debug.Log("Flip State after rotation: " + tile.flipState);
            flip?.Invoke(); // invoke any events in separate scripts that respond to any tile flipping
            grid.RequestLevelWinCheck(); // grid and tiles already tightly together, maybe a redesign but for now this works
        }
        else
        {
            //Debug.Log("Tile is already rotating.");
        }
    }

    //TODO: Not entering enum???
    private void FlipEnum()
    {
        switch (tile.flipState)
        {
            case FlipCode.Empty:
                break;

            case FlipCode.Flipped:
                tile.flipState = FlipCode.Unflipped;
                break;

            case FlipCode.Unflipped:
                tile.flipState = FlipCode.Flipped;
                break;
        }
        //Debug.Log("New Flip State: " + tile.flipState);
    }


    public FlipCode GetFlipCode()
    {
        return tile.flipState;
    }
    #endregion
}