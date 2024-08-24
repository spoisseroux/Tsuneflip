using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    //public LevelMusicHandler levelMusic;
    public SceneChangeManager sceneChangeManager;
    public LevelMusicManager levelMusicManager;
    private bool isRunning = false;
    private bool onStart = true;
    public Button optionsButton;
    public Button doneButton;
    public Animator animator; 
    private FMOD.Studio.EventInstance toOptionsSound;
    private FMOD.Studio.EventInstance toStartSound;
    // Start is called before the first frame update
    void Start()
    {
        //Start Level Music
        levelMusicManager.PlayEvent("event:/PlayStartMenuMusic");
        CursorManager.UnlockCursor();

        toOptionsSound = FMODUnity.RuntimeManager.CreateInstance("event:/SwipeForward");
        toOptionsSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        toStartSound = FMODUnity.RuntimeManager.CreateInstance("event:/SwipeBackward");
        toStartSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    public void PlayAnimation()
    {
        StartCoroutine(PlayAnimationCoroutine());
    }

    public IEnumerator PlayAnimationCoroutine() {
        if (!isRunning) {
            isRunning = true;
            optionsButton.interactable = false;
            doneButton.interactable = false;
            if (onStart) {
                toOptionsSound.start();
                //toLeaderboardSound.release();
                animator.SetTrigger("ToOptionsTrigger");
                // Wait for the Animator to be in the correct state
                while (!animator.GetCurrentAnimatorStateInfo(0).IsName("StartScreenToOptions"))
                {
                    yield return null; // Wait for the next frame
                }
                // Wait until the animation is no longer playing
                while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                {
                    yield return null; // Wait for the next frame
                }
                Debug.Log("Animation finished!");
                // Animation has finished, you can proceed with other actions
                onStart = false;
            }
            else {
                toStartSound.start();
                //toPreviewSound.release();
                animator.SetTrigger("ToStartTrigger");
                // Wait for the Animator to be in the correct state
                while (!animator.GetCurrentAnimatorStateInfo(0).IsName("OptionsToStartScreen"))
                {
                    yield return null; // Wait for the next frame
                }
                // Wait until the animation is no longer playing
                while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                {
                    yield return null; // Wait for the next frame
                }
                onStart = true;

            }
        }
        isRunning = false;
        optionsButton.interactable = true;
        doneButton.interactable = true;
    }
}
