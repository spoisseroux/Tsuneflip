using UnityEngine;
using TMPro;

public class BillowingText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float waveFrequency = 2.0f;  // Frequency of the wave motion
    public float waveAmplitude = 5.0f;  // Amplitude of the wave (how much it moves)
    public float speed = 1.0f;          // Speed of the wave motion
    public float billowSpeed = 0.5f;    // How quickly the text moves toward the camera

    private TMP_TextInfo textInfo;
    private Vector3[] originalVertices;
    private Matrix4x4 matrix;

    void Start()
    {
        textMeshPro.ForceMeshUpdate();  // Ensure the mesh is updated
        textInfo = textMeshPro.textInfo;
        originalVertices = new Vector3[textInfo.meshInfo[0].vertices.Length];
    }

    void Update()
    {
        textMeshPro.ForceMeshUpdate(); // Ensure text mesh is up-to-date

        // Get text info
        textInfo = textMeshPro.textInfo;

        // Loop through each character
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            // Skip characters that aren't visible
            if (!charInfo.isVisible)
                continue;

            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;

            // Get the vertices for this character
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // Copy the original vertices to modify them
            if (originalVertices.Length != vertices.Length)
            {
                originalVertices = new Vector3[vertices.Length];
                vertices.CopyTo(originalVertices, 0);
            }

            // Compute the wave movement based on character position and time
            float offset = Mathf.Sin(Time.time * speed + i * waveFrequency) * waveAmplitude;

            // Apply a scaling transformation (moving the text toward the camera)
            Vector3 center = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;
            matrix = Matrix4x4.TRS(Vector3.forward * billowSpeed * Time.deltaTime, Quaternion.identity, Vector3.one);

            // Apply the wave motion and matrix transformation
            vertices[vertexIndex] = matrix.MultiplyPoint3x4(vertices[vertexIndex] + Vector3.forward * offset);
            vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1] + Vector3.forward * offset);
            vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2] + Vector3.forward * offset);
            vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3] + Vector3.forward * offset);
        }

        // Apply the modified vertices back to the mesh
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
