using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelMenuManager : MonoBehaviour
{
    public Transform worldButtonContainer;
    public Transform levelButtonContainer;
    public Button worldButtonPrefab;
    public Button levelButtonPrefab;
    public string levelsRootPath = "Levels";

    void Start()
    {
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
            worldButton.onClick.AddListener(() => LoadLevels(worldName));
        }
    }

    void LoadLevels(string worldName)
    {
        // Clear previous level buttons
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }

        string worldPath = Path.Combine(levelsRootPath, worldName);
        Debug.Log("Loading levels from: " + worldPath);

        TestLevelScriptableObject[] levels = Resources.LoadAll<TestLevelScriptableObject>(worldPath);
        Debug.Log("Found " + levels.Length + " levels in world: " + worldName);

        foreach (TestLevelScriptableObject level in levels)
        {
            Button levelButton = Instantiate(levelButtonPrefab, levelButtonContainer);
            TMP_Text buttonText = levelButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = level.levelName;
            }
            levelButton.onClick.AddListener(() => LoadLevel(level));
        }
    }

    void LoadLevel(TestLevelScriptableObject level)
    {
        // Load the level (e.g., load the scene or do something with the level data)
        Debug.Log("Loading level: " + level.levelName);
        // Example: SceneManager.LoadScene(level.sceneName);
    }
}

//TODO: CHANGE TestLevelScriptableObject TO NEW SCRIPTABLE OBJECT NAME, LIKE JUST Level AFTER TESTING