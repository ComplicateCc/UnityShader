﻿Shader "Unity Shaders Book/Chapter9/AlphaBlendWithShadow" {
	Properties {
		_Color ("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Main Tex", 2D) = "white" {}
		_AlphaScale ("Alpha Scale", Range(0, 1)) = 1
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

		pass{
			Tags{"LightMode"="ForwardBase"}

			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma multi_compile_fwdbase

			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"
            #include "AutoLight.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _AlphaScale;

			struct a2v{
             float4 vertex:POSITION;
			 float3 normal:NORMAL;
			 float4 texcoord:TEXCOORD0;
			};

			struct v2f{
              float4 pos:SV_POSITION;
			  float3 worldNormal:TEXCOORD0;
			  float3 worldPos:TEXCOORD1;
			  float2 uv:TEXCOORD2;
			  SHADOW_COORDS(3)
			};

			v2f vert(a2v v){
               v2f o;
			   o.pos=UnityObjectToClipPos(v.vertex);
			   o.worldNormal=UnityObjectToWorldNormal(v.normal);
			   o.worldPos=mul(unity_ObjectToWorld,v.vertex).xyz;
			   o.uv=TRANSFORM_TEX(v.texcoord,_MainTex);
			   TRANSFER_SHADOW(o);
			   return o;
			}
			
            fixed4 frag(v2f i):SV_TARGET{
				fixed3 worldNormal=normalize(i.worldNormal);
				fixed3 worldLightDir=normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed4 texColor=tex2D(_MainTex,i.uv);
				//clip(texColor.a-_Cutoff);

				fixed3 albedo=texColor.rgb*_Color.rgb;
				fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz*albedo;
				fixed3 diffuse=_LightColor0.rgb*albedo*max(0,dot(worldNormal,worldLightDir));
				UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos);
				return fixed4(ambient+diffuse*atten,texColor.a*_AlphaScale);
			}
			ENDCG
		}//Cutout/
	}
		//FallBack "Transparent/VertexLit"
	Fallback "VertexLit"
}
