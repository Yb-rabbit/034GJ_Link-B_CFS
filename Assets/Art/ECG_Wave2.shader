Shader "Unlit/ECG_Wave2"
{
    Properties
    {
        _LineColor ("Line Color", Color) = (0,1,0,1)
        _BgColor ("Background Color", Color) = (0,0,0,1)
        _Speed ("Speed", Range(-2,2)) = 0.5
        _Amplitude ("Amplitude", Range(0,0.3)) = 0.1
        _Frequency ("Frequency", Range(2,20)) = 8
        _Thickness ("Line Thickness", Range(0.001,0.02)) = 0.008
        _Pulse ("Pulse", Range(0,1)) = 0
        _ActiveRatio ("Active Ratio (0~1)", Range(0.1, 1)) = 0.7
        _BaselineValue ("Baseline Value", Range(-0.2, 0.2)) = 0.0   // 静息期的平直线数值
        _SmoothWidth ("Smooth Width", Range(0.01, 0.2)) = 0.05       // 过渡区宽度
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _LineColor;
            fixed4 _BgColor;
            float _Speed;
            float _Amplitude;
            float _Frequency;
            float _Thickness;
            float _Pulse;
            float _ActiveRatio;
            float _BaselineValue;
            float _SmoothWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // 原始心电图波形（无静息逻辑）
            float getWaveY_original(float x, float pulseVal)
            {
                float y = 0.2 * sin(x * _Frequency * 0.6);
                y += 0.1 * sin(x * _Frequency * 1.8 + 1.5);
                float spike = 0.0;
                if (pulseVal > 0.9)
                {
                    float dx = abs(x - 3.14159);
                    if (dx < 0.3)
                        spike = 0.4 * (1.0 - dx / 0.3);
                }
                y += spike;
                return y;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y * _Speed;
                float x = i.uv.x + time;
                x = frac(x);  // 0~1 循环

                // 平滑的活动因子：在活动区间边界处渐变，而非硬切
                float edgeLow = _ActiveRatio - _SmoothWidth * 0.5;
                float edgeHigh = _ActiveRatio + _SmoothWidth * 0.5;
                float activity = smoothstep(edgeLow, edgeHigh, x);
                // activity = 1 表示完全显示波形，0 表示完全显示基线

                // 计算原始波形值（x 映射到 0~2π）
                float waveRaw_original = getWaveY_original(x * 6.28318, _Pulse);
                // 根据 activity 混合波形和基线
                float waveRaw = lerp(_BaselineValue, waveRaw_original, activity);
                // 映射到屏幕 Y 坐标
                float waveY = 0.5 + waveRaw * _Amplitude;

                float pixelY = i.uv.y;
                float dist = abs(pixelY - waveY);
                float lineAlpha = 0.0;
                if (dist < _Thickness)
                    lineAlpha = 1.0;
                else if (dist < _Thickness * 2.0)
                    lineAlpha = 1.0 - (dist - _Thickness) / _Thickness;
                else
                    lineAlpha = 0.0;

                fixed4 col = lerp(_BgColor, _LineColor, lineAlpha);
                return col;
            }
            ENDCG
        }
    }
}