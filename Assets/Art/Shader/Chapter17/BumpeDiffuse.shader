﻿Shader "Unity Shaders Book/Chapter 17/BumpeDiffuse" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap("Normalmap",2D)="bump"{}
	}
	SubShader {
		Tags{"RenderType"="Opaque"}
		LOD 300

		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _Color;

		struct Input{
          float2 uv_MainTex;
		  float2 uv_BumpMap;
		};

		void surf(Input IN,inout SurfaceOutput o){
            fixed4 tex=tex2D(_MainTex,IN.uv_MainTex);
			o.Albedo=tex.rgb*_Color.rgb;
			o.Alpha=tex.a*_Color.a;
			o.Normal=UnpackNormal(tex2D(_BumpMap,IN.uv_BumpMap));   //采集法线信息 UnpackNormal  TODO 是否是在切线空间下
		}
		ENDCG
		
	}
	FallBack "Legacy Shaders/Diffuse"
}
