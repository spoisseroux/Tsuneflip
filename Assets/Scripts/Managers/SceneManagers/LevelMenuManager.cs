using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class LevelMenuManager : MonoBehaviour
{
    public Button worldButtonPrefab;
    public Button levelButtonPrefab;

    public Material uiCubemapMat;
    public LevelMusicManager levelMusicManager;
    public GoalPreviewInstancer gridPreview;
    public GoalPreviewCamera gridPreviewCam;
    public TransitionColorScriptableObject transitionColor;
    public Material UITransitionMat;
    private Transform worldOrganizer, levelOrganizer;
    private Button worldUpButton, worldDownButton, levelUpButton, levelDownButton, activeWorldButton;
    private string levelAssetsRootPath = "LevelAssets";
    private TMP_Text levelNameText, levelBestTimeText;
    //public Button startButton;
    [HideInInspector] public int currentWorldIndex;
    [HideInInspector] public int currentLevelIndex;
    private bool isAnimating = false;
    private List<WorldData> worlds;
    // static LevelData to indicate which level we are loading, persists into Gameplay Scene!!!
    public static LevelData loaded;
    public PreviewToLeaderboardUIController previewToLeaderboard;

    void Start()
    {
        CursorManager.UnlockCursor();
        levelMusicManager.PlayEvent("event:/PlayLevelMenuMusic");
        SetupButtons();
        LoadWorlds();

        //load saved world/level index if its in playerprefs
        currentWorldIndex = PlayerPrefs.GetInt("LastWorldIndex", 0); // Default to 0 if no data is found
        Debug.Log("CurrentWorldIndex: " + currentWorldIndex);
        currentLevelIndex = PlayerPrefs.GetInt("LastLevelIndex", 0);
        Debug.Log("CurrentLevelIndex: " + currentLevelIndex);

        // Scroll directly to the saved indexes
        ScrollWorldsToSavedIndex(currentWorldIndex);
        ScrollLevelsToSavedIndex(currentLevelIndex);
    }

    void ScrollWorldsToSavedIndex(int targetWorldIndex)
    {
        if (targetWorldIndex < worldOrganizer.childCount && targetWorldIndex >= 0)
        {
            currentWorldIndex = targetWorldIndex;
            UpdateWorldButtonPositions(); // Update UI to show the correct world
            LoadLevels(worlds[currentWorldIndex], worldOrganizer.GetChild(currentWorldIndex).GetComponent<Button>()); // Load levels for the selected world
        }
    }

    void ScrollLevelsToSavedIndex(int targetLevelIndex)
    {
        if (targetLevelIndex < levelOrganizer.childCount && targetLevelIndex >= 0)
        {
            currentLevelIndex = targetLevelIndex;
            UpdateLevelButtonPositions(); // Update UI to show the correct level
            if (levelOrganizer.childCount > 0)
            {
                LevelDataHolder levelDataHolder = levelOrganizer.GetChild(currentLevelIndex).GetComponent<LevelDataHolder>();
                if (levelDataHolder != null)
                {
                    ShowLevelPreview(levelDataHolder.levelData); // Show the preview for the selected level
                }
            }
        }
    }

    void SetupButtons()
    {
        //TODO: FIx these:///
        worldOrganizer = GameObject.Find("Canvas/ScreensHolder/MainScreen/LevelSelectHolder/WorldOrganizer").transform;
        levelOrganizer = GameObject.Find("Canvas/ScreensHolder/MainScreen/LevelSelectHolder/LevelOrganizer").transform;
        worldUpButton = GameObject.Find("Canvas/ScreensHolder/MainScreen/LevelSelectHolder/WorldUpButton").GetComponent<Button>();
        worldDownButton = GameObject.Find("Canvas/ScreensHolder/MainScreen/LevelSelectHolder/WorldDownButton").GetComponent<Button>();
        levelUpButton = GameObject.Find("Canvas/ScreensHolder/MainScreen/LevelSelectHolder/LevelUpButton").GetComponent<Button>();
        levelDownButton = GameObject.Find("Canvas/ScreensHolder/MainScreen/LevelSelectHolder/LevelDownButton").GetComponent<Button>();
        levelNameText = GameObject.Find("Canvas/ScreensHolder/MainScreen/LevelPreview/LevelNameAndSwapButton/LevelNameText").GetComponent<TMP_Text>();
        levelBestTimeText = GameObject.Find("Canvas/ScreensHolder/MainScreen/LevelPreview/LevelBestTime").GetComponent<TMP_Text>();

        worldUpButton.onClick.AddListener(() => ScrollWorlds(-1));
        worldDownButton.onClick.AddListener(() => ScrollWorlds(1));
        levelUpButton.onClick.AddListener(() => ScrollLevels(-1));
        levelDownButton.onClick.AddListener(() => ScrollLevels(1));
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

            Button worldButton = Instantiate(worldButtonPrefab, worldOrganizer);
            TMP_Text buttonText = worldButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = worldData.worldName;
            worldButton.onClick.AddListener(() => LoadLevels(worldData, worldButton));
        }

        UpdateWorldButtonPositions();

        // Automatically load levels of the first world
        if (worldOrganizer.childCount > 0)
        {
            WorldData firstWorldData = worlds[0];
            LoadLevels(firstWorldData, worldOrganizer.GetChild(0).GetComponent<Button>());
        }
    }

    void UpdateLevelPreviewAfterLoad()
    {
        if (levelOrganizer.childCount > 0)
        {
            LevelDataHolder levelDataHolder = levelOrganizer.GetChild(0).GetComponent<LevelDataHolder>();
            if (levelDataHolder != null)
            {
                ShowLevelPreview(levelDataHolder.levelData); 
            }
        }
    }

    void LoadLevels(WorldData worldData, Button worldButton = null)
    {
        foreach (Transform child in levelOrganizer)
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
            Button levelButton = Instantiate(levelButtonPrefab, levelOrganizer);
            TMP_Text buttonText = levelButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = level.levelName;

            //TODO:
            //level.bestTime = float.MaxValue;

            // Store LevelData in the button itself
            LevelDataHolder dataHolder = levelButton.gameObject.AddComponent<LevelDataHolder>();
            dataHolder.levelData = level;

            // Optional: Add event listeners for preview and loading level
            //levelButton.onClick.AddListener(() => LoadLevel(level));
        }

        currentLevelIndex = 0;

        // Force layout rebuild
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(levelOrganizer as RectTransform);

        // Ensure the level buttons are positioned correctly
        UpdateLevelButtonPositions();

        // Call UpdateLevelButtonPositions again to ensure correct placement
        Invoke("UpdateLevelButtonPositions", 0.1f);

        // Start the fade-in coroutine
        StartCoroutine(FadeInLevelButtons());

        // Show preview of the first level if available, with a delay
        if (levelOrganizer.childCount > 0)
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
        CanvasGroup canvasGroup = levelOrganizer.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = levelOrganizer.gameObject.AddComponent<CanvasGroup>();
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
        int targetIndex = (currentWorldIndex + direction + worldOrganizer.childCount) % worldOrganizer.childCount;
        StartCoroutine(AnimateCarousel(worldOrganizer, currentWorldIndex, targetIndex, () =>
        {
            currentWorldIndex = targetIndex;
            UpdateWorldButtonPositions();

            // Load levels for the new current world and update levelBestTimeText
            WorldData currentWorldData = worlds[currentWorldIndex];
            LoadLevels(currentWorldData, worldOrganizer.GetChild(currentWorldIndex).GetComponent<Button>());

            // Show preview of the first level of the new world
            if (levelOrganizer.childCount > 0)
            {
                LevelDataHolder levelDataHolder = levelOrganizer.GetChild(0).GetComponent<LevelDataHolder>();
                if (levelDataHolder != null)
                {
                    ShowLevelPreview(levelDataHolder.levelData);
                }
            }
        }));
    }


    void UpdateLevelPreviewAfterScroll()
    {
        ShowLevelPreview(levelOrganizer.GetChild(0).GetComponent<LevelDataHolder>().levelData);
    }

    void ScrollLevels(int direction)
    {
        if (isAnimating) return;
        int targetIndex = (currentLevelIndex + direction + levelOrganizer.childCount) % levelOrganizer.childCount;
        StartCoroutine(AnimateCarousel(levelOrganizer, currentLevelIndex, targetIndex, () =>
        {
            currentLevelIndex = targetIndex;
            UpdateLevelButtonPositions();

            // Show preview of the current level and update levelBestTimeText
            LevelDataHolder levelDataHolder = levelOrganizer.GetChild(currentLevelIndex).GetComponent<LevelDataHolder>();
            
            if (levelDataHolder != null)
            {
                ShowLevelPreview(levelDataHolder.levelData);
            
                //get leaderboard for level
                previewToLeaderboard.currentLevel = levelDataHolder.levelData;
                //if on leaderboard screen update
                if (previewToLeaderboard.onLeaderboard == true) {
                    previewToLeaderboard.RefreshLeaderboard();
                } 
            }
        }));
    }


    IEnumerator AnimateCarousel(Transform container, int startIndex, int targetIndex, System.Action onComplete)
    {
        isAnimating = true;

        int itemCount = container.childCount;
        if (itemCount == 0) 
        {
            isAnimating = false;
            onComplete?.Invoke();
            yield break; // Terminate the coroutine
        }

        Vector3[] startPositions = new Vector3[itemCount];
        Vector3[] endPositions = new Vector3[itemCount];

        for (int i = 0; i < itemCount; i++)
        {
            if (i < container.childCount) // Check if the child exists
            {
                startPositions[i] = container.GetChild(i).localPosition;
                endPositions[i] = GetCarouselPosition(i - targetIndex, itemCount, 40f);
            }
        }

        float animationDuration = 0.3f;
        float elapsedTime = 0;

        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            t = t * (2 - t); // Ease-out effect

            for (int i = 0; i < itemCount; i++)
            {
                if (i < container.childCount) // Check if the child exists before accessing
                {
                    container.GetChild(i).localPosition = Vector3.Lerp(startPositions[i], endPositions[i], t);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Correctly yielding execution
        }

        onComplete?.Invoke();
        isAnimating = false;
    }

    void UpdateWorldButtonPositions()
    {
        float baseOpacity = 0.05f; // Minimum opacity for buttons farthest from the center
        float maxDistance = 3; // The maximum distance from the center button

        for (int i = 0; i < worldOrganizer.childCount; i++)
        {
            Transform child = worldOrganizer.GetChild(i);
            int distanceFromCenter = Mathf.Abs(i - currentWorldIndex);
            float opacity = Mathf.Lerp(1, baseOpacity, (float)distanceFromCenter / maxDistance);
            SetButtonOpacity(child.GetComponent<Button>(), opacity);
            Vector3 targetPosition = GetCarouselPosition(i - currentWorldIndex, worldOrganizer.childCount, 40f); // Adjust spacing to 40f
            child.localPosition = targetPosition;
        }
    }

    void UpdateLevelButtonPositions()
    {
        float baseOpacity = 0.05f; // Minimum opacity for buttons farthest from the center
        float maxDistance = 3; // The maximum distance from the center button

        for (int i = 0; i < levelOrganizer.childCount; i++)
        {
            Transform child = levelOrganizer.GetChild(i);
            int distanceFromCenter = Mathf.Abs(i - currentLevelIndex);
            float opacity = Mathf.Lerp(1, baseOpacity, (float)distanceFromCenter / maxDistance);
            SetButtonOpacity(child.GetComponent<Button>(), opacity);

            Vector3 targetPosition = GetCarouselPosition(i - currentLevelIndex, levelOrganizer.childCount, 40f); // Adjust spacing to 40f
            child.localPosition = targetPosition;
        }
    }

    Vector3 GetCarouselPosition(int index, int count, float spacing)
    {
        return new Vector3(0, -index * spacing, 0);
    }

    void ShowLevelPreview(LevelData level)
    {
        if (gridPreviewCam.levelData == level)
        {
            // The level preview is already showing this level, no need to do anything
            return;
        }

        // Set the level name and best time
        levelNameText.text = level.levelName;
        if (level.bestTime != float.MaxValue)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(level.bestTime);
            int milliseconds = (int)(timeSpan.Milliseconds / 10); // Convert to two digits
            levelBestTimeText.text = string.Format("Best: {0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, milliseconds);
        }
        else
        {
            levelBestTimeText.text = "Best: --:--:--";
        }

        // Assign the level data
        gridPreviewCam.levelData = level;
        // Removed: gridPreviewCam.SetCameraPosition();
        gridPreview.goal = level;

        // Initialize the grid preview
        gridPreview.InitializeGridPreview(level);
        
        previewToLeaderboard.currentLevel = level;

        //Set transition color
        transitionColor.SetColor(level.tileColorTop);
        UITransitionMat.SetColor("_startColor", transitionColor.GetColor());  

        // Set the static variable for persistence when selecting
        loaded = level;
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