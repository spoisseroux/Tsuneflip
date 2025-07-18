using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPreviewInstancer : MonoBehaviour
{
    // The Grid being rendered
    public GameObject[,] grid; // Expose this as public

    // Level data
    [SerializeField] public LevelData goal;

    public GoalPreviewCamera previewCam;

    // Tile creator
    [SerializeField] private GameObject tilePrefab; // For instantiating new Tiles upon level load

    // Boards
    private FlipCode2DArray goalBoard;

    #region Monobehaviour Functions
    private void Awake()
    {
        InitializeGridPreview(goal);
    }
    #endregion

    public void InitializeGridPreview(LevelData goal)
    {
        DestroyGrid();
        
        // Create Grid object & create + set GoalGrid object
        grid = new GameObject[goal.rows, goal.columns];
        goalBoard = goal.goalDataArray;

        // Set the material colors
        Material tileMaterial = tilePrefab.GetComponentInChildren<Renderer>().sharedMaterial;
        tileMaterial.SetColor("_Color1", goal.tileColorBottom);
        tileMaterial.SetColor("_Color2", goal.tileColorTop);

        // Fill Grid with Tile prefabs
        for (int row = 0; row < goal.rows; row++)
        {
            for (int col = 0; col < goal.columns; col++)
            {
                // Instantiate Tile
                GameObject tile = Instantiate(tilePrefab,
                    new Vector3(row * (goal.tileSize /*+ 0.05f*/), 0, col * (goal.tileSize /*+ 0.05f*/)),
                    Quaternion.identity);

                // Set Tile data
                FlipCode state = goalBoard.GetValue(row, col);
                Transform tileMesh = tile.transform.Find("TileCollider/TileMesh");

                if (state == FlipCode.Empty)
                {
                    // Do not render the Tile
                    tileMesh.gameObject.SetActive(false);
                }
                else
                {
                    // Render the Tile
                    tileMesh.gameObject.SetActive(true);

                    if (state == FlipCode.Flipped)
                    {
                        // Rotate the TileMesh 180 degrees on the x-axis
                        tileMesh.rotation = Quaternion.Euler(180, 0, 0);
                    }
                    else
                    {
                        // Set the TileMesh rotation to 0 degrees
                        tileMesh.rotation = Quaternion.Euler(0, 0, 0);
                    }
                }

                // Initialize the Tile with the goalBoard value
                tile.GetComponent<Tile>().InitializeTile(goalBoard.GetValue(row, col));

                // Add the Tile to the grid
                grid[row, col] = tile;
            }
        }
        
        // Set camera position after grid is fully instantiated
        previewCam.grid = grid; // Pass the grid to the camera
        previewCam.SetCameraPosition();
    }

    // Destroy the current grid of tiles
    public void DestroyGrid()
    {
        if (grid != null)
        {
            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    if (grid[row, col] != null)
                    {
                        Destroy(grid[row, col]);
                    }
                }
            }
        }
    }

    // Resolve a given position in world space to a Tile in the Grid
    public Tile GetTileFromWorldSpace(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / goal.tileSize);
        int z = Mathf.FloorToInt(worldPos.z / goal.tileSize);

        x = Mathf.Clamp(x, 0, goal.rows - 1);
        z = Mathf.Clamp(z, 0, goal.columns - 1);

        return grid[x, z].GetComponent<Tile>();
    }
}