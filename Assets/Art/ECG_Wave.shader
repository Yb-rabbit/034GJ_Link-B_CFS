Shader "Unlit/ECG_Wave_Simple"
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // 简化的心电图波形函数（避免 sin 参数过大）
            float getWaveY(float x, float pulseVal)
            {
                // 基础正弦波
                float y = 0.2 * sin(x * _Frequency * 0.6);
                y += 0.1 * sin(x * _Frequency * 1.8 + 1.5);
                // R 波尖峰：当 pulseVal > 0.9 时产生一个窄脉冲
                float spike = 0.0;
                if (pulseVal > 0.9)
                {
                    // 只在 x 靠近 0.5 时出现尖峰
                    float dx = abs(x - 0.5);
                    if (dx < 0.05)
                        spike = 0.4 * (1.0 - dx / 0.05);
                }
                y += spike;
                return y;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 滚动偏移
                float time = _Time.y * _Speed;
                float x = i.uv.x + time;
                x = frac(x);  // 循环滚动

                // 计算波形值（范围 -0.3~0.3 左右）
                float waveRaw = getWaveY(x * 6.28318, _Pulse);
                // 映射到屏幕高度 0~1
                float waveY = 0.5 + waveRaw * _Amplitude;

                float pixelY = i.uv.y;
                float dist = abs(pixelY - waveY);
                // 线宽
                float lineAlpha = 1.0 - step(_Thickness, dist);
                // 边缘柔和一点
                if (dist < _Thickness * 2.0)
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