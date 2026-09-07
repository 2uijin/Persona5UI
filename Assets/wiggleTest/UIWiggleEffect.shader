Shader "Custom/UIWiggleEffect"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
         _Color ("Color Tint", Color)= (1,1,1,1)
         _WiggleAmountX ("WiggleAmountX", float) = 5
         _WiggleAmountY ("WiggleAmountY", float) = 5
         _Speed ("Speed", float) = 2
         _TimeOffset ("Time Offset", float) = 0
         _Chaos ("Chaos (freq variation)", Range(0, 3)) = 1.5
         _WhiteThreshold ("White Threshold", Range(0, 1)) = 0.85

        // ---- 추가된 프로퍼티 ----
        _Intensity ("Wiggle / Reveal Intensity", Range(0, 1)) = 1   // 0=숨김(display:none), 1=풀 웨이글. 평소엔 1로 두고 등장 연출 쓸 때만 0에서 트윈
        _ScaleX ("Shape Scale X", float) = 1                        // cyan-fill scale(3,.1) 같은 비대칭 스케일 재현
        _ScaleY ("Shape Scale Y", float) = 1
        _PunchScale ("Punch Scale", float) = 1                      // tiltBounce 펀치 (DOTween Yoyo 1→1.5→1.15)
        _RotationDeg ("Z Rotation (deg)", float) = 0                // rotate(-6deg) / rotate(11deg) 재현

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "CanUseSpriteAtlas"="True" }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha One
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _Color;
                float _WiggleAmountX;
                float _WiggleAmountY;
                float _Speed;
                float _TimeOffset;
                float _Chaos;
                float _WhiteThreshold;
                // ---- 추가된 변수 ----
                float _Intensity;
                float _ScaleX;
                float _ScaleY;
                float _PunchScale;
                float _RotationDeg;
            CBUFFER_END            // 0~1 사이 의사 난수 (정점 좌표를 시드로 사용)

            float hash11(float p)
            {
                p = frac(p * 0.1031);
                p *= p + 33.33;
                p *= p + p;
                return frac(p);
            }

            // ---- 추가된 함수: Z축 회전 ----
            float2 rotate2D(float2 v, float degrees)
            {
                float rad = radians(degrees);
                float s = sin(rad);
                float c = cos(rad);
                return float2(v.x * c - v.y * s, v.x * s + v.y * c);
            }

Varyings vert(Attributes IN)
{
    Varyings OUT;

    float seed = hash11(dot(IN.vertex.xy, float2(12.9898, 78.233)) + _TimeOffset * 10.0);

    float freqA = 1.0 + seed * _Chaos;
    float freqB = 2.3 + (1.0 - seed) * _Chaos;
    float phaseA = seed * 6.2831853;
    float phaseB = (1.0 - seed) * 6.2831853;

    float time = _Time.y * _Speed + _TimeOffset;

    float noiseX = sin(time * freqA + phaseA) * 0.6
                 + sin(time * freqB * 1.7 + phaseB) * 0.4;
    float noiseY = sin(time * freqB + phaseB * 1.3) * 0.5
                 + sin(time * freqA * 0.8 + phaseA * 0.6) * 0.5;

    // ---- 수정: 웨이글 세기를 Intensity로 게이팅 ----
    float offsetX = noiseX * _WiggleAmountX * _Intensity;
    float offsetY = noiseY * _WiggleAmountY * _Intensity;

    float3 pos = IN.vertex.xyz;

    // ---- 추가: 비대칭 스케일 + 펀치 스케일 (오브젝트 중심 기준) ----
    pos.x *= _ScaleX * _PunchScale;
    pos.y *= _ScaleY * _PunchScale;

    // ---- 추가: Z축 회전 ----
    pos.xy = rotate2D(pos.xy, _RotationDeg);

    pos.x += offsetX;
    pos.y += offsetY;

    OUT.vertex = TransformObjectToHClip(pos);
    OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
    OUT.color = IN.color * _BaseColor;
    return OUT;
}
            half4 frag(Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

                // 텍스처 픽셀의 밝기(luma) 계산
                half luma = dot(tex.rgb, half3(0.299, 0.587, 0.114));

                // 밝기가 임계값 이상이면(흰색이면) 1, 아니면 0
                half whiteMask = step(_WhiteThreshold, luma);

                // 흰색 부분만 틴트 컬러 적용, 나머지는 원본 그대로
                half3 tintedRGB = lerp(tex.rgb, _Color.rgb, whiteMask);

                // ---- 수정: 알파에도 Intensity 곱해서 등장/소멸(display:none<->block) 표현 ----
                half4 color = half4(tintedRGB, tex.a * _Intensity) * IN.color;

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif
                return color;
            }
            ENDHLSL
        }
    }
}
