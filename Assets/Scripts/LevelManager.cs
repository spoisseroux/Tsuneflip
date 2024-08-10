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
    [SerializeField] LevelData level;
    [SerializeField] Vector3 respawnPosition;
    [SerializeField] Quaternion respawnRotation;
    [SerializeField] DeathZone deathZone;

    // Enemy objects


    private void Awake()
    {
        /*
         * Need to make a refactor when it's clear how many of these can be Serialized 
         * and how many need to be instantiated/built at runtime, then found
         */

        // find Player, get components
        player = GameObject.Find("Player");
        playerDamage = player.GetComponent<PlayerDamage>();
        playerMovement = player.GetComponent<PlayerMovement>();

        // read this from leveldata later i guess
        respawnPosition = new Vector3(0.5f, 1f, 0.5f);
        respawnRotation = Quaternion.identity;

        // find Grid, get components
        level = LevelMenuManager.loaded; // STATIC VARIABLE, READ FOR PERSISTENT MEMORY ACROSS SCENES
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
        livesRemainingText.text = livesLeft.ToString();
        if (livesLeft <= 0)
        {
            HandleLevelLoss();
        }
        else
        {
            // spawn depending on source of damage???
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        // disable playermovement for a coroutine?

        // play a shader to simulate fading into position

        // spawn in player
        player.transform.position = respawnPosition;
        player.transform.rotation = respawnRotation;

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
}
