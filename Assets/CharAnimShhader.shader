Shader "Custom/CharAnimShhader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Opacity ("Opacity", Range(0,1)) = 1.0 // Opacity property
        _RedThreshold ("Red Threshold", Range(0,1)) = 0.9 // Threshold for red culling
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
            float _Opacity;
            float _RedThreshold;

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
                
                // Check if the red component of the pixel color is above the threshold and discard if it is
                if (texCol.r > _RedThreshold)
                {
                    discard;
                }

                // Multiply the color's alpha by the opacity property
                texCol.a *= _Opacity;

                return texCol * _Color;
            }
            ENDHLSL
        }
    }
}
