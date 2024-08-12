using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    //public LevelMusicHandler levelMusic;
    public TransitionHandler transitioner;
    private FMOD.Studio.EventInstance playLevelMusic;
    // Start is called before the first frame update
    void Start()
    {
        playLevelMusic = FMODUnity.RuntimeManager.CreateInstance("event:/PlayStartMenuMusic");
        playLevelMusic.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        playLevelMusic.start();

        //levelMusic.PlayEvent("event:/PlayStartMenuMusic");
        UnlockCursor();
    }

    public void toLevelMenu()
    {
        //levelMusic.FadeOutAndStop();
        playLevelMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        playLevelMusic.release();
        Time.timeScale = 1;
        StartCoroutine(toLevelMenuCoroutine());
    }

    private IEnumerator toLevelMenuCoroutine()
    {
        yield return transitioner.ExitTransition();
        Debug.Log("Going back to level select");
        //TODO: Change scene name
        SceneManager.LoadScene("LevelMenu");
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
