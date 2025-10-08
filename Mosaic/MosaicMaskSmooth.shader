Shader "Custom/MosaicMaskSmooth_UI"
{
    Properties
    {
        _Foreground("Foreground Mask", 2D) = "white" {}
        _MainTex("Background", 2D) = "black" {}         // UI用に必須
        _KeyColor("Key Color", Color) = (0,1,0,1)       // クロマキー色
        _Threshold("Threshold", Range(0,1)) = 0.3       // クロマキー判定
        _Fade("Fade Width", Range(0,0.2)) = 0.05        // 境界ぼかし幅
        _Size("Block Size", Float) = 8                  // モザイク粗さ
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _Foreground;
            sampler2D _MainTex;
            float4 _KeyColor;
            float _Threshold;
            float _Fade;
            float _Size;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Foreground マスク
                fixed4 fg = tex2D(_Foreground, uv);
                float dist = distance(fg.rgb, _KeyColor.rgb);

                // マスクのフェード量（smoothstepで境界ぼかし）
                float mask = smoothstep(_Threshold, _Threshold + _Fade, dist);

                // モザイクUV
                float2 muv = floor(uv * _Size) / _Size + 0.5 / _Size;

                // モザイク色と元色を補間
                fixed4 mosaic = tex2D(_MainTex, muv);
                fixed4 orig = tex2D(_MainTex, uv);

                fixed4 col = lerp(orig, mosaic, mask);

                return col;
            }
            ENDCG
        }
    }
}
