using UnityEngine;

public class ArrowFaceZAxis : MonoBehaviour
{
    public Transform player;            // Reference to the player
    public Vector3 localOffset;         // Position relative to the player
    public float bobAmplitude = 0.1f;   // The height of the bobbing
    public float bobFrequency = 1f;     // The speed of the bobbing

    private Vector3 initialLocalPosition;

    void Start()
    {
        // Set the initial position relative to the player
        initialLocalPosition = localOffset;
        transform.position = player.position + localOffset;
    }

    void LateUpdate()
    {
        // Calculate the new position with bobbing
        float bobbingY = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        Vector3 bobbingPosition = new Vector3(localOffset.x, localOffset.y + bobbingY, localOffset.z);

        // Maintain the position relative to the player's position without considering rotation
        transform.position = player.position + bobbingPosition;

        // Keep the object facing the global Z-axis
        transform.rotation = Quaternion.LookRotation(Vector3.forward);
    }
}