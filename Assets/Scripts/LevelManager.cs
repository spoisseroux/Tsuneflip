using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// maybe a singleton? in case we actually load new scenes and instantiate everything
public class LevelManager : MonoBehaviour
{
    // Player stuff
    [SerializeField] GameObject player;
    [SerializeField] PlayerDamage playerDamage;
    [SerializeField] PlayerMovement playerMovement;

    // UI stuff
    [SerializeField] GameObject UI;
    [SerializeField] TextMeshProUGUI levelNameText;
    [SerializeField] TextMeshProUGUI livesRemainingText;
    [SerializeField] TextMeshProUGUI timerText;

    // Time object
    // have to create this

    // Grid stuff
    [SerializeField] GridManager grid;
    [SerializeField] LevelData level;
    [SerializeField] Vector3 respawnPosition;
    [SerializeField] Quaternion respawnRotation;
    [SerializeField] DeathZone deathZone;

    // Enemy stuff

    private void Awake()
    {
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

        // find UI, get components
        UI = GameObject.Find("UI");
        levelNameText = GameObject.Find("LevelNameText").GetComponent<TextMeshProUGUI>();
        levelNameText.text = "Level " + level.levelName;
        livesRemainingText = GameObject.Find("LivesRemainingText").GetComponent<TextMeshProUGUI>();
        timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();

        // subscribe to events
        grid.OnGridMatch += HandleLevelWin;
        playerDamage.OnLivesNumberChange += CheckGameOver;
    }

    private void OnDisable()
    {
        grid.OnGridMatch -= HandleLevelWin;
        playerDamage.OnLivesNumberChange -= CheckGameOver;
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
