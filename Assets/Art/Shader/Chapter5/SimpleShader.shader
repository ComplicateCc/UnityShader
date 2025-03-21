﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unity Shaders Book/Chapter5/SimpleShader" {
	SubShader{
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			float4 vert(float4 v:POSITION):SV_POSITION{
                 return UnityObjectToClipPos(v);
			}
			fixed4 frag():SV_TARGET{
				return fixed4(0.0,0.0,0.0,0.0);
			}
			ENDCG
		}
	}

}
