using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public LevelMusicManager levelMusicManager;
    public TransitionManager transitioner;
    public LevelMenuManager levelMenuManager;

    public void SceneChange(string sceneName) {
        if (levelMenuManager != null){
            // Store the current world and level index in PlayerPrefs
            PlayerPrefs.SetInt("LastWorldIndex", levelMenuManager.currentWorldIndex);
            Debug.Log("LastWorldIndex: " + levelMenuManager.currentWorldIndex);
            PlayerPrefs.SetInt("LastLevelIndex", levelMenuManager.currentLevelIndex);
            Debug.Log("LastLevelIndex: " + levelMenuManager.currentLevelIndex);
            PlayerPrefs.Save(); // Save the changes
        }
        StartCoroutine(LoadLevelCoroutine(sceneName));
    }

    private IEnumerator LoadLevelCoroutine(string sceneName)
    {
        yield return transitioner.ExitTransition(); //wait for exit transition to finish
        levelMusicManager.StopEvent();
        Debug.Log("Loading level: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
