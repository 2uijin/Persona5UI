Shader "Custom/BrushTransition"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Screenshot Map", 2D) = "white" {}
        _BrushTex ("Brush Mask", 2D) = "white" {}
        _Progress ("Progress", Range(0,1)) = 0
        _StencilRef ("Stencil Reference", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline" }
        Cull Off ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Stencil
        {
            Ref [_StencilRef]
            Comp always
            Pass replace
        }
        Pass
        {
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

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BrushTex);
            SAMPLER(sampler_BrushTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                float4 _BrushTex_ST;
                float _Progress;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 snapshotColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                float mask = SAMPLE_TEXTURE2D(_BrushTex, sampler_BrushTex, IN.uv).r;
                float alpha = step(_Progress, mask);
                return half4(snapshotColor.rgb, alpha * snapshotColor.a * _BaseColor.a);
            }
            ENDHLSL
        }
    }
}