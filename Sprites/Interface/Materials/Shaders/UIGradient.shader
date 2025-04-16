Shader "UI/AlphaParabolaOverlay"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1) // Цвет тени
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Falloff Intensity", Range(0.1, 10)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Intensity;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float y = saturate(i.uv.y);
                float fade = 1.0 - pow(y, _Intensity); // параболическое или более крутое затухание
                fixed4 result = _Color;
                result.a *= fade;
                return result;
            }
            ENDCG
        }
    }
}
