using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionHandler : MonoBehaviour
{
    public GameObject transition1;
    public GameObject transition2;
    public Material transitionSphere;

    private RawImage transition1Image;
    private RawImage transition2Image;

    // Boolean to check if an animation is in progress
    public bool IsAnimating { get; private set; }

    private void Start()
    {
        transition1Image = transition1.GetComponent<RawImage>();
        transition2Image = transition2.GetComponent<RawImage>();
        EnterTransition();
    }

    public Coroutine ExitTransition()
    {
        return StartCoroutine(ExitTransitionCoroutine());
    }

    private IEnumerator ExitTransitionCoroutine()
    {
        IsAnimating = true; // Animation starts

        transition1.SetActive(true);
        SetAlpha(transition1Image, 0f);
        SetAlpha(transition2Image, 1f);
        transition2.SetActive(false);

        yield return FadeRawImage(transition1Image, 0f, 1f, 0.2f);

        transition2.SetActive(true);
        yield return FadeMaterial(transitionSphere, "_Fade", 0f, 1f, 1.2f);

        IsAnimating = false; // Animation ends
    }

    public void EnterTransition()
    {
        StartCoroutine(EnterTransitionCoroutine());
    }

    private IEnumerator EnterTransitionCoroutine()
    {
        IsAnimating = true; // Animation starts

        transition2.SetActive(true);
        SetAlpha(transition2Image, 1f);
        transition1.SetActive(true);
        SetAlpha(transition1Image, 1f);

        yield return FadeMaterial(transitionSphere, "_Fade", 1f, 0f, 1.2f);

        transition2.SetActive(false);
        yield return FadeRawImage(transition1Image, 1f, 0f, 0.2f);

        transition1.SetActive(false);

        IsAnimating = false; // Animation ends
    }

    private void SetAlpha(RawImage image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private IEnumerator FadeRawImage(RawImage image, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color color = image.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            image.color = color;
            yield return null;
        }

        color.a = endAlpha;
        image.color = color;
    }

    private IEnumerator FadeMaterial(Material material, string property, float startValue, float endValue, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            material.SetFloat(property, Mathf.Lerp(startValue, endValue, elapsedTime / duration));
            yield return null;
        }

        material.SetFloat(property, endValue);
    }
}
