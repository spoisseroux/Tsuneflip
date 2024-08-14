using UnityEngine;
using UnityEngine.UI;

public class CropRawImage : MonoBehaviour
{
    public RawImage rawImage;  // Reference to the RawImage component
    public float cropAmount;  // Amount to crop from the bottom (0 to 1)

    void Start()
    {
        if (rawImage == null)
        {
            Debug.LogError("RawImage component is not assigned.");
            return;
        }

        // Ensure cropAmount is within valid range
        cropAmount = Mathf.Clamp01(cropAmount);

        // Adjust the UV Rect to crop the bottom part of the image
        Rect currentUVRect = rawImage.uvRect;
        currentUVRect.yMin += cropAmount;  // Crop the bottom part
        rawImage.uvRect = currentUVRect;
    }
}