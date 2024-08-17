using UnityEngine;

public class EndScreenController : MonoBehaviour
{
    public Animator animator;  // Reference to the Animator component

    public void PlayToLeaderboardSwipe()
    {
        animator.SetTrigger("ToLeaderboardSwipeTrigger");
    }

    public void PlayBackToResultsSwipe()
    {
        animator.SetTrigger("BackToResultsSwipeTrigger");
    }
}