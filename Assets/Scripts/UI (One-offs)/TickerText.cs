using UnityEngine;
using TMPro;

public class TickerText : MonoBehaviour
{
    public float scrollSpeed = 50f;
    private RectTransform rectTransform;
    private float startPosX;
    private float textWidth;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosX = rectTransform.localPosition.x;
        textWidth = rectTransform.rect.width;
    }

    void Update()
    {
        rectTransform.localPosition += Vector3.left * scrollSpeed * Time.deltaTime;

        if (rectTransform.localPosition.x < -textWidth)
        {
            rectTransform.localPosition = new Vector3(startPosX, rectTransform.localPosition.y, rectTransform.localPosition.z);
        }
    }
}