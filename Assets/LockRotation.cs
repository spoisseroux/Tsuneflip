using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private Quaternion initialRotation;
    public GameObject parentObject; // Reference to the parent object
    public Vector3 offset = new Vector3(-2.5f, -2.5f, 0f); // Desired offset relative to the parent

    void Start()
    {
        // Store the initial local rotation of the child object
        initialRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        // Reset the local rotation to the initial value every frame
        transform.localRotation = initialRotation;

        if (parentObject != null)
        {
            // Set the child's position to the parent's position plus the offset
            transform.position = parentObject.transform.position + offset;
        }
        else
        {
            Debug.LogWarning("Parent object not assigned.");
        }
    }
    
}
