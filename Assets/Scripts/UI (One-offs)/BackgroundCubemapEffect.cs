using UnityEngine;

public class FaceCameraAndCursor : MonoBehaviour
{
    public Camera mainCamera;
    public float cursorInfluence = 0.1f; // Adjust this value to control the influence of the cursor
    public float smoothSpeed = 5f; // Speed of the smooth transition

    private Quaternion initialRotation;
    private bool cursorInWindow = true;
    private Quaternion targetRotation;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        initialRotation = transform.rotation;
        targetRotation = initialRotation;
    }

    void Update()
    {
        // Check if the cursor is inside the window
        if (Input.mousePosition.x < 0 || Input.mousePosition.y < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y > Screen.height)
        {
            cursorInWindow = false;
        }
        else
        {
            cursorInWindow = true;
        }

        if (cursorInWindow)
        {
            // Make the cube face the camera
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(-directionToCamera, Vector3.up);

            // Get the cursor position in world space
            Vector3 cursorScreenPosition = Input.mousePosition;
            cursorScreenPosition.z = mainCamera.transform.position.z - transform.position.z; // Ensure correct depth
            Vector3 cursorWorldPosition = mainCamera.ScreenToWorldPoint(cursorScreenPosition);

            // Calculate the influence of the cursor on the rotation
            Vector3 directionToCursor = cursorWorldPosition - transform.position;
            Quaternion cursorRotation = Quaternion.LookRotation(-directionToCursor, Vector3.up);

            // Apply the influence of the cursor on the rotation
            targetRotation = Quaternion.Slerp(lookRotation, cursorRotation, cursorInfluence);
        }
        else
        {
            // Smoothly snap back to initial rotation if cursor leaves the window
            targetRotation = initialRotation;
        }

        // Smoothly transition to the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }
}