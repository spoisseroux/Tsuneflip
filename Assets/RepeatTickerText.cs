using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InfiniteScrollingText : MonoBehaviour
{
    public TextMeshProUGUI scrollingText;
    public float scrollSpeed = 100f;
    public RectTransform viewport; // Parent RectTransform that defines the visible area

    private RectTransform textRectTransform;
    private float textWidth;
    private float viewportWidth;
    private List<RectTransform> activeTexts = new List<RectTransform>();

    private void Start()
    {
        textRectTransform = scrollingText.GetComponent<RectTransform>();
        UpdateTextWidth();
        viewportWidth = viewport.rect.width;

        // Set up the original and initial clones
        InitializeClones();
    }

    private void Update()
    {
        // Move all active texts
        foreach (var textRect in activeTexts)
        {
            textRect.anchoredPosition -= new Vector2(scrollSpeed * Time.deltaTime, 0);

            // If a text object has completely moved off-screen to the left, reposition it to the right
            if (textRect.anchoredPosition.x <= -textWidth)
            {
                RepositionText(textRect);
            }
        }
    }

    private void UpdateTextWidth()
    {
        // Calculate the accurate width, taking font size and scale into account
        textWidth = scrollingText.preferredWidth * scrollingText.transform.lossyScale.x;
    }

    private void InitializeClones()
    {
        // Add the original text to the list
        activeTexts.Add(textRectTransform);

        // Create up to three more clones for seamless looping
        for (int i = 1; i < 4; i++)
        {
            CreateTextClone(i * textWidth);
        }
    }

    private void CreateTextClone(float offsetX)
    {
        TextMeshProUGUI textClone = Instantiate(scrollingText, scrollingText.transform.parent);
        RectTransform cloneRectTransform = textClone.GetComponent<RectTransform>();

        // Set the position of the clone based on the offset from the original
        cloneRectTransform.anchoredPosition = new Vector2(textRectTransform.anchoredPosition.x + offsetX, textRectTransform.anchoredPosition.y);

        // Add the clone to the active list
        activeTexts.Add(cloneRectTransform);
    }

    private void RepositionText(RectTransform textRect)
    {
        // Find the rightmost text object
        RectTransform rightmost = activeTexts[0];
        foreach (var t in activeTexts)
        {
            if (t.anchoredPosition.x > rightmost.anchoredPosition.x)
            {
                rightmost = t;
            }
        }

        // Reposition the textRect to the right of the rightmost text
        textRect.anchoredPosition = new Vector2(rightmost.anchoredPosition.x + textWidth, textRect.anchoredPosition.y);
    }
}