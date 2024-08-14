using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class CharacterWidthScaler : MonoBehaviour
{
    public float widthScale = 1.0f; // Default scale factor, 1.0 means no scaling

    private TMP_Text textMeshPro;

    void Start()
    {
        textMeshPro = GetComponent<TMP_Text>();
        if (textMeshPro != null)
        {
            textMeshPro.ForceMeshUpdate();
            ScaleCharacterWidths();
        }
    }

    void OnValidate()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TMP_Text>();
        }

        if (textMeshPro != null)
        {
            textMeshPro.ForceMeshUpdate();
            ScaleCharacterWidths();
        }
    }

    private void ScaleCharacterWidths()
    {
        // Get the text mesh's geometry
        TMP_TextInfo textInfo = textMeshPro.textInfo;

        // Ensure there is valid character data
        if (textInfo == null || textInfo.characterCount == 0)
            return;

        // Loop through each character in the text
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            int vertexIndex = charInfo.vertexIndex;

            // Get the vertices of the character
            Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            // Calculate the mid-point of the character's vertices
            Vector3 charMidBasline = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2;

            // Scale the vertices horizontally
            vertices[vertexIndex + 0].x = (vertices[vertexIndex + 0].x - charMidBasline.x) * widthScale + charMidBasline.x;
            vertices[vertexIndex + 1].x = (vertices[vertexIndex + 1].x - charMidBasline.x) * widthScale + charMidBasline.x;
            vertices[vertexIndex + 2].x = (vertices[vertexIndex + 2].x - charMidBasline.x) * widthScale + charMidBasline.x;
            vertices[vertexIndex + 3].x = (vertices[vertexIndex + 3].x - charMidBasline.x) * widthScale + charMidBasline.x;
        }

        // Update the mesh with the new vertex positions
        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
}