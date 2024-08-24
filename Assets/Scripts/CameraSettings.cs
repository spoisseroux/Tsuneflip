using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "CameraSettings")]
public class CameraSettings : ScriptableObject
{
    [SerializeField] public float cameraSensitivity;
    [SerializeField] public bool cameraIsInverted;
}

