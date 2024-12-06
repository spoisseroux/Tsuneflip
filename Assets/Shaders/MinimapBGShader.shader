Shader "Custom/MinimapBGShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeFade ("Edge Fade Distance", float) = 0.5
        _FadeAmount ("Fade Amount", float) = 1.0 // Controls the amount of fading
        _CheckerColor1 ("Checker Color 1", Color) = (1, 1, 1, 1)
        _CheckerColor2 ("Checker Color 2", Color) = (0, 0, 0, 1)
        _CheckerTiling ("Checker Tiling", float) = 5.0 // Tiling amount for checkerboard
        _ScrollSpeedX ("Scroll Speed X", float) = 0.1 // Speed for X scrolling
        _ScrollSpeedY ("Scroll Speed Y", float) = 0.1 // Speed for Y scrolling
        _Color ("Overall Color", Color) = (1, 1, 1, 1) // Color applied to final output
    }
    SubShader
    {
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
            float4 _CheckerColor1;
            float4 _CheckerColor2;
            float _CheckerTiling;
            float _ScrollSpeedX;
            float _ScrollSpeedY;
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
                // Main texture color
                half4 texCol = tex2D(_MainTex, IN.uv);

                // Calculate distance from center of UV space for fade
                float2 centerUV = float2(0.5, 0.5);
                float distFromCenter = distance(IN.uv, centerUV);

                // Create a fade factor based on distance (inner 70% opaque, fading out)
                float fade = smoothstep(0.2, 0.5, distFromCenter) * _FadeAmount;
                texCol.a *= (1.0 - fade);

                // Use built-in _Time for scrolling
                float2 scrollUV = IN.uv + float2(_ScrollSpeedX, _ScrollSpeedY) * _Time.y;

                // Calculate checkerboard pattern
                float2 checkerCoords = frac(scrollUV * _CheckerTiling);
                bool isChecker1 = (checkerCoords.x > 0.5) == (checkerCoords.y > 0.5);

                // Choose between two checker colors based on the calculated checkerboard pattern
                half4 checkerColor = isChecker1 ? _CheckerColor1 : _CheckerColor2;

                // Combine checkerboard with main texture using alpha blending
                half4 finalColor = lerp(checkerColor, texCol, texCol.a);

                // Apply overall color
                return finalColor * _Color;
            }
            ENDHLSL
        }
    }
}