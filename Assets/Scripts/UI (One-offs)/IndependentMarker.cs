using UnityEngine;

public class IndependentMarker : MonoBehaviour
{
    public float rotationSpeed = 20f; // Speed of the marker's rotation
    public float bobSpeed = 2f; // Speed of the bobbing motion
    public float bobHeight = 0.5f; // Height of the bobbing motion

    private float initialY;
    private Quaternion initialRotation;

    void Start()
    {
        // Store the initial Y position and initial rotation of the marker
        initialY = transform.position.y;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // Maintain original horizontal position and independently adjust Y position
        Vector3 position = transform.position;
        position.y = initialY + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = position;

        // Rotate the marker around its local z-axis (spinning like a top)
        transform.rotation = initialRotation * Quaternion.Euler(0, 0, rotationSpeed * Time.time);
    }
}
