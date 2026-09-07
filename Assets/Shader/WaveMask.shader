Shader "Custom/WaveMask"
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
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
                float curve = sin(i.uv.y * 6.28318 * _CurveFreq + _CurvePhase) * _CurveAmount;
                float edge = _Progress + curve;
            
                float x = 1.0 - i.uv.x;
                clip(edge - x);
            
                return half4(1,1,1,1);
            }
            ENDCG
        }
    }
}