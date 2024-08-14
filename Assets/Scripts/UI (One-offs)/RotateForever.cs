using UnityEngine;

public class RotateForever : MonoBehaviour
{
    public float rotationSpeed = 180f; // Degrees per second (360 degrees / 2 seconds)

    void Update()
    {
        // Calculate the amount of rotation for this frame
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Apply the rotation to the transform
        transform.Rotate(Vector3.right, rotationAmount);
    }
}