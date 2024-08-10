using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class StartButtonFade : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Material startButtonMat;
    public string fadeProperty = "_Fade"; // Assuming the property is named "_Fade" in the shader
    public float hoverFadeTime = 0.5f;
    public float leaveFadeTime = 0.5f;

    private Coroutine fadeCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Fade in
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(Fade(1f, hoverFadeTime));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Fade out
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(Fade(0f, leaveFadeTime));
    }

    private IEnumerator Fade(float targetFade, float duration)
    {
        float currentFade = startButtonMat.GetFloat(fadeProperty);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float fadeValue = Mathf.Lerp(currentFade, targetFade, time / duration);
            startButtonMat.SetFloat(fadeProperty, fadeValue);
            yield return null;
        }

        // Ensure the final value is set
        startButtonMat.SetFloat(fadeProperty, targetFade);
    }
}