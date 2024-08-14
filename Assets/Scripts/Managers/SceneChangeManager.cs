using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public LevelMusicManager levelMusicManager;
    public TransitionManager transitioner;

    public void SceneChange(string sceneName) {
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
