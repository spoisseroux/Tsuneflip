Shader "Custom/PreviewCameraTransparentShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" } // Set to Overlay to ensure it renders with the UI
        Pass
        {
            ZWrite On
            ZTest Always // Always pass the depth test
            Blend SrcAlpha OneMinusSrcAlpha
            SetTexture [_MainTex] { combine texture }
        }
    }
}