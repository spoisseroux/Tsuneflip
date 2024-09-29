using UnityEngine;

[CreateAssetMenu(fileName = "TransitionColor", menuName = "Custom/TransitionColorData", order = 1)]
public class TransitionColorScriptableObject : ScriptableObject
{
    public Color color;

    public void SetColor(Color newColor)
    {
        color = newColor;
    }

    public Color GetColor()
    {
        return color;
    }
}
