﻿Shader "Unlit/Screen Position"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            struct v2f
            {
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (float4 vertex:POSITION,float2 uv:TEXCOORD0,out float4 outpos:SV_POSITION)
            {
                v2f o;
                o.uv = uv;
                outpos=UnityObjectToClipPos(vertex);
                return o;
            }

            fixed4 frag (v2f i,UNITY_VPOS_TYPE screenPos:VPOS) : SV_Target
            {
                screenPos.xy=floor(screenPos.xy*0.25)*0.5;
                float checker=-frac(screenPos.r+screenPos.g);
                clip(checker);
                fixed4 col=tex2D(_MainTex,i.uv);
                return col;
            }
            ENDCG
        }
    }
}
