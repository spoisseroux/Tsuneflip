using System.Collections;
using System.Collections.Generic;
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
    public Button worldUpButton;
    public Button worldDownButton;
    public Button levelUpButton;
    public Button levelDownButton;
    public string levelAssetsRootPath = "LevelAssets";

    public TransitionHandler transitioner;
    public GameObject levelPreviewPanel;
    public TMP_Text levelNameText;
    public TMP_Text levelDescriptionText;
    public Button startButton;

    public GridGoalPreview gridPreview;
    public GridPreviewCamera gridPreviewCam;
    private int currentWorldIndex = 0;
    private int currentLevelIndex = 0;
    private Button activeWorldButton;
    private bool isAnimating = false;

    private List<WorldData> worlds;

    // static LevelData to indicate which level we are loading, persists into Gameplay Scene!!!
    public static LevelData loaded;

    // Reference to the UI Cubemap material
    public Material uiCubemapMat;

    void Start()
    {
        levelPreviewPanel.SetActive(false);
        LoadWorlds();
        SetupButtonListeners();
    }

    void SetupButtonListeners()
    {
        worldUpButton.onClick.AddListener(() => ScrollWorlds(1));
        worldDownButton.onClick.AddListener(() => ScrollWorlds(-1));
        levelUpButton.onClick.AddListener(() => ScrollLevels(1));
        levelDownButton.onClick.AddListener(() => ScrollLevels(-1));
    }

    void LoadWorlds()
    {
        worlds = new List<WorldData>(Resources.LoadAll<WorldData>(levelAssetsRootPath));

        if (worlds == null || worlds.Count == 0)
        {
            Debug.LogError("No worlds found in Resources/LevelAssets");
            return;
        }

        foreach (WorldData worldData in worlds)
        {
            Debug.Log("Found world: " + worldData.worldName);

            Button worldButton = Instantiate(worldButtonPrefab, worldButtonContainer);
            TMP_Text buttonText = worldButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = worldData.worldName;
            worldButton.onClick.AddListener(() => LoadLevels(worldData, worldButton));
        }

        UpdateWorldButtonPositions();

        // Automatically load levels of the first world
        if (worldButtonContainer.childCount > 0)
        {
            WorldData firstWorldData = worlds[0];
            LoadLevels(firstWorldData, worldButtonContainer.GetChild(0).GetComponent<Button>());
        }
    }

    void UpdateLevelPreviewAfterLoad()
    {
        if (levelButtonContainer.childCount > 0)
        {
            LevelDataHolder levelDataHolder = levelButtonContainer.GetChild(0).GetComponent<LevelDataHolder>();
            if (levelDataHolder != null)
            {
                ShowLevelPreview(levelDataHolder.levelData);
            }
        }
    }

    void LoadLevels(WorldData worldData, Button worldButton = null)
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

        string worldPath = Path.Combine(levelAssetsRootPath, worldData.worldFolderName);
        Debug.Log("Loading levels from: " + worldPath);

        LevelData[] levels = Resources.LoadAll<LevelData>(worldPath);
        Debug.Log("Found " + levels.Length + " levels in world: " + worldData.worldName);

        // Trigger the cubemap transition
        StartCubemapTransition(worldData.worldCubemap, worldData.worldCubemapColor);
        //HERE

        foreach (LevelData level in levels)
        {
            Button levelButton = Instantiate(levelButtonPrefab, levelButtonContainer);
            TMP_Text buttonText = levelButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = level.levelName;

            // Store LevelData in the button itself
            LevelDataHolder dataHolder = levelButton.gameObject.AddComponent<LevelDataHolder>();
            dataHolder.levelData = level;

            // Optional: Add event listeners for preview and loading level
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

    void StartCubemapTransition(Cubemap newCubemap, Color newColor)
    {
        // Set the CubemapNext property to the new cubemap
        uiCubemapMat.SetTexture("_CubemapNext", newCubemap);
        uiCubemapMat.SetColor("_ColorNext", newColor);

        // Start the blend animation
        StartCoroutine(AnimateCubemapTransition());
    }

    IEnumerator AnimateCubemapTransition()
    {
        float duration = 0.2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float blendFactor = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            uiCubemapMat.SetFloat("_BlendFactor", blendFactor);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the blend factor is set to 1
        uiCubemapMat.SetFloat("_BlendFactor", 1f);

        // After the transition, set CubemapCurr to the new cubemap and reset blend factor
        uiCubemapMat.SetTexture("_CubemapCurr", uiCubemapMat.GetTexture("_CubemapNext"));
        uiCubemapMat.SetColor("_ColorCurr", uiCubemapMat.GetColor("_ColorNext"));
        uiCubemapMat.SetFloat("_BlendFactor", 0f);
    }

    IEnumerator FadeInLevelButtons()
    {
        CanvasGroup canvasGroup = levelButtonContainer.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = levelButtonContainer.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0;

        yield return new WaitForSeconds(0.1f);

        float fadeDuration = 0.2f;
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
            WorldData currentWorldData = worlds[currentWorldIndex];
            LoadLevels(currentWorldData, worldButtonContainer.GetChild(currentWorldIndex).GetComponent<Button>());

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

        float animationDuration = 0.3f;
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
        float baseOpacity = 0.05f; // Minimum opacity for buttons farthest from the center
        float maxDistance = 3; // The maximum distance from the center button

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
        float baseOpacity = 0.05f; // Minimum opacity for buttons farthest from the center
        float maxDistance = 3; // The maximum distance from the center button

        for (int i = 0; i < levelButtonContainer.childCount; i++)
        {
            Transform child = levelButtonContainer.GetChild(i);
            int distanceFromCenter = Mathf.Abs(i - currentLevelIndex);
            float opacity = Mathf.Lerp(1, baseOpacity, (float)distanceFromCenter / maxDistance);
            SetButtonOpacity(child.GetComponent<Button>(), opacity);

            Vector3 targetPosition = GetCarouselPosition(i - currentLevelIndex, levelButtonContainer.childCount, 40f); // Adjust spacing to 40f
            child.localPosition = targetPosition;
        }
    }

    Vector3 GetCarouselPosition(int index, int count, float spacing)
    {
        return new Vector3(0, -index * spacing, 0);
    }

    public void LoadLevel(LevelData level)
    {
        StartCoroutine(LoadLevelCoroutine(level));
    }

    //TODO: PUT LEVEL DATA PIPING HERE
    private IEnumerator LoadLevelCoroutine(LevelData level)
    {
        yield return transitioner.ExitTransition();
        Debug.Log("Loading level: " + level.levelName);
        // Example: SceneManager.LoadScene(level.sceneName);
    }

    void ShowLevelPreview(LevelData level)
    {
        levelPreviewPanel.SetActive(true);
        levelNameText.text = level.levelName;
        gridPreviewCam.levelData = level;
        gridPreview.goal = level;
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() => LoadLevel(level));

        gridPreview.InitializeGridPreview(level);

        // set the static variable for persistence when selecting
        loaded = level;
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

    private class LevelDataHolder : MonoBehaviour
    {
        public LevelData levelData;
    }
}