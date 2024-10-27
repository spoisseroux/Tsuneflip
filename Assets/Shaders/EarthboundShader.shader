Shader "Custom/EarthboundBackgroundDualPass"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _Palette("Palette Texture", 2D) = "white" {}
        _Amplitude("Amplitude", Float) = 0.075
        _Frequency("Frequency", Float) = 10.0
        _Speed("Speed", Float) = 2.0
        _AmplitudeVertical("Amplitude Vertical", Float) = 0.0
        _FrequencyVertical("Frequency Vertical", Float) = 0.0
        _SpeedVertical("Speed Vertical", Float) = 0.0
        _ScrollDirection("Scroll Direction", Vector) = (0.0, 0.0, 0.0, 0.0)
        _ScrollingSpeed("Scrolling Speed", Float) = 0.08
        _EnablePaletteCycling("Enable Palette Cycling", Float) = 0.0
        _PaletteSpeed("Palette Speed", Float) = 0.1
        _ScreenHeight("Screen Height", Float) = 640.0
        _Opacity("Opacity", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

        // First Pass: Background Effect
        Pass
        {
            Name "BackgroundEffect"
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragBackground

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _Palette;
            float _Amplitude;
            float _Frequency;
            float _Speed;
            float _AmplitudeVertical;
            float _FrequencyVertical;
            float _SpeedVertical;
            float2 _ScrollDirection;
            float _ScrollingSpeed;
            float _EnablePaletteCycling;
            float _PaletteSpeed;
            float _ScreenHeight;
            float _Opacity;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 fragBackground(v2f i) : SV_Target
            {
                // Calculate horizontal and vertical distortion
                float diff_x = _Amplitude * sin((_Frequency * i.uv.y) + (_Speed * _Time.y));
                float diff_y = _AmplitudeVertical * sin((_FrequencyVertical * i.uv.y) + (_SpeedVertical * _Time.y));

                // Apply scrolling
                float2 scroll = _ScrollDirection * _Time.y * _ScrollingSpeed;
                float2 uv = i.uv + float2(diff_x, diff_y) + scroll;

                // Sample the main texture with distortion applied
                fixed4 texColor = tex2D(_MainTex, uv);

                // Palette cycling
                if (_EnablePaletteCycling > 0.5)
                {
                    float paletteOffset = fmod(texColor.r - _Time.y * _PaletteSpeed, 1.0);
                    texColor.rgb = tex2D(_Palette, float2(paletteOffset, 0.0)).rgb;
                }

                // Pattern effect
                float pattern = fmod(i.uv.y * _ScreenHeight, 2.0) < 1.0 ? 1.0 : 0.0;
                texColor *= pattern;

                // Apply opacity to the background effect
                texColor.a *= _Opacity;

                return texColor;
            }
            ENDCG
        }

        // Second Pass: Main Texture with Opacity
        Pass
        {
            Name "MainTexturePass"
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragMain

            sampler2D _MainTex;
            float _Opacity;

            fixed4 fragMain(v2f i) : SV_Target
            {
                // Sample the main texture without any distortion
                fixed4 texColor = tex2D(_MainTex, i.uv);
                
                // Apply opacity control
                texColor.a *= _Opacity;

                return texColor;
            }
            ENDCG
        }
    }
}
