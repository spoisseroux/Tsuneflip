Shader "Custom/PreviewCameraTransparentShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Pass
        {
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha
            SetTexture [_MainTex] { combine texture }
        }
    }
}