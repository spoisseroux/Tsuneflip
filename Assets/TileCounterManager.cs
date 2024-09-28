using System.Collections;
using UnityEngine;

public class TileCounterManager : MonoBehaviour
{
    public LevelManager levelManager;
    public GridManager gridManager;
    public LevelGoalPreview previewGridManager;
    private LevelData level;
    private float position;
    public float yBegin = 170f;
    public float yEnd = -46f;
    public float lerpDuration = 0.5f; // Duration of the lerp animation

    private Coroutine lerpCoroutine;
    private float lastTargetY; // Store the last target Y position

    // Start is called before the first frame update
    void Start()
    {
        level = levelManager.level;
        lastTargetY = transform.localPosition.y; // Initialize last target Y to the current Y position
    }

    // Update is called once per frame
    void Update()
    {
        if (gridManager.tilesCorrect.Count != 0 && previewGridManager.numOfGoalTiles != 0)
        {
            // Calculate the normalized position and clamp it between 0 and 1
            position = Mathf.Clamp01((float)gridManager.tilesCorrect.Count / (float)previewGridManager.numOfGoalTiles);
            Debug.Log("Meter Position (Normalized): " + position);

            // Calculate the target Y position based on yBegin, yEnd, and position
            float targetY = Mathf.Lerp(yBegin, yEnd, position);

            // Check if targetY is different from the last one
            if (Mathf.Abs(targetY - lastTargetY) > Mathf.Epsilon)
            {
                lastTargetY = targetY; // Update lastTargetY to the new target

                // Stop the previous coroutine if it's running, and start a new one
                if (lerpCoroutine != null)
                {
                    StopCoroutine(lerpCoroutine);
                }

                lerpCoroutine = StartCoroutine(SmoothLerpYPosition(targetY));
            }
        }
    }

    private IEnumerator SmoothLerpYPosition(float targetY)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = transform.localPosition;

        while (elapsedTime < lerpDuration)
        {
            // Calculate the new Y position based on time
            float newY = Mathf.Lerp(startingPos.y, targetY, elapsedTime / lerpDuration);
            transform.localPosition = new Vector3(startingPos.x, newY, startingPos.z); // Apply the new position

            // Increase the elapsed time and wait for the next frame
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is exactly the targetY when finished
        transform.localPosition = new Vector3(startingPos.x, targetY, startingPos.z);
        Debug.Log("Arrow reached target Y position: " + targetY);
    }
}