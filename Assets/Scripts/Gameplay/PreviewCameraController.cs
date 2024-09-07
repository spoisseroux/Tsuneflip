using UnityEngine;
using Cinemachine;

public class PreviewCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook playerFreeLookCamera; // Reference to the player's Cinemachine FreeLook camera
    [SerializeField] private Transform previewCamera; // The preview camera transform
    [SerializeField] private float minZoomDistance = 10f; // Minimum distance the camera can zoom in
    [SerializeField] private float maxZoomDistance = 200f; // Maximum distance the camera can zoom out (reduced from 1000f)
    [SerializeField] private float zoomSpeed = 10f; // Speed at which the camera zooms in/out

    public Transform playerMinimapLocation;
    private float targetCameraDistance; // The target distance for the camera
    private float zoomVelocity = 0f; // Used for SmoothDamp

    private Transform cameraPivot; // The pivot point for the camera
    private Vector3 gridMidpoint;
    private float cameraDistance;

    private void Start()
    {
        //init cam to be 80 percent zoomed out
        targetCameraDistance = Mathf.Lerp(maxZoomDistance, minZoomDistance, 0.2f);
        cameraDistance = targetCameraDistance;
    }

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
        cameraDistance = gridSize * 2f; // Adjust this factor to control how far the camera should be
        cameraDistance = Mathf.Clamp(cameraDistance, minZoomDistance, maxZoomDistance);

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

        // Handle camera zoom
        HandleZoom();
    }

    private void UpdateCameraRotation()
    {
        // Interpolate between the grid midpoint and the player's minimap location based on the inverse zoom level
        float zoomFactor = Mathf.InverseLerp(minZoomDistance, maxZoomDistance, cameraDistance);

        // Flip the interpolation by subtracting the zoomFactor from 1
        Vector3 adjustedPlayerMinimapLocation = new Vector3(playerMinimapLocation.position.x, gridMidpoint.y, playerMinimapLocation.position.z);
        Vector3 focusPoint = Vector3.Lerp(adjustedPlayerMinimapLocation, gridMidpoint, zoomFactor);

        // Constrain the vertical rotation to prevent the camera from going below the pivot point
        float clampedVerticalRotation = Mathf.Clamp(playerFreeLookCamera.m_YAxis.Value * 360f, 0f, 80f);

        // Match the rotation of the player's FreeLook camera but prevent it from going below the pivot
        cameraPivot.rotation = Quaternion.Euler(clampedVerticalRotation, playerFreeLookCamera.m_XAxis.Value, 0f);

        // Update the camera pivot to focus on the interpolated focus point
        cameraPivot.position = new Vector3(focusPoint.x, gridMidpoint.y + 100f, focusPoint.z); // Adjust the y-offset if needed
    }

    private void HandleZoom()
    {
        // Get the scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Adjust the target camera distance based on the scroll input
        targetCameraDistance -= scrollInput * zoomSpeed;
        targetCameraDistance = Mathf.Clamp(targetCameraDistance, minZoomDistance, maxZoomDistance);

        // Smoothly transition to the target camera distance using SmoothDamp
        cameraDistance = Mathf.SmoothDamp(cameraDistance, targetCameraDistance, ref zoomVelocity, 0.2f);

        // Alternatively, you could use Lerp for a different easing effect:
        // cameraDistance = Mathf.Lerp(cameraDistance, targetCameraDistance, Time.deltaTime * zoomSpeed);

        // Update the camera's position
        previewCamera.localPosition = new Vector3(0, 0, -cameraDistance);
    }
}
