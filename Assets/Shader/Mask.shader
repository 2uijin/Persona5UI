Shader "Custom/Mask"
{
    Properties{
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _ID("Mask ID", Int) = 1
        _Progress("Wipe Progress", Range(-0.5, 1.5)) = 0
        _CurveAmount("Curve Amount", Range(0, 0.5)) = 0.1
        _CurveFreq("Curve Frequency", Float) = 1.0
        _CurvePhase("Curve Phase", Float) = 0.0
    }
    SubShader{
        Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+1" }
        ColorMask 0
        ZWrite off			
        Stencil{
            Ref[_ID]
            Comp always
            Pass replace
        }
        Pass{
            CGINCLUDE
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _Progress;
            float _CurveAmount;
            float _CurveFreq;
            float _CurvePhase;

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            half4 frag(v2f i) : SV_Target{
                // 프로퍼티 무시하고 강제로 큰 값 하드코딩
                float curve = sin(i.uv.y * 6.28318 * 2.0) * 0.3; // 크게, 확실히 보이게
                float edge = 0.5 + curve; // _Progress도 하드코딩
            
                clip(edge - i.uv.x);
                return half4(1,1,1,1);
            }
            ENDCG
        }
    }
}