using UnityEngine;

public class GridPreviewCamera : MonoBehaviour
{
    public LevelData levelData; // Reference to the LevelData ScriptableObject
    public float padding = 2f;  // Extra space around the grid
    public float rotationSpeed = 2f; // Speed of rotation

    private Camera cam;
    private float angle = 0f;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("No Camera component found on this GameObject.");
            return;
        }
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

    void SetCameraPosition()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData is not assigned!");
            return;
        }

        // Calculate the size of the grid from LevelData
        float gridWidth = levelData.columns * levelData.tileSize;
        float gridHeight = levelData.rows * levelData.tileSize;

        // Calculate the pivot point (center of the center-most tile)
        Vector3 pivotPoint = new Vector3(
            (levelData.columns - 1) * levelData.tileSize / 2f,
            0,
            (levelData.rows - 1) * levelData.tileSize / 2f
        );

        // Calculate the distance for perspective camera
        float distance = Mathf.Max(gridWidth, gridHeight) / (2f * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad)) + padding;

        // Calculate the camera position
        float height = (distance * Mathf.Tan(33f * Mathf.Deg2Rad))*1.15f;
        cam.transform.position = new Vector3((pivotPoint.x + distance)*.85f, height*.85f, pivotPoint.z); // Positioned along +x axis

        // Look at the center of the grid
        cam.transform.LookAt(pivotPoint);

        // Adjust the camera to look down at the grid
        cam.transform.Rotate(5f, 0f, 0f); // Adjust the angle as needed
    }

    void RotateCamera()
    {
        if (levelData == null)
        {
            return;
        }

        // Calculate the size of the grid from LevelData
        float gridWidth = levelData.columns * levelData.tileSize;
        float gridHeight = levelData.rows * levelData.tileSize;

        // Calculate the pivot point (center of the center-most tile)
        Vector3 pivotPoint = new Vector3(
            (levelData.columns - 1) * levelData.tileSize / 2f,
            0,
            (levelData.rows - 1) * levelData.tileSize / 2f
        );

        // Rotate the camera around the pivot point with easing
        angle += rotationSpeed * Time.deltaTime;
        float angleOffset = Mathf.Sin(angle) * 15f; // Smoothly rotate within a 30-degree range (15 degrees each side)

        float targetAngle = -90f + angleOffset;
        cam.transform.RotateAround(pivotPoint, Vector3.up, targetAngle - cam.transform.eulerAngles.y);
    }
}