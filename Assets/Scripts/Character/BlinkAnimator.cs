using UnityEngine;
using System.Collections;

public class SpriteSheetAnimator : MonoBehaviour
{
    private Material material;
    private float offsetX = 0f;
    private bool isAnimating = false;

    private void Start()
    {
        material = GetComponent<Renderer>().material;
        StartCoroutine(AnimateTextureOffset());
    }

    private IEnumerator AnimateTextureOffset()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0f, 10f));  // Wait for a random time between 0 and 10 seconds

            if (!isAnimating)
            {
                StartCoroutine(AnimateOffsetSequence());
            }
        }
    }

    private IEnumerator AnimateOffsetSequence()
    {
        isAnimating = true;

        SetOffset(0.2f);  // Jump to offset 0.2
        yield return new WaitForSeconds(0.15f); // Hold for 0.15 seconds

        SetOffset(0.4f);  // Jump to offset 0.4
        yield return new WaitForSeconds(0.15f); // Hold for 0.15 seconds

        SetOffset(0f);    // Jump back to offset 0
        yield return new WaitForSeconds(0.15f); // Hold for 0.15 seconds

        isAnimating = false;
    }

    private void SetOffset(float offsetX)
    {
        material.mainTextureOffset = new Vector2(offsetX, material.mainTextureOffset.y);
    }
}
