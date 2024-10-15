using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapResizer : MonoBehaviour
{
    public PlayerInputHandler playerInput; 
    private bool inMinimapToggle = false;
    public Animator minimapAnimator;

    void OnEnable() {
        playerInput.OnMinimapInput += ToggleMinimapResize;
    }

    void OnDisable() {
        playerInput.OnMinimapInput -= ToggleMinimapResize;
    }

    private void ToggleMinimapResize() {
        StartCoroutine(ResizeMinimap());
    }
    private IEnumerator ResizeMinimap(){
        if (inMinimapToggle == false){
            inMinimapToggle = true;
            if (minimapAnimator != null)
            {
                // Trigger the animation
                minimapAnimator.SetTrigger("resizeMinimap");
                // Wait for the duration of the animation
                yield return new WaitForSeconds(minimapAnimator.GetCurrentAnimatorStateInfo(0).length);
            }
            inMinimapToggle = false;
        }
    }
}
