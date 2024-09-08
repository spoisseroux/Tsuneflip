using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Rendering.Universal;

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
    public TransitionManager transitioner;
    public GameTimer gameTimer;
    public CountdownFinishText countdownText;
    public Material skyboxMaterial;
    public LevelGoalPreview gridPreviewManager;
    [SerializeField] private PreviewCameraController previewCameraController;

    //Results Screen
    public GameObject resultsScreen;
    public GameObject endScreenBackground;

    private TextMeshProUGUI resultsLevelNameText;
    private TextMeshProUGUI resultsTimeText;
    private TextMeshProUGUI resultsBestTimeText;
    private TextMeshProUGUI resultsRankText;
    private RainbowTextEffect resultsBestTimeRainbowEffect;
    private bool isNewRecord = false;
    public Material resultsCubemapMat;
    public Material defaultSkybox;

    public GameObject gridPreviewImage;
    private Color resultsTextColor;
    public Camera resultsCamera;
    public Canvas uiCanvas;

    // Enemy objects

    //TODO: FMod
    public LevelMusicManager levelMusicManager;
    public SceneChangeManager sceneChangerManager;

    private FMOD.Studio.EventInstance playDeath;
    private FMOD.Studio.EventInstance playRank;
    private FMOD.Studio.EventInstance playNewRecord;
    //public LevelMusicHandler levelMusic;
    public RankCalculator rankCalculator;
    [SerializeField] UniversalRendererData feature;
    public LeaderboardManager leaderboard;
    public TMP_InputField leaderboardUsername;
    public TextMeshProUGUI leaderboardInputTime;


    private void Awake()
    {
        Debug.Log("Level manager Awake");
        //Get Results Screen Texts
        FModStarter();
        resultsLevelNameText = resultsScreen.transform.Find("ResultsScreen/LevelName").GetComponent<TextMeshProUGUI>();
        resultsTimeText = resultsScreen.transform.Find("ResultsScreen/InfoHolder/TimeHolder/Time").GetComponent<TextMeshProUGUI>();
        resultsBestTimeText = resultsScreen.transform.Find("ResultsScreen/InfoHolder/BestTimeHolder/BestTime").GetComponent<TextMeshProUGUI>();
        resultsBestTimeRainbowEffect = resultsBestTimeText.GetComponent<RainbowTextEffect>();
        resultsRankText = resultsScreen.transform.Find("ResultsScreen/InfoHolder/RankHolder/Rank").GetComponent<TextMeshProUGUI>();
        resultsScreen.SetActive(false);
        endScreenBackground.SetActive(false);
        /*
         * Need to make a refactor when it's clear how many of these can be Serialized 
         * and how many need to be instantiated/built at runtime, then found
         */
        CursorManager.LockCursor();
        level = LevelMenuManager.loaded; // STATIC VARIABLE, READ FOR PERSISTENT MEMORY ACROSS SCENES

        // find Player, get components
        player = GameObject.Find("Player");
        playerDamage = player.GetComponent<PlayerDamage>();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.ToggleMovementInput(true); // turn off input, will be turned on again when timer hits go

        //TURN ON OUTLINE
        feature.rendererFeatures[1].SetActive(true);

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

    void FModStarter() {
        playDeath = FMODUnity.RuntimeManager.CreateInstance("event:/PlayDeath");
        playDeath.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        playRank = FMODUnity.RuntimeManager.CreateInstance("event:/PlayRank");
        playRank.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        playNewRecord = FMODUnity.RuntimeManager.CreateInstance("event:/PlayNewRecord");
        playNewRecord.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    private void OnDisable()
    {
        grid.OnGridMatch -= HandleLevelWin;
        playerDamage.OnLivesNumberChange -= CheckGameOver;
        playerInput.OnPauseInput -= TogglePauseMenu;
    }

    public void TogglePauseMenu(bool pause)
    {
        Debug.Log("we r pausing !");
        // toggle back and forth
        pauseMenuUI.SetActive(!pauseMenuUI.activeSelf);
        if (!pause)
        {
            //TURN ON OUTLINE
            feature.rendererFeatures[1].SetActive(true);
            CursorManager.LockCursor();
            Time.timeScale = 1;
        }
        else
        {
            //TURN OFF OUTLINE
            feature.rendererFeatures[1].SetActive(false);
            CursorManager.UnlockCursor();
            Time.timeScale = 0;
        }
    }

    private void CheckGameOver(IDealDamage source, int livesLeft)
    {
        Debug.Log("Checking game over");
        playDeath.start();
        playDeath.release();
        //TODO: Just reloading the scene on death for now
        sceneChangerManager.SceneChange("LevelScene"); //Just reload the level
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

        levelMusicManager.PlayEvent("event:/PlayLevelMusic");

        //Start Timer once countdown finishes
        playerMovement.ToggleMovementInput(false);
        gameTimer.StartTimer();

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
        //levelMusic.fadeOutDuration = 0.1f;
        //levelMusic.FadeOutAndStop();
        levelMusicManager.StopEvent();
        playerMovement.ToggleMovementInput(true);
        gameTimer.StopTimer();
        yield return StartCoroutine(countdownText.FinishCoroutine());
        yield return transitioner.ExitTransition();
        PauseCamera();

        //TURN OFF OUTLINE
        feature.rendererFeatures[1].SetActive(false);

        endScreenBackground.SetActive(true);
        resultsScreen.SetActive(true);

        //TURN OFF PREVIEW
        //TURN OFF TIMER TEXT
        resultsCubemapMat.SetTexture("_CubemapCurr", level.cubemap);
        resultsCubemapMat.SetColor("_ColorCurr", level.cubemapColor);
        CursorManager.UnlockCursor();
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
        resultsBestTimeText.color = resultsTextColor;

        //BEST TIME/NEW RECORD HANDLER
        if (gameTimer.UpdateBestTime() == true) {
            isNewRecord = true;
        }
        yield return StartCoroutine(gameTimer.AnimateTimeResult(gameTimer.GetBestTime(), resultsBestTimeText));
        if (isNewRecord == true) { //New record
            resultsBestTimeRainbowEffect.enabled = true;
            playNewRecord.start();
            playNewRecord.release();
        }

        yield return new WaitForSeconds(0.3f);
        
        playRank.start();
        playRank.release();
        resultsRankText.color = resultsTextColor;
        //TODO: Implement ranking
        resultsRankText.text = rankCalculator.CalculateRank(level, gameTimer.GetElapsedTime());
        RankCalculator.ResetFlips();
        RankCalculator.ResetMinFlips();

        //TODO: handle leaderboard
        
        //show rank
        //show buttons
    }

    private void HandleLevelLoss()
    {
        // stop timer
        // play things
        // return to menu
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

    public void DestroyAllInstancesByTag(string tag)
    {
        GameObject[] instances = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject instance in instances)
        {
            Destroy(instance);
        }
    }

    public void openLeaderboard() {
        //add time to leaderboard input box
        StartCoroutine(leaderboard.GetLeaderboard(level.levelId));
        leaderboardInputTime.text = gameTimer.GetTimeResult();
    }

    public void onAddToBoard()
    {
        // Get the input from the input field


        string userInput = leaderboardUsername.text;
        StartCoroutine(leaderboard.SubmitScore(level.levelId, userInput, gameTimer.GetElapsedTime(), gameTimer.GetTimeResult()));
        //StartCoroutine(leaderboard.GetLeaderboard(level.levelId));
        // Do something with the input
        Debug.Log("User input: " + userInput);
    }
}
