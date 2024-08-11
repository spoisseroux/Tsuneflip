using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// maybe a singleton? in case we actually load new scenes and instantiate everything
public class LevelManager : MonoBehaviour
{
    // Player objects
    [SerializeField] GameObject player;
    [SerializeField] PlayerDamage playerDamage;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerInputHandler playerInput;

    // Gameplay UI objects
    [SerializeField] GameObject gameplayUI;
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


    // Enemy objects


    private void Awake()
    {
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

        // read this from leveldata later i guess
        respawnPosition = new Vector3(0.5f, 1.5f, 0.5f);
        respawnRotation = new Vector3(0.5f, 0f, 0.5f);

        // find Grid, get components

        grid = GameObject.Find("Grid").GetComponent<GridManager>();
        grid.InitializeLevelData(level);
        grid.InitializeGrid();

        // find Gameplay UI, get components
        gameplayUI = GameObject.Find("GameplayUI");
        // levelNameText.text = "Level " + level.levelName;

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
            Cursor.visible = false;
            Time.timeScale = 1;
        }
        else
        {
            Cursor.visible = true;
            Time.timeScale = 0;
        }
    }

    private void CheckGameOver(IDealDamage source, int livesLeft)
    {
        Debug.Log("Checking game over");
        //livesRemainingText.text = livesLeft.ToString(); push lives to UI
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
    }

    private void StartLevel()
    {
        StartCoroutine(StartLevelCoroutine());
        // PUT PREVIEW IN AWAKE()

        // Stuff to do here
        // run player spawn routine (shader, etc.)

        // enable player input

        // start timer

        // start enemy spawner routine or something idk yet

    }

    private IEnumerator StartLevelCoroutine()
    {
        //Populate goal preview
        //Load colors to tile material
        //Load cubemap
        skyboxMaterial.SetTexture("_Cubemap", level.cubemap);

        // Set the _Color shader variable
        skyboxMaterial.SetColor("_Color", level.cubemapColor);
        

        //Wait .5 Seconds
        yield return new WaitForSeconds(2f);

        //Check if transitioner is not in transition

        //Play Countdown Timer
        yield return StartCoroutine(countdownText.CountdownCoroutine());
        Debug.Log("Countdown finished");

        //Start Timer once countdown finishes

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
        // play things
        // return to menu
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
}
