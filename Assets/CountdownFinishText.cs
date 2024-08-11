using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownFinishText : MonoBehaviour
{
    public TextMeshProUGUI countdownText;

    private void Start()
    {
        if (countdownText == null)
        {
            countdownText = GetComponent<TextMeshProUGUI>();
        }
        countdownText.enabled = false;
        //Countdown();
    }

    public void Finish()
    {
        StartCoroutine(FinishCoroutine());
    }

    public void Countdown()
    {
        StartCoroutine(CountdownCoroutine());
    }

    public IEnumerator FinishCoroutine()
    {
        countdownText.enabled = true;
        yield return new WaitForSeconds(0.15f);

        // Set the text and initial scale and transparency
        countdownText.text = "Finish";
        countdownText.alpha = 0f;
        countdownText.transform.localScale = Vector3.one;

        float duration = 0.2f;
        float time = 0f;

        // Animate the scale down and fade in
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            countdownText.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.75f, 0.75f, 0.75f), t);
            countdownText.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // Wait for another half a second
        yield return new WaitForSeconds(1f);

    }

    public IEnumerator CountdownCoroutine()
    {
        Debug.Log("in countdown");
        countdownText.enabled = true;
        string[] countdownValues = { "3", "2", "1", "Go!" };

        foreach (string value in countdownValues)
        {
            // Wait for half a second
            yield return new WaitForSeconds(0.5f);

            // Set the text and initial scale and transparency
            countdownText.text = value;
            countdownText.alpha = 0f;
            countdownText.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            float duration = 0.5f;
            float time = 0f;

            // Animate the scale down and fade in
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                countdownText.transform.localScale = Vector3.Lerp(new Vector3(1.5f, 1.5f, 1.5f), Vector3.one, t);
                countdownText.alpha = Mathf.Lerp(0f, 1f, t);

                yield return null;
            }

            // Wait for another half a second
            yield return new WaitForSeconds(0.5f);
        }

        //Go
        yield return new WaitForSeconds(0.5f);

        float duration2 = 0.15f;
        float time2 = 0f;

        while (time2 < duration2)
        {
            time2 += Time.deltaTime;
            float t = time2 / duration2;

            countdownText.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }
        countdownText.enabled = false;
        //Finish();
    }
}