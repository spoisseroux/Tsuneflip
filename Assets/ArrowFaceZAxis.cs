using UnityEngine;

public class ArrowFaceZAxis : MonoBehaviour
{
    public float bobAmplitude = 0.1f;  // The height of the bobbing
    public float bobFrequency = 1f;    // The speed of the bobbing

    private Vector3 initialPosition;

    void Start()
    {
        // Store the initial position of the arrow
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        // Ensure the arrow's forward direction always faces the Z-axis
        transform.forward = Vector3.forward;

        // If you want the arrow to be flat (only rotating around Z), you can reset the x and y rotation.
        Vector3 rotation = transform.eulerAngles;
        rotation.x = 0;
        rotation.y = 0;
        transform.eulerAngles = rotation;

        // Bob the arrow up and down
        float newY = initialPosition.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.localPosition = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}