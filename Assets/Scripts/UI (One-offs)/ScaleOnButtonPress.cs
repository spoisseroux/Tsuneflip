using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScaleOnButtonPress : MonoBehaviour
{
    private Vector3 originalScale;
    private bool isAnimating = false;

    void Start()
    {
        originalScale = transform.localScale;
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonPress);
        }
    }

    void OnButtonPress()
    {
        if (!isAnimating)
        {
            StartCoroutine(ScaleAnimation());
        }
    }

    IEnumerator ScaleAnimation()
    {
        isAnimating = true;
        float duration = 0.12f;
        float elapsedTime = 0f;

        // Scale down
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t = Mathf.SmoothStep(0, 1, t); // Easing in and out
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 0.9f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale * 0.9f;

        // Reset time and scale back up
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t = Mathf.SmoothStep(0, 1, t); // Easing in and out
            transform.localScale = Vector3.Lerp(originalScale * 0.9f, originalScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;

        isAnimating = false;
    }
}