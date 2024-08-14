using UnityEngine;
using TMPro;

public class RainbowTextEffect : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro; // Assign your TextMeshPro component in the Inspector
    public float speed = 1.0f; // Speed of the gradient scrolling
    public float gradientScale = 5.0f; // Controls the width of the gradient across the text

    private void Update()
    {
        textMeshPro.ForceMeshUpdate();
        TMP_TextInfo textInfo = textMeshPro.textInfo;
        
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            float timeOffset = Time.time * speed;
            float charPositionOffset = (float)i / textInfo.characterCount * gradientScale;
            float totalOffset = timeOffset + charPositionOffset;

            Color rainbowColor = new Color(
                Mathf.Sin(totalOffset + 0f) * 0.5f + 0.5f,
                Mathf.Sin(totalOffset + 2f * Mathf.PI / 3f) * 0.5f + 0.5f,
                Mathf.Sin(totalOffset + 4f * Mathf.PI / 3f) * 0.5f + 0.5f
            );

            textMeshPro.textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].colors32[textInfo.characterInfo[i].vertexIndex + 0] = rainbowColor;
            textMeshPro.textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].colors32[textInfo.characterInfo[i].vertexIndex + 1] = rainbowColor;
            textMeshPro.textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].colors32[textInfo.characterInfo[i].vertexIndex + 2] = rainbowColor;
            textMeshPro.textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].colors32[textInfo.characterInfo[i].vertexIndex + 3] = rainbowColor;
        }

        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}