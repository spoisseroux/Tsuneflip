using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public LevelManager levelManager;
    public LevelData levelData;
    public TextMeshProUGUI timerText;

    private float startTime;
    private float elapsedTime;
    private bool isRunning = false;

    public void Start()
    {
        Debug.Log("GameTimer Start method called.");
        StartCoroutine(StartInit());
    }

    private IEnumerator StartInit()
    {
        Debug.Log("In startinit");
        yield return new WaitForSeconds(0.1f); // Adjust delay as necessary
        InitializeTimer();
    }

    private void InitializeTimer()
    {
        Debug.Log("in init timer");

        levelData = levelManager.level;
        Debug.Log(levelData);
    }


    public void StartTimer()
    {
        startTime = Time.time;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
        UpdateBestTime();
    }

    public string GetTime()
    {
        elapsedTime = Time.time - startTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
        return string.Format("{0:D2}:{1:D2}:{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
    }

    public void UpdateBestTime()
    {
        if (elapsedTime < levelData.bestTime)
        {
            levelData.bestTime = elapsedTime;
        }
    }

    public string GetBestTime()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(levelData.bestTime);
        return string.Format("{0:D2}:{1:D2}:{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
    }

    private void Update()
    {
        if (isRunning)
        {
            timerText.text = GetTime();
        }
    }
}
