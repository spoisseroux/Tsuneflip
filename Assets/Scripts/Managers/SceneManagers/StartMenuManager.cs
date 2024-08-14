using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    //public LevelMusicHandler levelMusic;
    public SceneChangeManager sceneChangeManager;
    public LevelMusicManager levelMusicManager;
    // Start is called before the first frame update
    void Start()
    {
        //Start Level Music
        levelMusicManager.PlayEvent("event:/PlayStartMenuMusic");
        CursorManager.UnlockCursor();
    }
}
