using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using System.Collections.Generic;
using TMPro;

public class VolumeSlider : MonoBehaviour
{
    private string masterBusString = "bus:/";
    private string musicBusString = "bus:/MusicBus";
    private string sfxBusString = "bus:/SFXBus";
    [SerializeField] private Slider masterBusSlider;
    [SerializeField] private Slider musicBusSlider;
    [SerializeField] private Slider sfxBusSlider;

    private FMOD.Studio.Bus musicBus;
    private FMOD.Studio.Bus sfxBus;
    private FMOD.Studio.Bus masterBus;

    [SerializeField] private CameraSettings levelCameraSettings;
    [SerializeField] private Slider cameraSensSlider;
    [SerializeField] private Toggle cameraInvToggle;
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private Resolution[] availableResolutions =
    {
        new Resolution { width = 3840, height = 2160 },
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 1280, height = 720 }
    };
    [SerializeField] private TMP_Dropdown displayModeDropdown;
    private enum DisplayMode
    {
        Fullscreen,
        BorderlessWindowed,
        Windowed
    }


    void Start()
    {
        if (displayModeDropdown != null)
        {
            // Clear existing options and populate dropdown with display modes
            displayModeDropdown.ClearOptions();
            displayModeDropdown.AddOptions(new List<string> { "Fullscreen", "Borderless", "Windowed" });

            // Set the current display mode as the selected option in the dropdown
            displayModeDropdown.value = GetCurrentDisplayModeIndex();
            displayModeDropdown.RefreshShownValue();

            // Add listener to handle changes in the dropdown's value
            displayModeDropdown.onValueChanged.AddListener(ChangeDisplayMode);
        }

        if (resolutionDropdown != null)
        {
            // Clear existing options
            resolutionDropdown.ClearOptions();

            // Populate dropdown with available resolutions
            resolutionDropdown.AddOptions(GetResolutionOptions());

            // Set the current resolution as the selected option in the dropdown
            int currentResolutionIndex = GetCurrentResolutionIndex();
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            // Add listener to handle changes in the dropdown's value
            resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
        }

        masterBus = FMODUnity.RuntimeManager.GetBus(masterBusString);
        masterBusSlider.onValueChanged.AddListener(MasterSetVolume);
        masterBusSlider.value = 1f; // Start with the slider at full volume

        musicBus = FMODUnity.RuntimeManager.GetBus(musicBusString);
        musicBusSlider.onValueChanged.AddListener(MusicSetVolume);
        musicBusSlider.value = 1f; 

        sfxBus = FMODUnity.RuntimeManager.GetBus(sfxBusString);
        sfxBusSlider.onValueChanged.AddListener(SFXSetVolume);
        sfxBusSlider.value = 1f; // Start with the slider at full volume

        cameraSensSlider.onValueChanged.AddListener(UpdateCameraSensitivity);
        cameraSensSlider.value = levelCameraSettings.cameraSensitivity;

        cameraInvToggle.isOn = levelCameraSettings.cameraIsInverted;
        cameraInvToggle.onValueChanged.AddListener(ToggleCamInv);

        vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
        vsyncToggle.onValueChanged.AddListener(ToggleVSync);
        

    }

    private int GetCurrentDisplayModeIndex()
    {
        if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
        {
            return (int)DisplayMode.Fullscreen;
        }
        else if (Screen.fullScreenMode == FullScreenMode.MaximizedWindow)
        {
            return (int)DisplayMode.BorderlessWindowed;
        }
        else
        {
            return (int)DisplayMode.Windowed;
        }
    }

    private void ChangeDisplayMode(int modeIndex)
    {
        DisplayMode selectedMode = (DisplayMode)modeIndex;

        switch (selectedMode)
        {
            case DisplayMode.Fullscreen:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case DisplayMode.BorderlessWindowed:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
            case DisplayMode.Windowed:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }

        Debug.Log("Display mode changed to: " + selectedMode.ToString());
    }

    private List<string> GetResolutionOptions()
    {
        List<string> options = new List<string>();

        foreach (Resolution res in availableResolutions)
        {
            string option = res.width + " x " + res.height;
            options.Add(option);
        }

        return options;
    }

    private int GetCurrentResolutionIndex()
    {
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            if (Screen.width == availableResolutions[i].width && Screen.height == availableResolutions[i].height)
            {
                return i;
            }
        }

        // If the current resolution is not in the list, return the default option (0)
        return 0;
    }

    private void ChangeResolution(int resolutionIndex)
    {
        Resolution selectedResolution = availableResolutions[resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
        Debug.Log("Resolution changed to: " + selectedResolution.width + " x " + selectedResolution.height);
    }

    private void ToggleVSync(bool isVSyncOn)
    {
        if (isVSyncOn)
        {
            // Enable VSync
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            // Disable VSync
            QualitySettings.vSyncCount = 0;
        }

        Debug.Log("VSync is " + (isVSyncOn ? "enabled" : "disabled"));
    }

    private void ToggleCamInv(bool newValue)
    {
        levelCameraSettings.cameraIsInverted = newValue;
        Debug.Log("Camera inv toggled: " + levelCameraSettings.cameraIsInverted);
    }

    private void UpdateCameraSensitivity(float newValue)
    {
            levelCameraSettings.cameraSensitivity = newValue;
            Debug.Log("Updated Cam Sensitivity value: " + levelCameraSettings.cameraSensitivity);
        
    }
    public void MasterSetVolume(float sliderValue)
    {
        float volumeInDb;

        // Convert slider value to dB, avoiding log(0) error by setting a minimum value
        if (sliderValue <= 0.0001f)
        {
            volumeInDb = -80f; // Silence the bus (or set to your minimum dB level)
        }
        else
        {
            volumeInDb = Mathf.Log10(sliderValue) * 20f;
        }

        masterBus.setVolume(sliderValue); // Alternatively, you can use `bus.setFaderLevel(volumeInDb);` depending on how you want to control the volume.
    }
    public void MusicSetVolume(float sliderValue)
    {
        float volumeInDb;

        // Convert slider value to dB, avoiding log(0) error by setting a minimum value
        if (sliderValue <= 0.0001f)
        {
            volumeInDb = -80f; // Silence the bus (or set to your minimum dB level)
        }
        else
        {
            volumeInDb = Mathf.Log10(sliderValue) * 20f;
        }

        musicBus.setVolume(sliderValue); // Alternatively, you can use `bus.setFaderLevel(volumeInDb);` depending on how you want to control the volume.
    }
    public void SFXSetVolume(float sliderValue)
    {
        float volumeInDb;

        // Convert slider value to dB, avoiding log(0) error by setting a minimum value
        if (sliderValue <= 0.0001f)
        {
            volumeInDb = -80f; // Silence the bus (or set to your minimum dB level)
        }
        else
        {
            volumeInDb = Mathf.Log10(sliderValue) * 20f;
        }

        sfxBus.setVolume(sliderValue); // Alternatively, you can use `bus.setFaderLevel(volumeInDb);` depending on how you want to control the volume.
    }
}
