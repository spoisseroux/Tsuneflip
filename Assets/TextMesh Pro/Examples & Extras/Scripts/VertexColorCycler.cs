using UnityEngine;
using System.Collections;


namespace TMPro.Examples
{

    public class VertexColorCycler : MonoBehaviour
    {

        private TMP_Text m_TextComponent;
        public Color32 defaultColor = new Color32(0, 0, 255, 255);  // Blue
        public Color32 waveColor = new Color32(173, 216, 230, 255); // Light Blue

        void Awake()
        {
            m_TextComponent = GetComponent<TMP_Text>();
        }


        void Start()
        {
            StartCoroutine(AnimateVertexColors());
        }


        /// <summary>
        /// Method to animate vertex colors of a TMP Text object.
        /// </summary>
        /// <returns></returns>
        IEnumerator AnimateVertexColors()
            {
                m_TextComponent.ForceMeshUpdate();

                TMP_TextInfo textInfo = m_TextComponent.textInfo;
                Color32[] newVertexColors;

                while (true)
                {
                    int characterCount = textInfo.characterCount;

                    if (characterCount == 0)
                    {
                        yield return new WaitForSeconds(0.25f);
                        continue;
                    }

                    float waveDuration = 0.3f;
                    float interval = waveDuration / characterCount;

                    for (int currentCharacter = 0; currentCharacter < characterCount; currentCharacter++)
                    {
                        int materialIndex = textInfo.characterInfo[currentCharacter].materialReferenceIndex;
                        newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                        int vertexIndex = textInfo.characterInfo[currentCharacter].vertexIndex;

                        if (textInfo.characterInfo[currentCharacter].isVisible)
                        {
                            // Set the wave color for the current character
                            newVertexColors[vertexIndex + 0] = waveColor;
                            newVertexColors[vertexIndex + 1] = waveColor;
                            newVertexColors[vertexIndex + 2] = waveColor;
                            newVertexColors[vertexIndex + 3] = waveColor;

                            // Update the vertex colors
                            m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                        }

                        yield return new WaitForSeconds(interval);

                        // Set the default color for the current character
                        newVertexColors[vertexIndex + 0] = defaultColor;
                        newVertexColors[vertexIndex + 1] = defaultColor;
                        newVertexColors[vertexIndex + 2] = defaultColor;
                        newVertexColors[vertexIndex + 3] = defaultColor;

                        // Update the vertex colors again
                        m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                    }

                    // Wait for 4 seconds before starting the next wave
                    yield return new WaitForSeconds(9f);
                }
            }

    }
}
