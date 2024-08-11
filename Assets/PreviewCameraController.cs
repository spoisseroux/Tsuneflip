using UnityEngine;
using Cinemachine;

public class PreviewCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook playerFreeLookCamera; // Reference to the player's Cinemachine FreeLook camera
    [SerializeField] private Transform previewCamera; // The preview camera transform

    private Transform cameraPivot; // The pivot point for the camera
    private Vector3 gridMidpoint;
    private float cameraDistance;

    public void SetGridMidpointAndSize(Vector3 gridMidpoint, int rows, int columns, float tileSize)
    {
        this.gridMidpoint = gridMidpoint;

        // Create a new pivot point dynamically if it doesn't exist
        if (cameraPivot == null)
        {
            cameraPivot = new GameObject("CameraPivot").transform;
        }

        // Adjust the pivot point to the correct y level (matching the grid y level + some offset if needed)
        float gridHeightOffset = 100f; // Adjust this value to properly center the grid
        cameraPivot.position = new Vector3(gridMidpoint.x, gridMidpoint.y + gridHeightOffset, gridMidpoint.z);

        // Calculate the distance based on the grid size
        float gridSize = Mathf.Max(rows, columns) * tileSize;
        cameraDistance = gridSize * 5f; // Adjust this factor to control how far the camera should be

        // Parent the camera to the pivot
        previewCamera.SetParent(cameraPivot);

        // Set the initial camera position relative to the pivot
        previewCamera.localPosition = new Vector3(0, 0, -cameraDistance);

        // Set the camera's initial rotation to face the +Z axis
        cameraPivot.rotation = Quaternion.Euler(30f, 0f, 0f); // Slightly tilted downwards to see the grid

        // Ensure the camera is facing the +Z axis
        previewCamera.localRotation = Quaternion.identity;

        // Set the camera's initial rotation
        UpdateCameraRotation();
    }

    private void Update()
    {
        // Update the camera's rotation to match the player's FreeLook camera rotation
        UpdateCameraRotation();
    }

    private void UpdateCameraRotation()
    {
        // Constrain the vertical rotation to prevent the camera from going below the pivot point
        float clampedVerticalRotation = Mathf.Clamp(playerFreeLookCamera.m_YAxis.Value * 360f, 0f, 80f);

        // Match the rotation of the player's FreeLook camera but prevent it from going below the pivot
        cameraPivot.rotation = Quaternion.Euler(clampedVerticalRotation, playerFreeLookCamera.m_XAxis.Value, 0f);
    }
}
