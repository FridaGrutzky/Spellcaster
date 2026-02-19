Shader "SpellTracing/PathShader"
{
    Properties
    {
        _MainTex    ("Shape Silhouette", 2D)    = "white" {}
        _FillMap    ("Fill Map",         2D)    = "white" {}
        _FillAmount ("Fill Amount",      Range(0,1)) = 0
        _FillColor  ("Fill Color",       Color) = (1,1,0,1)
        _BaseColor  ("Base Color",       Color) = (0.2,0.2,0.2,1)
    }

    SubShader
    {
        // Transparent so shape silhouette alpha-clips correctly
        Tags
        {
            "RenderType"      = "TransparentCutout"
            "Queue"           = "AlphaTest"
            "RenderPipeline"  = "UniversalPipeline"
        }

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            // Render both faces so the tablet is visible from behind
            Cull Off

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // ── Textures ─────────────────────────────────────────────
            TEXTURE2D(_MainTex);  SAMPLER(sampler_MainTex);
            TEXTURE2D(_FillMap);  SAMPLER(sampler_FillMap);

            // ── Per-material CBUFFER (required by URP SRP Batcher) ───
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _FillMap_ST;
                float  _FillAmount;
                float4 _FillColor;
                float4 _BaseColor;
            CBUFFER_END

            // ── Vertex ───────────────────────────────────────────────
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            // ── Fragment ─────────────────────────────────────────────
            half4 frag(Varyings IN) : SV_Target
            {
                // Sample textures
                half4 shapeSample   = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half  fillValue     = SAMPLE_TEXTURE2D(_FillMap, sampler_FillMap, IN.uv).r;

                // Discard pixels outside the shape silhouette
                clip(shapeSample.a - 0.1);

                // Pixels where fillValue > (1 - _FillAmount) are "filled"
                // e.g. _FillAmount = 0   → nothing filled
                //      _FillAmount = 0.5 → bottom half filled
                //      _FillAmount = 1   → fully filled
                half4 color = fillValue > (1.0 - _FillAmount) ? _FillColor : _BaseColor;

                // Preserve shape alpha for soft edges if your silhouette has them
                color.a = shapeSample.a;
                return color;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
