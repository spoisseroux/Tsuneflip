using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapResizer : MonoBehaviour
{
    public PlayerInputHandler playerInput; 
    private bool inMinimapToggle = false;
    public Animator minimapAnimator;
    public Material minimapMaterial;

    void Start() {
        Color currentColor = minimapMaterial.GetColor("_Color");
        currentColor.a = 0;
        minimapMaterial.SetColor("_Color", currentColor);
    }

    void OnEnable() {
        playerInput.OnMinimapInput += ToggleMinimapResize;
    }

    void OnDisable() {
        playerInput.OnMinimapInput -= ToggleMinimapResize;
    }

    private void ToggleMinimapResize() {
        StartCoroutine(ResizeMinimap());
    }
    private IEnumerator ResizeMinimap()
    {
        if (inMinimapToggle == false)
        {
            inMinimapToggle = true;

            if (minimapAnimator != null)
            {
                // Trigger the animation
                minimapAnimator.SetTrigger("resizeMinimap");

                // Wait for a very short time to ensure the animator starts the transition
                yield return null;

                // Get the animation duration after the state transition has started
                float animationDuration = minimapAnimator.GetCurrentAnimatorStateInfo(0).length;

                // Retrieve the current color of the material
                Color currentColor = minimapMaterial.GetColor("_Color");
                float targetAlpha = currentColor.a == 0 ? 175f / 255f : 0f; // Target alpha based on current alpha

                // Animate the alpha change over the animation duration
                float elapsedTime = 0f;
                float initialAlpha = currentColor.a;

                while (elapsedTime < animationDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float newAlpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsedTime / animationDuration);
                    
                    // Set the updated alpha value
                    currentColor.a = newAlpha;
                    minimapMaterial.SetColor("_Color", currentColor);

                    yield return null; // Wait for the next frame
                }

                // Ensure the final alpha is correctly set
                currentColor.a = targetAlpha;
                minimapMaterial.SetColor("_Color", currentColor);

                // Wait for the animation to finish if it's still running
                yield return new WaitForSeconds(animationDuration - elapsedTime);
            }

            inMinimapToggle = false;
        }
    }
}
