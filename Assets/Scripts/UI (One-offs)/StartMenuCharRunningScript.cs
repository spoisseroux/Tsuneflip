using UnityEngine;

public class StartMenuCharRunningScript : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("No Animator component found on this GameObject!");
        }
        SetRunningState();
    }

    public void SetRunningState()
    {
        if (animator != null)
        {
            animator.SetBool("isIdle", false);
            // Set the isRunning parameter to true
            animator.SetBool("isRunning", true);
            // Set the isIdle parameter to false

        }
    }

    public void PlayFootstep() {
        //Suppress warning
    }

    public void PlayOtherFootstep() {
        //Suppress warning
    }
}
