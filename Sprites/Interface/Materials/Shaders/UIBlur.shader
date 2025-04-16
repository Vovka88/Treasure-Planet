Shader "UI/BlurPanelBright"
{
    Properties
    {
        _Color("Tint Color", Color) = (1,1,1,0.5)
        _Size("Blur Size", Range(1,20)) = 2
        _Brightness("Brightness", Range(-1, 2.0)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        GrabPass { "_GrabTexture" }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            float4 _Color;
            float _Size;
            float _Brightness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy / i.screenPos.w;

                fixed4 col = fixed4(0,0,0,0);
                int samples = 9;
                float total = 0;

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 offset = float2(x, y) * _GrabTexture_TexelSize.xy * _Size;
                        col += tex2D(_GrabTexture, uv + offset);
                        total += 1.0;
                    }
                }

                col /= total;
                col.rgb *= _Brightness;     // увеличение яркости
                col = lerp(col, _Color, _Color.a); // добавление цвета (tint)

                return col;
            }
            ENDCG
        }
    }
}
