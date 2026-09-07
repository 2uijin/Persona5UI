Shader "Custom/URP/SkillPageMask"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _BgTex ("Background Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _BlackThreshold ("Black Threshold", Range(0,0.3)) = 0.05
        _WhiteThreshold ("White Threshold", Range(0.0,1)) = 0.95

        // ---- Wave Mask ----
        _Progress ("Wipe Progress", Range(-0.5, 1.5)) = 0
        _CurveAmount ("Curve Amount", Range(0, 0.5)) = 0.1
        _CurveFreq ("Curve Frequency", Float) = 1.0
        _CurvePhase ("Curve Phase", Float) = 0.0

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex     : SV_POSITION;
                half4 color       : COLOR;
                float2 texcoord   : TEXCOORD0;
                float4 screenPos  : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            TEXTURE2D(_BgTex);
            SAMPLER(sampler_BgTex);
            float4 _BgTex_ST;

            float4 _Color;
            float _BlackThreshold;
            float _WhiteThreshold;

            float _Progress;
            float _CurveAmount;
            float _CurveFreq;
            float _CurvePhase;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = TransformObjectToHClip(v.vertex.xyz);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.screenPos = ComputeScreenPos(OUT.vertex);
                OUT.color = v.color * _Color;
                return OUT;
            }

half4 frag(v2f IN) : SV_Target
{
    // ---- Wave Mask clip ----
    float curve = sin(IN.texcoord.y * 6.28318 * _CurveFreq + _CurvePhase) * _CurveAmount;
    float edge = _Progress + curve;
    float x = IN.texcoord.x;
    clip(edge - x);

    half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.texcoord);
    half4 col = texColor * IN.color;

    bool isBlack = (col.r < _BlackThreshold &&
                     col.g < _BlackThreshold &&
                     col.b < _BlackThreshold &&
                     col.a > 0.01);

    if (isBlack)
    {
        float2 screenUV = IN.vertex.xy / _ScreenParams.xy;
        half4 bgColor = SAMPLE_TEXTURE2D(_BgTex, sampler_BgTex, screenUV);

        bool isBgWhite = (bgColor.r > _WhiteThreshold &&
                           bgColor.g > _WhiteThreshold &&
                           bgColor.b > _WhiteThreshold);

        if (isBgWhite)
            col.rgb = half3(0, 0, 0);
        else
            discard;
    }

    return col;
}
            ENDHLSL
        }
    }
}