﻿Shader "EACH/EyeBallHDPbr"
{
    Properties
    {
        [Header(Colors)]
        _InternalColor("Internal Color",Color)=(1,1,1,1)  //内部的
        _EmissionColor("Emission Color",Color)=(1,1,1,1)  //发出自发光？
        _EyeColor("Iris Color",Color)=(0,0,1,0)
        _ScleraColor("Sclera Color",Color)=(1,1,1,0)                                  //Sclera虹膜
        [HideInInspector]
        _Color("Main Color",Color)=(1,1,1,1)
        [Space(20)]
        _MainTex("Albedo (RGB)",2D)="white"{}
        _BumpMap("Normals",2D)="bump"{}
        [HideInInspector]
        
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
