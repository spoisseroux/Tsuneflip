using UnityEngine;
using TMPro;

public class TitleTextEffect : MonoBehaviour
{
    public Material transitionMaterial;   // Reference to the transition material for face color
    public TextMeshProUGUI textMeshPro;   // Reference to the TextMeshPro component
    private Color faceColor = Color.white;            // Color for the face of the text
    private Color glowColor;  // White glow color for the glow effect
    public float glowIntensityCycleSpeed = 1.0f; // Speed of the glow intensity change
    public float minGlowIntensity = 0f;
    public float maxGlowIntensity = 0.5f;
    public float beatScaleMultiplier = 1.1f; // How much the text scales on a beat
    public float beatDuration = 5.0f; // Duration between beats
    public float scaleUpSpeed = 0.2f; // Speed at which the text scales up
    public float scaleDownSpeed = 0.05f; // Speed at which the text scales down

    private Material textMaterial;
    private float originalScale;
    private float beatTimer;
    private bool scalingUp = true;

    void Start()
    {
        glowColor = transitionMaterial.GetColor("_startColor");

        // Convert the color to HSV
        Color.RGBToHSV(glowColor, out float h, out float s, out float v);

        // Increase the saturation (clamping to a max of 1)
        s = Mathf.Clamp(s * 1.5f, 0f, 1f); // Multiply the saturation by 1.5 (increase by 50%)

        // Convert back to RGB
        glowColor = Color.HSVToRGB(h, s, v);

        // Ensure the alpha remains 1
        glowColor.a = 1f;

        // Get the material of the TextMeshPro object
        textMaterial = textMeshPro.fontMaterial;
        textMaterial.EnableKeyword("_GLOW_ON");
        originalScale = textMeshPro.rectTransform.localScale.x;
        beatTimer = beatDuration;
    }

    void Update()
    {
        // Cycle glow intensity
        float glowIntensity = Mathf.Lerp(minGlowIntensity, maxGlowIntensity, (Mathf.Sin(Time.time * glowIntensityCycleSpeed) + 1) / 2);
        textMaterial.SetFloat("_GlowPower", glowIntensity);

        // Keep the glow color white
        textMaterial.SetColor("_GlowColor", glowColor);

        // Set the face color from the transition material
        textMaterial.SetColor("_FaceColor", faceColor);  // Set face color here

        // Handle the "beat" scaling effect
        beatTimer -= Time.deltaTime;

        if (beatTimer <= 0f)
        {
            // Start a new beat cycle
            scalingUp = true;
            beatTimer = beatDuration;
        }

        if (scalingUp)
        {
            // Scale up quickly
            textMeshPro.rectTransform.localScale = Vector3.Lerp(textMeshPro.rectTransform.localScale, Vector3.one * originalScale * beatScaleMultiplier, scaleUpSpeed * Time.deltaTime);

            if (textMeshPro.rectTransform.localScale.x >= originalScale * beatScaleMultiplier * 0.99f)
            {
                scalingUp = false; // Switch to scaling down
            }
        }
        else
        {
            // Scale down slowly
            textMeshPro.rectTransform.localScale = Vector3.Lerp(textMeshPro.rectTransform.localScale, Vector3.one * originalScale, scaleDownSpeed * Time.deltaTime);
        }
    }
}
