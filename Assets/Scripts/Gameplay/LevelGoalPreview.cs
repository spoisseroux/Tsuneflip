using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalPreview : MonoBehaviour
{
    // the Grid being rendered
    private GameObject[,] grid;

    // level data
    [SerializeField]
    public LevelData goal;

    // Tile creator
    [SerializeField] private GameObject tilePrefab; // for instantiating new Tiles upon level load

    // boards
    private FlipCode2DArray goalBoard;

    public void InitializeLevelGridPreview(LevelData goal, PreviewCameraController previewCameraController)
    {
        DestroyGrid();

        // Create Grid object & create + set GoalGrid object
        grid = new GameObject[goal.rows, goal.columns];
        goalBoard = goal.goalDataArray;

        // Calculate grid midpoint at the correct Y level (100)
        Vector3 gridMidpoint = new Vector3((goal.rows - 1) * goal.tileSize / 2f, 100, (goal.columns - 1) * goal.tileSize / 2f);

        // Set up the camera with the grid midpoint and size
        previewCameraController.SetGridMidpointAndSize(gridMidpoint, goal.rows, goal.columns, goal.tileSize);

        // Fill Grid with Tile prefabs
        for (int row = 0; row < goal.rows; row++)
        {
            for (int col = 0; col < goal.columns; col++)
            {
                // Instantiate Tile at y = 100
                GameObject tile = Instantiate(tilePrefab,
                    new Vector3(row * (goal.tileSize), 200, col * (goal.tileSize)),
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

    // resolve a given position in world space to a Tile in the Grid
    public Tile GetTileFromWorldSpace(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / goal.tileSize);
        int z = Mathf.FloorToInt(worldPos.z / goal.tileSize);

        x = Mathf.Clamp(x, 0, goal.rows - 1);
        z = Mathf.Clamp(z, 0, goal.columns - 1);

        return grid[x, z].GetComponent<Tile>();
    }
}
