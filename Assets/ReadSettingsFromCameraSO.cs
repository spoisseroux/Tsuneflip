using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ReadSettingsFromCameraSO : MonoBehaviour
{
    public CameraSettings cameraSettings;
    public CinemachineFreeLook mainCamera;
    public float minSens = 5f;
    public float maxSens = 50f;

    void Start()
    {
        UpdateCamSettings();
    }

    public void UpdateCamSettings(){
        SetInversion();
        AdjustSensitivity(cameraSettings.cameraSensitivity);
    }

    public void SetInversion()
    {
        if (cameraSettings.cameraIsInverted)
        {
            mainCamera.m_XAxis.m_InvertInput = true;
            mainCamera.m_YAxis.m_InvertInput = true;
        }
        else
        {
            mainCamera.m_XAxis.m_InvertInput = false;
            mainCamera.m_YAxis.m_InvertInput = false;
        }
    }

    public void AdjustSensitivity(float value)
    {
        if (mainCamera != null)
        {
            // Calculate the speed based on slider value (0 to 1 mapped to minSpeed to maxSpeed)
            float speed = Mathf.Lerp(minSens, maxSens, value);

            // Adjust both X and Y axis speeds
            mainCamera.m_XAxis.m_MaxSpeed = speed;
            mainCamera.m_YAxis.m_MaxSpeed = speed/105;
        }
    }
}
