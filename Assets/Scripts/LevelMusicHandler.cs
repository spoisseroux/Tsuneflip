using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class LevelMusicHandler : MonoBehaviour
{
    private EventInstance eventInstance;
    public float fadeOutDuration = 1.4f; // Duration of the fade-out in seconds

    // Start the sound event
    public void PlayEvent(string eventPath)
    {
        eventInstance = RuntimeManager.CreateInstance(eventPath);
        eventInstance.start();
    }

    // Fade out and stop the event
    public void FadeOutAndStop()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float currentVolume = 1.0f;
        eventInstance.getParameterByName("Volume", out currentVolume);

        float elapsedTime = 0.0f;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeOutDuration);
            eventInstance.setParameterByName("Volume", newVolume);
            yield return null;
        }

        // Ensure the volume is set to 0 and stop the event
        eventInstance.setParameterByName("Volume", 0.0f);
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }
}