using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PreviewToLeaderboardUIController : MonoBehaviour
{
    public Animator animator;  // Reference to the Animator component
    public LeaderboardManager leaderboard;
    [HideInInspector] public LevelData currentLevel;
    [HideInInspector] public bool onLeaderboard = false;
    private bool onPreview = true;
    private bool isRunning = false;
    public Button swapButton;
    private FMOD.Studio.EventInstance toLeaderboardSound;
    private FMOD.Studio.EventInstance toPreviewSound;

    void Start() {
        toLeaderboardSound = FMODUnity.RuntimeManager.CreateInstance("event:/SwipeForward");
        toLeaderboardSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        toPreviewSound = FMODUnity.RuntimeManager.CreateInstance("event:/SwipeBackward");
        toPreviewSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    public void PlayAnimation()
    {
        StartCoroutine(PlayAnimationCoroutine());
    }

    public IEnumerator PlayAnimationCoroutine() {
        if (!isRunning) {
            isRunning = true;
            swapButton.interactable = false;
            if (onPreview) {
                toLeaderboardSound.start();
                //toLeaderboardSound.release();
                animator.SetTrigger("ToLeaderboardTrigger");
                // Wait for the Animator to be in the correct state
                while (!animator.GetCurrentAnimatorStateInfo(0).IsName("PreviewToLeaderboard"))
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
                StartCoroutine(leaderboard.GetLeaderboard(currentLevel.levelId));
                onPreview = false;
                onLeaderboard = true;
            }
            else {
                toPreviewSound.start();
                //toPreviewSound.release();
                animator.SetTrigger("ToPreviewTrigger");
                // Wait for the Animator to be in the correct state
                while (!animator.GetCurrentAnimatorStateInfo(0).IsName("LeaderboardToPreview"))
                {
                    yield return null; // Wait for the next frame
                }
                // Wait until the animation is no longer playing
                while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                {
                    yield return null; // Wait for the next frame
                }
                onLeaderboard = false;
                onPreview = true;
            }
        }
        isRunning = false;
        swapButton.interactable = true;
    }

    public void RefreshLeaderboard() {
        StartCoroutine(leaderboard.GetLeaderboard(currentLevel.levelId));
    }
}