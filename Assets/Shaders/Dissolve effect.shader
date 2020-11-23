// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CheeseEffect"
{
    Properties
    {
        [PerRendererData] _MainTex("Main texture", 2D) = "white" {}
        [PerRendererData] _Color("Color", Color) = (1,1,1,1)

        _DissolveTexture("Dissolve Texture", 2D) = "white" {}
        _DissolveAmount("Dissolve Amount", Range(0,1)) = 1
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

            sampler2D _DissolveTexture;
            float _DissolveAmount;

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
                float4 dissolveColor = tex2D(_DissolveTexture, IN.uv);

                //float alpha = (5 + sin(_Time.y*10))/6;
                float alpha = 1;
                float tint = (4 + sin(IN.uv.x * 10)) / 5;
                //float tint = 1;

                float4 initialBurn = float4(tint, tint, 0, alpha);
                float4 endBurn = float4(tint, 0, 0, alpha);

                if (any(dissolveColor.rgb - _DissolveAmount < 0) || any(textureColor.a <= 0)) {
                    discard;
                }
                else if (any(dissolveColor.rgb - _DissolveAmount < 0.02)) {
                    return float4(0,0,0,1);
                }
                else if (any(dissolveColor.rgb - _DissolveAmount < 0.04)) {
                    return initialBurn;
                }
                else if (any(dissolveColor.rgb - _DissolveAmount < 0.06)) {
                    return endBurn;
                }
                //clip(dissolveColor.rgb - _DissolveAmount);

                return textureColor * _Color;
            }
            ENDCG
        }
    }
}