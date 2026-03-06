Shader "Custom/SimpleDistanceFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float3 worldPos : TEXCOORD1; 
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _FogColor;
            
            // 接收 C# 传来的全局变量
            float3 _PlayerPos;
            float _ViewRadius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // 关键一步：把顶点坐标转换成世界坐标传给片元着色器
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 采样原纹理
                fixed4 col = tex2D(_MainTex, i.uv);

                // 计算距离
                float dist = distance(i.worldPos, _PlayerPos);

                // 计算可见度：距离小于(_ViewRadius - 2)完全可见，大于_ViewRadius完全不可见
                // smoothstep 会让边缘有柔和的过渡
                float visibility = smoothstep(_ViewRadius, _ViewRadius - 2.0, dist); 

                // 混合颜色：visibility=0显示迷雾，=1显示原色
                return lerp(_FogColor, col, visibility);
            }
            ENDCG
        }
    }
}
