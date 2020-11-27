// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Genesis Particles Separation"
{
    Properties
    {
        [PerRendererData] _MainTex("Main texture", 2D) = "white" {}
        [PerRendererData] _Color("Color", Color) = (1,1,1,1)

        _FillTexture("Fill Texture", 2D) = "white" {}
        _FillAmount("Fill Amount", Range(0,1)) = 1

        _PositiveCenter("Positive Center", Vector) = (0,0,0,0)
        _NegativeCenter("Negative Center", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _MainTex_ST;

            sampler2D _FillTexture;
            float4 _FillTexture_TexelSize;
            float4 _FillTexture_ST;
            float _FillAmount;

            float4 _PositiveCenter;
            float4 _NegativeCenter;

            v2f vert(appdata IN)
            {
                v2f OUT;

                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;

                return OUT;
            }


            fixed4 frag(v2f IN) : SV_Target
            {
                float4 textureColor = tex2D(_MainTex, IN.uv);
                float4 fillColor = tex2D(_FillTexture, float2(IN.uv.x * _FillTexture_TexelSize.x * _MainTex_TexelSize.z, IN.uv.y * _FillTexture_TexelSize.y * _MainTex_TexelSize.w)
                    + float2(_FillTexture_ST.z * _FillTexture_TexelSize.x * (_FillTexture_TexelSize.z - _MainTex_TexelSize.z), _FillTexture_ST.w * _FillTexture_TexelSize.y * (_FillTexture_TexelSize.w - _MainTex_TexelSize.w)));
                
                if (textureColor.a <= 0) {
                    discard;
                }

                float difference = fillColor.r - 1;

                float4 result = clamp(textureColor + float4(-0.5f * difference, difference, -0.5f * difference, 0),
                        float4(0, 0, 0, 0), float4(1, 1, 1, 1));

                float MULTIPLY_FACTOR = pow(_FillAmount,2) * 7.5f;

                float centersDistance = distance(_PositiveCenter, _NegativeCenter);

                float brighter = max(_FillAmount - 0.5f, 0) * 2;

                bool altered = false;

                float distancePositive = distance(IN.uv, _PositiveCenter.xy);
                float distanceNegative = distance(IN.uv, _NegativeCenter.xy);

                float currentMaxDistance = centersDistance * (1 + fillColor.r) * (1.1f - _FillAmount);
                if (distanceNegative < currentMaxDistance) {
                    result += float4(MULTIPLY_FACTOR / (distanceNegative / currentMaxDistance), brighter / (distanceNegative / currentMaxDistance), brighter / (distanceNegative / currentMaxDistance), 0) * fillColor.r;
                    altered = true;
                }
                if (distancePositive < currentMaxDistance) {
                    result += float4(brighter / (distancePositive / currentMaxDistance), brighter/ (distancePositive / currentMaxDistance), MULTIPLY_FACTOR / (distancePositive / currentMaxDistance), 0) * (1 - fillColor.r);
                    altered = true;
                }
                if (altered) {
                    result.rgb = max(result.rgb, float3(0, 0, 0));
                    return result;
                }


                return textureColor * _Color;
            }
            ENDCG
        }
    }
}