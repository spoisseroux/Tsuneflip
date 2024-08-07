using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LevelMenuManager : MonoBehaviour
{
    public Transform worldButtonContainer;
    public Transform levelButtonContainer;
    public Button worldButtonPrefab;
    public Button levelButtonPrefab;
    public string levelsRootPath = "Levels";

    // UI elements for level preview
    public GameObject levelPreviewPanel;
    public TMP_Text levelNameText;
    public TMP_Text levelDescriptionText;

    private Button activeWorldButton;

    void Start()
    {
        levelPreviewPanel.SetActive(false);
        LoadWorlds();
    }

    void LoadWorlds()
    {
        string path = Path.Combine(Application.dataPath, "Resources", levelsRootPath);
        Debug.Log("Worlds Path: " + path);

        if (!Directory.Exists(path))
        {
            Debug.LogError("Directory does not exist: " + path);
            return;
        }

        string[] worldDirectories = Directory.GetDirectories(path);
        Debug.Log("Found " + worldDirectories.Length + " world directories.");

        foreach (string worldDir in worldDirectories)
        {
            string worldName = Path.GetFileName(worldDir);
            Debug.Log("Found world: " + worldName);

            if (worldButtonPrefab == null)
            {
                Debug.LogError("worldButtonPrefab is not assigned.");
                return;
            }

            if (worldButtonContainer == null)
            {
                Debug.LogError("worldButtonContainer is not assigned.");
                return;
            }

            Button worldButton = Instantiate(worldButtonPrefab, worldButtonContainer);
            if (worldButton == null)
            {
                Debug.LogError("Failed to instantiate worldButtonPrefab.");
                return;
            }

            TMP_Text buttonText = worldButton.GetComponentInChildren<TMP_Text>();
            if (buttonText == null)
            {
                Debug.LogError("TMP_Text component not found in worldButtonPrefab.");
                return;
            }

            buttonText.text = worldName;
            worldButton.onClick.AddListener(() => LoadLevels(worldName, worldButton));
        }
    }

    void LoadLevels(string worldName, Button worldButton)
    {
        // Clear previous level buttons
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Reset the color of the previously active world button
        if (activeWorldButton != null)
        {
            SetButtonColor(activeWorldButton, Color.white);
        }

        // Set the active world button and grey it out
        activeWorldButton = worldButton;
        SetButtonColor(activeWorldButton, Color.grey);

        string worldPath = Path.Combine(levelsRootPath, worldName);
        Debug.Log("Loading levels from: " + worldPath);

        LevelData[] levels = Resources.LoadAll<LevelData>(worldPath);
        Debug.Log("Found " + levels.Length + " levels in world: " + worldName);

        foreach (LevelData level in levels)
        {
            Button levelButton = Instantiate(levelButtonPrefab, levelButtonContainer);
            TMP_Text buttonText = levelButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = level.levelName;
            }

            // Add event listeners for hover
            EventTrigger trigger = levelButton.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnter.callback.AddListener((eventData) => ShowLevelPreview(level));
            trigger.triggers.Add(pointerEnter);

            EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            pointerExit.callback.AddListener((eventData) => HideLevelPreview());
            trigger.triggers.Add(pointerExit);

            levelButton.onClick.AddListener(() => LoadLevel(level));
        }
    }

    void LoadLevel(LevelData level)
    {
        // Load the level (e.g., load the scene or do something with the level data)
        Debug.Log("Loading level: " + level.levelName);
        // Example: SceneManager.LoadScene(level.sceneName);
    }

    void ShowLevelPreview(LevelData level)
    {
        levelPreviewPanel.SetActive(true);
        levelNameText.text = level.levelName;
        //levelDescriptionText.text = level.description; // Assuming your ScriptableObject has a description field
    }

    void HideLevelPreview()
    {
        levelPreviewPanel.SetActive(false);
    }

    void SetButtonColor(Button button, Color color)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = color;
        colors.pressedColor = color;
        colors.selectedColor = color;
        colors.disabledColor = color;
        button.colors = colors;
    }
}
//TODO: CHANGE TestLevelScriptableObject TO NEW SCRIPTABLE OBJECT NAME, LIKE JUST Level AFTER TESTING