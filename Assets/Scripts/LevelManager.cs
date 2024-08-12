using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

// maybe a singleton? in case we actually load new scenes and instantiate everything
public class LevelManager : MonoBehaviour
{
    // Player objects
    public CinemachineFreeLook playerCamera;
    [SerializeField] GameObject player;
    [SerializeField] PlayerDamage playerDamage;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerInputHandler playerInput;

    // Gameplay UI objects
    //[SerializeField] GameObject gameplayUI;
    [SerializeField] TextMeshProUGUI levelNameText;
    [SerializeField] TextMeshProUGUI livesRemainingText;
    [SerializeField] TextMeshProUGUI timerText;

    // Pause Menu UI objects
    [SerializeField] GameObject pauseMenuUI;

    // Time object
    // have to create this

    // Grid objects
    [SerializeField] GridManager grid;
    [SerializeField] public LevelData level;
    [SerializeField] Vector3 respawnPosition;
    [SerializeField] Vector3 respawnRotation; // quaternion? :/
    [SerializeField] DeathZone deathZone;

    //UI
    public TransitionHandler transitioner;
    public GameTimer gameTimer;
    public CountdownFinishText countdownText;
    public Material skyboxMaterial;
    public LevelGoalPreview gridPreviewManager;
    [SerializeField] private PreviewCameraController previewCameraController;

    //Results Screen
    public GameObject resultsScreen;

    private TextMeshProUGUI resultsLevelNameText;
    private TextMeshProUGUI resultsTimeText;
    private TextMeshProUGUI resultsBestTimeText;
    private TextMeshProUGUI resultsRankText;
    public Material resultsCubemapMat;
    public Material defaultSkybox;

    public GameObject gridPreviewImage;
    private Color resultsTextColor;
    public Camera resultsCamera;
    public Canvas uiCanvas;

    // Enemy objects

    //TODO: FMod

    private FMOD.Studio.EventInstance playDeath;
    private FMOD.Studio.EventInstance playRank;
    public LevelMusicHandler levelMusic;


