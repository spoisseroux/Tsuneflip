using System.Collections;
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
    public Button worldUpButton;
    public Button worldDownButton;
    public Button levelUpButton;
    public Button levelDownButton;
    public string levelsRootPath = "Levels";

    // UI elements for level preview
    public GameObject levelPreviewPanel;
    public TMP_Text levelNameText;
    public TMP_Text levelDescriptionText;

    public GridGoalPreview gridPreview;
    public GridPreviewCamera gridPreviewCam;
    private int currentWorldIndex = 0;
    private int currentLevelIndex = 0;
    private Button activeWorldButton;
    private bool isAnimating = false;

    void Start()
    {
        levelPreviewPanel.SetActive(false);
        LoadWorlds();

        worldUpButton.onClick.AddListener(() => ScrollWorlds(1));  // Inverted direction
        worldDownButton.onClick.AddListener(() => ScrollWorlds(-1));  // Inverted direction
        levelUpButton.onClick.AddListener(() => ScrollLevels(1));  // Inverted direction
        levelDownButton.onClick.AddListener(() => ScrollLevels(-1));  // Inverted direction
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

            Button worldButton = Instantiate(worldButtonPrefab, worldButtonContainer);
            TMP_Text buttonText = worldButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = worldName;
            worldButton.onClick.AddListener(() => LoadLevels(worldName, worldButton));
        }

        UpdateWorldButtonPositions();

        // Automatically load levels of the first world
        if (worldButtonContainer.childCount > 0)
        {
            string firstWorldName = worldButtonContainer.GetChild(0).GetComponentInChildren<TMP_Text>().text;
            LoadLevels(firstWorldName, worldButtonContainer.GetChild(0).GetComponent<Button>());
        }
    }

    void LoadLevels(string worldName, Button worldButton = null)
    {
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }

        if (activeWorldButton != null)
        {
            SetButtonColor(activeWorldButton, Color.white);
        }

        if (worldButton != null)
        {
            activeWorldButton = worldButton;
        }

        string worldPath = Path.Combine(levelsRootPath, worldName);
        Debug.Log("Loading levels from: " + worldPath);

        LevelData[] levels = Resources.LoadAll<LevelData>(worldPath);
        Debug.Log("Found " + levels.Length + " levels in world: " + worldName);

        foreach (LevelData level in levels)
        {
            Button levelButton = Instantiate(levelButtonPrefab, levelButtonContainer);
            TMP_Text buttonText = levelButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = level.levelName;

            // Store LevelData in the button itself
            levelButton.gameObject.AddComponent<LevelDataHolder>().levelData = level;

            EventTrigger trigger = levelButton.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnter.callback.AddListener((eventData) => ShowLevelPreview(level));
            trigger.triggers.Add(pointerEnter);

            EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            pointerExit.callback.AddListener((eventData) => HideLevelPreview());
            trigger.triggers.Add(pointerExit);

            levelButton.onClick.AddListener(() => LoadLevel(level));
        }

        currentLevelIndex = 0;

        // Force layout rebuild
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(levelButtonContainer as RectTransform);

        // Ensure the level buttons are positioned correctly
        UpdateLevelButtonPositions();

        // Call UpdateLevelButtonPositions again to ensure correct placement
        Invoke("UpdateLevelButtonPositions", 0.1f);

        // Start the fade-in coroutine
        StartCoroutine(FadeInLevelButtons());

        // Show preview of the first level if available, with a delay
        if (levelButtonContainer.childCount > 0)
        {
            Invoke("UpdateLevelPreviewAfterLoad", 0.2f); // Adjust the delay as needed
        }
    }

    void UpdateLevelPreviewAfterLoad()
    {
        ShowLevelPreview(levelButtonContainer.GetChild(0).GetComponent<LevelDataHolder>().levelData);
    }

    IEnumerator FadeInLevelButtons()
    {
        CanvasGroup canvasGroup = levelButtonContainer.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = levelButtonContainer.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0;

        yield return new WaitForSeconds(0.25f);

        float fadeDuration = 0.5f;
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    void ScrollWorlds(int direction)
    {
        if (isAnimating) return;
        int targetIndex = (currentWorldIndex + direction + worldButtonContainer.childCount) % worldButtonContainer.childCount;
        StartCoroutine(AnimateCarousel(worldButtonContainer, currentWorldIndex, targetIndex, () =>
        {
            currentWorldIndex = targetIndex;
            UpdateWorldButtonPositions();

            // Load levels for the new current world
            string currentWorldName = worldButtonContainer.GetChild(currentWorldIndex).GetComponentInChildren<TMP_Text>().text;
            LoadLevels(currentWorldName, worldButtonContainer.GetChild(currentWorldIndex).GetComponent<Button>());

            // Show preview of the first level of the new world with a delay
            if (levelButtonContainer.childCount > 0)
            {
                Invoke("UpdateLevelPreviewAfterScroll", 0.2f); // Adjust the delay as needed
            }
        }));
    }

    void UpdateLevelPreviewAfterScroll()
    {
        ShowLevelPreview(levelButtonContainer.GetChild(0).GetComponent<LevelDataHolder>().levelData);
    }

    void ScrollLevels(int direction)
    {
        if (isAnimating) return;
        int targetIndex = (currentLevelIndex + direction + levelButtonContainer.childCount) % levelButtonContainer.childCount;
        StartCoroutine(AnimateCarousel(levelButtonContainer, currentLevelIndex, targetIndex, () =>
        {
            currentLevelIndex = targetIndex;
            UpdateLevelButtonPositions();

            // Show preview of the current level
            LevelDataHolder levelDataHolder = levelButtonContainer.GetChild(currentLevelIndex).GetComponent<LevelDataHolder>();
            if (levelDataHolder != null)
            {
                ShowLevelPreview(levelDataHolder.levelData);
            }
        }));
    }

    IEnumerator AnimateCarousel(Transform container, int startIndex, int targetIndex, System.Action onComplete)
    {
        isAnimating = true;

        int itemCount = container.childCount;
        Vector3[] startPositions = new Vector3[itemCount];
        Vector3[] endPositions = new Vector3[itemCount];

        for (int i = 0; i < itemCount; i++)
        {
            startPositions[i] = container.GetChild(i).localPosition;
            endPositions[i] = GetCarouselPosition(i - targetIndex, itemCount, 40f);
        }

        float animationDuration = 0.75f;
        float elapsedTime = 0;

        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            t = t * (2 - t); // Ease-out effect

            for (int i = 0; i < itemCount; i++)
            {
                container.GetChild(i).localPosition = Vector3.Lerp(startPositions[i], endPositions[i], t);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        onComplete?.Invoke();
        isAnimating = false;
    }

    void UpdateWorldButtonPositions()
    {
        float baseOpacity = 0.2f; // Minimum opacity for buttons farthest from the center
        float maxDistance = 5; // The maximum distance from the center button

        for (int i = 0; i < worldButtonContainer.childCount; i++)
        {
            Transform child = worldButtonContainer.GetChild(i);
            int distanceFromCenter = Mathf.Abs(i - currentWorldIndex);
            float opacity = Mathf.Lerp(1, baseOpacity, (float)distanceFromCenter / maxDistance);
            SetButtonOpacity(child.GetComponent<Button>(), opacity);

            Vector3 targetPosition = GetCarouselPosition(i - currentWorldIndex, worldButtonContainer.childCount, 40f); // Adjust spacing to 40f
            child.localPosition = targetPosition;
        }
    }

    void UpdateLevelButtonPositions()
    {
        float baseOpacity = 0.2f; // Minimum opacity for buttons farthest from the center
        float maxDistance = 5; // The maximum distance from the center button

        for (int i = 0; i < levelButtonContainer.childCount; i++)
        {
            Transform child = levelButtonContainer.GetChild(i);
            int distanceFromCenter = Mathf.Abs(i - currentLevelIndex);
            float opacity = Mathf.Lerp(1, baseOpacity, (float)distanceFromCenter / maxDistance);
            SetButtonOpacity(child.GetComponent<Button>(), opacity);

            Vector3 targetPosition = GetCarouselPosition(i - currentLevelIndex, levelButtonContainer.childCount, 40f); // Adjust spacing to 40f
            child.localPosition = targetPosition;
        }

        //Debug.Log("Updated level button positions.");
    }

    Vector3 GetCarouselPosition(int index, int count, float spacing)
    {
        return new Vector3(0, -index * spacing, 0);
    }

    void LoadLevel(LevelData level)
    {
        Debug.Log("Loading level: " + level.levelName);
        // Example: SceneManager.LoadScene(level.sceneName);
    }

    void ShowLevelPreview(LevelData level)
    {
        levelPreviewPanel.SetActive(true);
        levelNameText.text = level.levelName;
        gridPreviewCam.levelData = level;
        gridPreview.goal = level;

        gridPreview.InitializeGridPreview(level);


        // levelDescriptionText.text = level.description; // Assuming your ScriptableObject has a description field
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

    void SetButtonOpacity(Button button, float opacity)
    {
        ColorBlock colors = button.colors;
        Color newColor = colors.normalColor;
        newColor.a = opacity;

        colors.normalColor = newColor;
        colors.highlightedColor = newColor;
        colors.pressedColor = newColor;
        colors.selectedColor = newColor;
        colors.disabledColor = newColor;

        button.colors = colors;

        CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = button.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = opacity;
    }

    // Helper class to hold LevelData
    private class LevelDataHolder : MonoBehaviour
    {
        public LevelData levelData;
    }
}