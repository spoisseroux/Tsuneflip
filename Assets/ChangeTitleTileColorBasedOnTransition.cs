using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTitleTileColorBasedOnTransition : MonoBehaviour
{

    public Material transitionMaterial;
    private Color glowColor;
    public Material tileCursorMaterial;

    void Start()
    {
        glowColor = transitionMaterial.GetColor("_startColor");
        Color.RGBToHSV(glowColor, out float h, out float s, out float v);
        s = Mathf.Clamp(s * 1.5f, 0f, 1f);
        glowColor = Color.HSVToRGB(h, s, v);
        glowColor.a = 1f;
        tileCursorMaterial.SetColor("_Color2", glowColor);
    }
}
