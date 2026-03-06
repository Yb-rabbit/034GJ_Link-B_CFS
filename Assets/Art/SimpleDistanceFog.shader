Shader "Custom/SimpleDistanceFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0, 0, 0, 0.8) // 降低alpha，避免完全遮挡
        _NoiseTex ("Noise Texture", 2D) = "white" {} // 添加噪声纹理
        _NoiseScale ("Noise Scale", Float) = 1.0 // 噪声缩放（控制噪声大小）
        _NoiseIntensity ("Noise Intensity", Float) = 0.1 // 噪声强度（控制迷雾波动幅度）
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" } // 调整渲染队列，确保迷雾在其他物体之上
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // 设置混合模式，让迷雾半透明
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
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _FogColor;
            float3 _PlayerPos;
            float _ViewRadius;
            sampler2D _NoiseTex; // 声明噪声纹理
            float _NoiseScale;   // 声明噪声缩放参数
            float _NoiseIntensity; // 声明噪声强度参数

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // 计算距离（考虑高度差，让迷雾在物体周围更自然）
                float heightDiff = abs(i.worldPos.y - _PlayerPos.y);
                float dist = distance(i.worldPos, _PlayerPos) + heightDiff * 0.5f; // 高度差影响距离

                // 基础过渡（保持柔和边缘）
                float visibility = smoothstep(_ViewRadius, _ViewRadius - 8.0, dist);

                // --- 方案三：添加噪声纹理 ---
                // 1. 计算噪声UV（使用世界坐标的xz分量，并随时间流动）
                float2 noiseUV = i.worldPos.xz * _NoiseScale + _Time.y * 0.1;
                // 2. 采样噪声纹理（获取R通道的灰度值）
                float noise = tex2D(_NoiseTex, noiseUV).r;
                // 3. 用噪声调整迷雾的局部透明度（噪声区域会让迷雾稍微变暗/变亮）
                visibility *= (1.0 - noise * _NoiseIntensity);

                return lerp(_FogColor, col, visibility);
            }
            ENDCG
        }
    }
}
