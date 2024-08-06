using UnityEngine;
using System.Collections;

public class RotateTile : MonoBehaviour
{
    private float rotationPerSecond_c = 180f / 0.5f;  // 180 degrees over 0.5 seconds

    private float frameRotation = 0f;
    private bool isRotating = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isRotating)
        {
            StartCoroutine(Rotate());
        }
    }

    IEnumerator Rotate()
    {
        isRotating = true;
        float amountRotated = 0f;
        while (amountRotated < 180f) {
            frameRotation = rotationPerSecond_c * Time.deltaTime;  // Amount to rotate this frame
            transform.Rotate(frameRotation, 0, 0);  // Apply the rotation
            amountRotated += frameRotation;  // Keep track of the amount rotated so far
            yield return new WaitForEndOfFrame();  // Rotate every frame until we reach the target
        }

        // Snap to nearest valid rotation (0, 180, or 360 degrees)
        float currentXRotation = transform.eulerAngles.x;
        float snappedRotation = Mathf.Round(currentXRotation / 180f) * 180f;

        // Create a new Quaternion with the snapped rotation
        Quaternion snappedQuaternion = Quaternion.Euler(snappedRotation, transform.eulerAngles.y, transform.eulerAngles.z);

        // Apply the new rotation to the transform
        transform.rotation = snappedQuaternion;
        
        isRotating = false;
    }
}