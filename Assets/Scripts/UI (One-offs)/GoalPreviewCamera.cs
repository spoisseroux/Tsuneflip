using UnityEngine;

public class GoalPreviewCamera : MonoBehaviour
{
    public LevelData levelData; // Reference to the LevelData ScriptableObject
    public GameObject[,] grid;  // The grid passed from GoalPreviewInstancer
    public float padding = 2f;  // Extra space around the grid
    public float rotationSpeed = 2f; // Speed of rotation
    public Color gizmoColor = Color.green; // Color of the gizmo

    private Camera cam;
    private float angle = 0f;
    private Vector3 pivotPoint;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0);  // Transparent background
        SetCameraPosition();
    }

    void Update()
    {
        if (cam != null)
        {
            RotateCamera();
        }
    }

    public void SetCameraPosition()
    {
        // Ensure the camera and grid are assigned
        if (cam == null)
        {
            Debug.Log("Preview camera is not assigned or delayed!");
            return;
        }

        if (levelData == null)
        {
            Debug.LogError("LevelData is not assigned!");
            return;
        }

        if (grid == null)
        {
            Debug.LogError("Grid is not assigned!");
            return;
        }

        // Calculate the bounds of the grid based on the actual positions of the tiles
        Vector3 minBound = new Vector3(float.MaxValue, 0, float.MaxValue);
        Vector3 maxBound = new Vector3(float.MinValue, 0, float.MinValue);

        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if (grid[row, col] != null)
                {
                    Vector3 tilePos = grid[row, col].transform.position;

                    minBound.x = Mathf.Min(minBound.x, tilePos.x);
                    minBound.z = Mathf.Min(minBound.z, tilePos.z);

                    maxBound.x = Mathf.Max(maxBound.x, tilePos.x);
                    maxBound.z = Mathf.Max(maxBound.z, tilePos.z);
                }
            }
        }

        // Calculate the pivot point (center of the grid)
        pivotPoint = new Vector3(
            (minBound.x + maxBound.x) / 2f,
            0,
            (minBound.z + maxBound.z) / 2f
        );

        // Calculate the grid size for camera distance
        float gridWidth = maxBound.x - minBound.x;
        float gridHeight = maxBound.z - minBound.z;

        // Calculate the distance based on the larger dimension of the grid
        float distance = Mathf.Max(gridWidth, gridHeight) / (2f * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad)) + padding;

        // Calculate the camera position
        float height = distance * Mathf.Tan(33f * Mathf.Deg2Rad);
        cam.transform.position = new Vector3(pivotPoint.x, height, pivotPoint.z - distance);

        // Make the camera look at the center of the grid
        cam.transform.LookAt(pivotPoint);

        // Adjust the camera to look down at the grid
        cam.transform.Rotate(5f, 0f, 0f);
    }

    void RotateCamera()
    {
        if (levelData == null)
        {
            return;
        }

        // Rotate the camera around the pivot point with easing
        angle += rotationSpeed * Time.deltaTime;
        float angleOffset = Mathf.Sin(angle) * 15f; // Smoothly rotate within a 30-degree range (15 degrees each side)

        float targetAngle = angleOffset;
        cam.transform.RotateAround(pivotPoint, Vector3.up, targetAngle - cam.transform.eulerAngles.y);
    }

    void OnDrawGizmos()
    {
        if (levelData == null)
        {
            return;
        }

        // Calculate the pivot point (exact center of the grid) for the gizmo
        Vector3 gizmoPivotPoint = new Vector3(
            (levelData.columns - 1) * levelData.tileSize / 2f,
            0,
            (levelData.rows - 1) * levelData.tileSize / 2f
        );

        // Draw a sphere at the pivot point
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(gizmoPivotPoint, 0.2f);
    }
}