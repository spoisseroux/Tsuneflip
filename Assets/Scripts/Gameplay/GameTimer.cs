using System;
using System.Collections;
using UnityEngine;
using TMPro;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Rendering;

public class GameTimer : MonoBehaviour
{
    public LevelManager levelManager;
    public LevelData levelData;
    public TextMeshProUGUI timerText;

    private float startTime;
    private float stopTime;
    private float elapsedTime;
    private float currentTime;
    private bool isRunning = false;
    public float animationDuration = 0.5f;

    private EventInstance playTimeAnimation;


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
        stopTime = Time.time;
        isRunning = false;
        elapsedTime = stopTime - startTime;
    }

    public float GetElapsedTime(){
        return elapsedTime;
    }

    public string GetTime()
    {
        currentTime = Time.time - startTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
        int milliseconds = (int)(timeSpan.Milliseconds / 10); // Convert to two digits
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, milliseconds);
    }

    public string GetTimeResult()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
        int milliseconds = (int)(timeSpan.Milliseconds / 10); // Convert to two digits
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, milliseconds);
    }

    public string GetBestTime()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(levelData.bestTime);
        int milliseconds = (int)(timeSpan.Milliseconds / 10); // Convert to two digits
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, milliseconds);
    }


    public bool UpdateBestTime()
    {
        if (elapsedTime < levelData.bestTime)
        {
            levelData.bestTime = elapsedTime;
            // Save changes to the ScriptableObject instance
            SaveLevelData(levelData);
            return true;
        }
        else {
            return false;
        }
    }

    //Make sure besttime is actually saved from static var to real leveldata
    private void SaveLevelData(LevelData levelData)
    {
        #if UNITY_EDITOR
        // Mark the levelData as dirty to ensure it gets saved in the Editor
        UnityEditor.EditorUtility.SetDirty(levelData);
        #endif
    }

    private void Update()
    {
        if (isRunning)
        {
            timerText.text = GetTime();
        }
    }

    public IEnumerator AnimateTimeResult(string finalFormattedTime, TextMeshProUGUI textToUpdate)
    {
        // Parse the final formatted time string
        string[] timeParts = finalFormattedTime.Split(':');
        int finalMinutes = int.Parse(timeParts[0]);
        int finalSeconds = int.Parse(timeParts[1]);
        int finalMilliseconds = int.Parse(timeParts[2]);

        float finalTimeInSeconds = finalMinutes * 60 + finalSeconds + finalMilliseconds / 100f;
        float timer = 0f;

        playTimeAnimation = FMODUnity.RuntimeManager.CreateInstance("event:/PlayTimeAnimation");
        playTimeAnimation.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        while (timer < animationDuration)
        {
            playTimeAnimation.start();
            timer += Time.deltaTime;
            float animatedTime = Mathf.Lerp(0, finalTimeInSeconds, timer / animationDuration);
            textToUpdate.text = FormatTime(animatedTime);
            yield return null;
        }

        // Ensure the final value is exactly the final formatted time
        textToUpdate.text = finalFormattedTime;
        playTimeAnimation.release();
    }

    private string FormatTime(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        int milliseconds = (int)(timeSpan.Milliseconds / 10); // Convert to two digits
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, milliseconds);
    }
}
