Shader "Custom/KatamariCamShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _SkyboxColor ("Skybox Color", Color) = (0, 0, 0, 1) // Skybox color to discard (no longer used)
        _Fade ("Fade", Range(0.0, 1.0)) = 1.0 // Controls overall transparency
    }
    SubShader
    {
        // Change Queue to Transparent to ensure proper UI layering
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

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
            float _Fade; // Transparency control

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

                // Apply the overall fade factor to the alpha channel
                texCol.a *= _Fade;

                return texCol * _Color;
            }
            ENDHLSL
        }
    }
}