    void FModStarter() {
        playDeath = FMODUnity.RuntimeManager.CreateInstance("event:/PlayDeath");
        playDeath.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    

        playRank = FMODUnity.RuntimeManager.CreateInstance("event:/PlayRank");
        playRank.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    void Update()
    {
        //Quick fix to freeze player on win sorry
    }

    private void Awake()
    {
        Debug.Log("Level manager Awake");
        //Get Results Screen Texts
        FModStarter();
        resultsLevelNameText = resultsScreen.transform.Find("LevelName").GetComponent<TextMeshProUGUI>();
        resultsTimeText = resultsScreen.transform.Find("InfoHolder/TimeHolder/Time").GetComponent<TextMeshProUGUI>();
        resultsBestTimeText = resultsScreen.transform.Find("InfoHolder/BestTimeHolder/BestTime").GetComponent<TextMeshProUGUI>();
        resultsRankText = resultsScreen.transform.Find("InfoHolder/RankHolder/Rank").GetComponent<TextMeshProUGUI>();
        resultsScreen.SetActive(false);
        /*
         * Need to make a refactor when it's clear how many of these can be Serialized 
         * and how many need to be instantiated/built at runtime, then found
         */
        LockCursor();
        level = LevelMenuManager.loaded; // STATIC VARIABLE, READ FOR PERSISTENT MEMORY ACROSS SCENES

        // find Player, get components
        player = GameObject.Find("Player");
        playerDamage = player.GetComponent<PlayerDamage>();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.ToggleMovementInput(true); // turn off input, will be turned on again when timer hits go

        // read this from leveldata later i guess
        respawnPosition = new Vector3(0.5f, 1.5f, 0.5f);
        respawnRotation = new Vector3(0.5f, 0f, 0.5f);

        // find Grid, get components
        grid = GameObject.Find("Grid").GetComponent<GridManager>();
        grid.InitializeLevelData(level);
        grid.InitializeGrid();

        // find Gameplay UI, get components
        //gameplayUI = GameObject.Find("GameplayUI");
        // levelNameText.text = "Level " + level.levelName;
        //livesRemainingText.text = playerDamage.GetLives().ToString();

        // find PauseMenu UI and set false
        pauseMenuUI = GameObject.Find("PauseMenuUI");
        pauseMenuUI.SetActive(false);

        // subscribe to events
        grid.OnGridMatch += HandleLevelWin;
        playerDamage.OnLivesNumberChange += CheckGameOver;
        playerInput.OnPauseInput += TogglePauseMenu;

        //START LEVEL
        StartLevel();
    }

    private void OnDisable()
    {
        grid.OnGridMatch -= HandleLevelWin;
        playerDamage.OnLivesNumberChange -= CheckGameOver;
        playerInput.OnPauseInput -= TogglePauseMenu;
    }

    private void TogglePauseMenu(bool pause)
    {
        Debug.Log("we r pausing !");
        // toggle back and forth
        pauseMenuUI.SetActive(!pauseMenuUI.activeSelf);
        if (!pause)
        {
            LockCursor();
            Time.timeScale = 1;
        }
        else
        {
            UnlockCursor();
            Time.timeScale = 0;
        }
    }

    private void CheckGameOver(IDealDamage source, int livesLeft)
    {
        Debug.Log("Checking game over");
        playDeath.start();
        playDeath.release();
        levelMusic.FadeOutAndStop();
        StartCoroutine(retryLevelCoroutine()); //Just reload the level
        /*
        livesRemainingText.text = livesLeft.ToString(); // push lives to UI
        if (livesLeft <= 0)
        {
            //HandleLevelLoss();
            RespawnPlayer();
        }
        else
        {
            // spawn depending on source of damage???
            RespawnPlayer();
        }
        */

    }

    private void StartLevel()
    {
        Debug.Log("in startlevel");
        StartCoroutine(StartLevelCoroutine());
    }

    private IEnumerator StartLevelCoroutine()
    {
        Debug.Log("in startlevelcoroutine");
        gridPreviewManager.goal = level;
        //gridPreviewCamera.levelData = level;

        Debug.Log("calling init grid preview");
        gridPreviewManager.InitializeLevelGridPreview(level, previewCameraController);

        //playerMovement.ToggleMovementInput(true);
        skyboxMaterial.SetTexture("_Cubemap", level.cubemap);
        skyboxMaterial.SetColor("_Color", level.cubemapColor);


        //Wait 2 Seconds
        yield return new WaitForSeconds(2f);

        //Check if transitioner is not in transition

        //Play Countdown Timer
        Debug.Log("before countdown");
        yield return StartCoroutine(countdownText.CountdownCoroutine());
        Debug.Log("Countdown finished");

        levelMusic.PlayEvent("event:/PlayLevelMusic");

        //Start Timer once countdown finishes
        playerMovement.ToggleMovementInput(false);
        gameTimer.StartTimer();

        //playLevelMusic.release();

    }

    private void RespawnPlayer()
    {

        // disable playermovement for a coroutine?

        // play a shader to simulate fading into position

        // change player state?

        // spawn in player
        Debug.Log("position player");
        // 1. move to origin
        // 2. set desired rotation
        // 3. move to desired spawn location
        playerMovement.ApplySpawnPosition(respawnPosition, respawnRotation);

        // re-enable playermovement
    }

    private void HandleLevelWin()
    {
        // stop timer
        playerMovement.enabled = false;
        StartCoroutine(HandleLevelWinCoroutine());
        // play things
        // return to menu
    }

    private IEnumerator HandleLevelWinCoroutine()
    {
        levelMusic.fadeOutDuration = 0.1f;
        levelMusic.FadeOutAndStop();
        playerMovement.ToggleMovementInput(true);
        gameTimer.StopTimer();
        yield return StartCoroutine(countdownText.FinishCoroutine());
        yield return transitioner.ExitTransition();
        PauseCamera();
        resultsScreen.SetActive(true);
        ResetSky();
        //TURN OFF PREVIEW
        //TURN OFF TIMER TEXT
        resultsCubemapMat.SetTexture("_CubemapCurr", level.cubemap);
        resultsCubemapMat.SetColor("_ColorCurr", level.cubemapColor);
        UnlockCursor();
        timerText.enabled = false;
        gridPreviewImage.SetActive(false);
        resultsCamera.enabled = true;
        uiCanvas.worldCamera = resultsCamera;

        DestroyAllInstancesByTag("Tile");
        yield return transitioner.EnterTransition();

        resultsTextColor = level.tileColorTop;
        resultsTextColor.a = 1f;

        resultsLevelNameText.text = level.levelName;
        resultsTimeText.color = resultsTextColor;
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(gameTimer.AnimateTimeResult(gameTimer.GetTimeResult(), resultsTimeText));
        yield return new WaitForSeconds(0.3f);
        gameTimer.UpdateBestTime();
        resultsBestTimeText.color = resultsTextColor;
        yield return StartCoroutine(gameTimer.AnimateTimeResult(gameTimer.GetBestTime(), resultsBestTimeText));
        yield return new WaitForSeconds(0.3f);
        playRank.start();
        playRank.release();
        resultsRankText.color = resultsTextColor;
        resultsRankText.text = "B";
        //STEP THRU EACH RESULT TEXT BOX
        //Set level name
        //show time, count up for style?
        //show best time, is new record?
        //show rank
        //show buttons
    }

    private void HandleLevelLoss()
    {
        // stop timer
        // play things
        // return to menu
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void backToLevelSelect()
    {
        levelMusic.FadeOutAndStop();
        Time.timeScale = 1;
        StartCoroutine(backToLevelSelectCoroutine());
    }

    public void retryLevel()
    {
        levelMusic.FadeOutAndStop();
        Time.timeScale = 1;
        StartCoroutine(retryLevelCoroutine());
    }

    private IEnumerator backToLevelSelectCoroutine()
    {
        yield return transitioner.ExitTransition();
        Debug.Log("Going back to level select");
        //TODO: Change scene name
        SceneManager.LoadScene("LevelMenu");
    }

    private IEnumerator retryLevelCoroutine()
    {
        yield return transitioner.ExitTransition();
        Debug.Log("Retrying Level");
        //TODO: Change scene name
        SceneManager.LoadScene("SpencerGridTesting 1");
    }

    public void PauseCamera()
    {
        if (playerCamera != null)
        {
            // Set the speed to 0 to stop camera movement
            playerCamera.m_XAxis.m_MaxSpeed = 0f;
            playerCamera.m_YAxis.m_MaxSpeed = 0f;
        }
    }

    void ResetSky()
    {
        // Reset Skybox to Unity's default procedural skybox
        //Material defaultSkybox = RenderSettings.defaultSkybox;
        if (defaultSkybox != null)
        {
            RenderSettings.skybox = defaultSkybox;
            Debug.Log("Skybox set to Unity's default procedural skybox.");
        }
        else
        {
            Debug.LogError("Default procedural skybox material not found.");
        }

        // Reset ambient lighting to default
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
        RenderSettings.ambientIntensity = 1.0f;
        RenderSettings.ambientSkyColor = Color.white;
        RenderSettings.ambientEquatorColor = Color.gray;
        RenderSettings.ambientGroundColor = Color.black;
        RenderSettings.ambientLight = Color.white;

        // Reset reflection settings to default
        RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;
        RenderSettings.defaultReflectionResolution = 128;
        RenderSettings.reflectionBounces = 1;
        RenderSettings.reflectionIntensity = 1.0f;

        // Reset fog settings to default
        RenderSettings.fog = false;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.01f;
        RenderSettings.fogStartDistance = 0.0f;
        RenderSettings.fogEndDistance = 300.0f;

        Debug.Log("Lighting and Skybox settings reset to default.");
    }

    public void DestroyAllInstancesByTag(string tag)
    {
        GameObject[] instances = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject instance in instances)
        {
            Destroy(instance);
        }
    }
}
