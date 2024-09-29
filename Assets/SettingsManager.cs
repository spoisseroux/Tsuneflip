using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using System.Collections.Generic;
using TMPro;

public class SettingsManager : MonoBehaviour
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
        LoadSettings();

        if (displayModeDropdown != null)
        {
            displayModeDropdown.ClearOptions();
            displayModeDropdown.AddOptions(new List<string> { "Fullscreen", "Borderless", "Windowed" });

            displayModeDropdown.value = GetCurrentDisplayModeIndex();
            displayModeDropdown.RefreshShownValue();

            displayModeDropdown.onValueChanged.AddListener(ChangeDisplayMode);
        }

        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(GetResolutionOptions());

            resolutionDropdown.value = GetCurrentResolutionIndex();
            resolutionDropdown.RefreshShownValue();

            resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
        }

        masterBus = FMODUnity.RuntimeManager.GetBus(masterBusString);
        masterBusSlider.onValueChanged.AddListener(MasterSetVolume);
        masterBusSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);

        musicBus = FMODUnity.RuntimeManager.GetBus(musicBusString);
        musicBusSlider.onValueChanged.AddListener(MusicSetVolume);
        musicBusSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);

        sfxBus = FMODUnity.RuntimeManager.GetBus(sfxBusString);
        sfxBusSlider.onValueChanged.AddListener(SFXSetVolume);
        sfxBusSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        cameraSensSlider.onValueChanged.AddListener(UpdateCameraSensitivity);
        cameraSensSlider.value = PlayerPrefs.GetFloat("CameraSensitivity", levelCameraSettings.cameraSensitivity);

        cameraInvToggle.isOn = PlayerPrefs.GetInt("CameraInverted", levelCameraSettings.cameraIsInverted ? 1 : 0) == 1;
        cameraInvToggle.onValueChanged.AddListener(ToggleCamInv);

        vsyncToggle.isOn = PlayerPrefs.GetInt("VSync", QualitySettings.vSyncCount > 0 ? 1 : 0) == 1;
        vsyncToggle.onValueChanged.AddListener(ToggleVSync);
    }

    void OnApplicationQuit()
    {
        SaveSettings();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterBusSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicBusSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxBusSlider.value);
        PlayerPrefs.SetFloat("CameraSensitivity", cameraSensSlider.value);
        PlayerPrefs.SetInt("CameraInverted", cameraInvToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("VSync", vsyncToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("DisplayMode", displayModeDropdown.value);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        // Load any previously saved settings (already used in Start method)
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

        PlayerPrefs.SetInt("DisplayMode", modeIndex);
        PlayerPrefs.Save();

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

        return PlayerPrefs.GetInt("ResolutionIndex", 0);
    }

    private void ChangeResolution(int resolutionIndex)
    {
        Resolution selectedResolution = availableResolutions[resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();

        Debug.Log("Resolution changed to: " + selectedResolution.width + " x " + selectedResolution.height);
    }

    private void ToggleVSync(bool isVSyncOn)
    {
        QualitySettings.vSyncCount = isVSyncOn ? 1 : 0;
        PlayerPrefs.SetInt("VSync", isVSyncOn ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("VSync is " + (isVSyncOn ? "enabled" : "disabled"));
    }

    private void ToggleCamInv(bool newValue)
    {
        levelCameraSettings.cameraIsInverted = newValue;
        PlayerPrefs.SetInt("CameraInverted", newValue ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("Camera inversion toggled: " + levelCameraSettings.cameraIsInverted);
    }

    private void UpdateCameraSensitivity(float newValue)
    {
        levelCameraSettings.cameraSensitivity = newValue;
        PlayerPrefs.SetFloat("CameraSensitivity", newValue);
        PlayerPrefs.Save();

        Debug.Log("Updated Camera Sensitivity value: " + levelCameraSettings.cameraSensitivity);
    }

    public void MasterSetVolume(float sliderValue)
    {
        float volumeInDb = sliderValue <= 0.0001f ? -80f : Mathf.Log10(sliderValue) * 20f;
        masterBus.setVolume(sliderValue);
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
        PlayerPrefs.Save();
    }

    public void MusicSetVolume(float sliderValue)
    {
        float volumeInDb = sliderValue <= 0.0001f ? -80f : Mathf.Log10(sliderValue) * 20f;
        musicBus.setVolume(sliderValue);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        PlayerPrefs.Save();
    }

    public void SFXSetVolume(float sliderValue)
    {
        float volumeInDb = sliderValue <= 0.0001f ? -80f : Mathf.Log10(sliderValue) * 20f;
        sfxBus.setVolume(sliderValue);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
        PlayerPrefs.Save();
    }
}