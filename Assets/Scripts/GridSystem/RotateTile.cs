using UnityEngine;
using System.Collections;

public class RotateTile : MonoBehaviour
{
    private Transform tileMeshTransform;
    private float rotationPerSecond_c = 180f / 0.5f;  // 180 degrees over 0.5 seconds
    private bool isRotating = false;

    void Start()
    {
        // Find the TileMesh child object
        tileMeshTransform = transform.Find("TileCollider/TileMesh");
        if (tileMeshTransform == null)
        {
            Debug.LogError("TileMesh not found!");
        }
    }

    public IEnumerator Rotate()
    {
        isRotating = true;
        float amountRotated = 0f;
        while (amountRotated < 180f)
        {
            float frameRotation = rotationPerSecond_c * Time.deltaTime;  // Amount to rotate this frame
            tileMeshTransform.Rotate(frameRotation, 0, 0);  // Apply the rotation
            amountRotated += frameRotation;  // Keep track of the amount rotated so far
            yield return new WaitForEndOfFrame();  // Rotate every frame until we reach the target
        }

        // Snap to nearest valid rotation (0, 180, or 360 degrees)
        float currentXRotation = tileMeshTransform.eulerAngles.x;
        float snappedRotation = Mathf.Round(currentXRotation / 180f) * 180f;

        // Create a new Quaternion with the snapped rotation
        Quaternion snappedQuaternion = Quaternion.Euler(snappedRotation, tileMeshTransform.eulerAngles.y, tileMeshTransform.eulerAngles.z);

        // Apply the new rotation to the transform
        tileMeshTransform.rotation = snappedQuaternion;

        isRotating = false;
        //Debug.Log("Rotation complete. Snapped Rotation: " + snappedRotation);
    }

    // Public property to expose isRotating
    public bool IsRotating
    {
        get { return isRotating; }
    }
}