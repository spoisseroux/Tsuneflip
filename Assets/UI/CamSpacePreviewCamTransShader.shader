Shader "Custom/CamSpacePreviewCamTransShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _SkyboxColor ("Skybox Color", Color) = (0, 0, 0, 1) // Skybox color to discard
        _EdgeFade ("Edge Fade Distance", float) = 0.5
        _FadeAmount ("Fade Amount", float) = 1.0 // Controls the amount of fading
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }

        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _Color;
            float4 _SkyboxColor;
            float _EdgeFade;
            float _FadeAmount;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 texCol = tex2D(_MainTex, IN.uv);

                // Calculate distance from center of UV space
                float2 centerUV = float2(0.5, 0.5);
                float distFromCenter = distance(IN.uv, centerUV);

                // Create a fade factor based on distance
                // Inner 70% should be opaque, then fade out
                float fade = smoothstep(0.35, 0.5, distFromCenter) * _FadeAmount;

                // Check if the pixel color matches the skybox color and discard if it does
                if (all(abs(texCol.rgb - _SkyboxColor.rgb) < 0.01))
                {
                    discard;
                }

                // Apply fade to the alpha channel, smoothly transitioning the alpha based on distance
                texCol.a *= (1.0 - fade);

                return texCol * _Color;
            }
            ENDHLSL
        }
    }
}
