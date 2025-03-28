﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 高斯模糊效果
/// </summary>
public class GaussianBlur : PostEffectBase
{
    public Shader gaussianBlurShader;
    public Material gaussianBlurMaterial = null;
    public Material material
    {
        get
        {
            gaussianBlurMaterial = CheckShaderAndCreateMaterial(gaussianBlurShader, gaussianBlurMaterial);
            return gaussianBlurMaterial;
        }
    }
    [Range(0,4)]
    public int iterations = 3;
    [Range(0.2f,3.0f)]
    public float blurSpread = 0.6f;
    [Range(1,8)]
    public int downSample = 2;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //1rd
    //private void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    if (material!=null)
    //    {
    //        int rtW = source.width;
    //        int rtH = source.height;
    //        RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH,0);

    //        //Render the vertical pass
    //        Graphics.Blit(source,buffer,material,0);
    //        Graphics.Blit(source, destination, material, 1);

    //        RenderTexture.ReleaseTemporary(buffer);
    //    }else
    //    {
    //        Graphics.Blit(source, destination);   //使用着色器把原纹理复制到目标纹理
    //    }
    //}

    //2rd
    //private void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    if (material!=null)
    //    {
    //        int rtW = source.width / downSample;
    //        int rtH = source.height / downSample;
    //        RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH,0);
    //        buffer.filterMode = FilterMode.Bilinear;

    //        //Render the vertical pass
    //        Graphics.Blit(source, buffer, material, 0);
    //        Graphics.Blit(source, destination, material, 1);

    //        RenderTexture.ReleaseTemporary(buffer);
    //    }else
    //    {
    //        Graphics.Blit(source, destination);
    //    }
    //}

    //3rd
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material!=null)
        {
            int rtW = source.width / downSample;
            int rtH = source.height / downSample;

            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH,0);
            buffer0.filterMode = FilterMode.Bilinear;
            Graphics.Blit(source, buffer0);

            for (int i=0;i<iterations;i++)
            {
                material.SetFloat("_BlurSize",1.0f+i*blurSpread);

                RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
                //Render the vertical pass
                Graphics.Blit(buffer0, buffer1,material,0);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
                buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                //Render the horizontal pass
                Graphics.Blit(buffer0,buffer1,material,1);
                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
            }
            Graphics.Blit(buffer0,destination);
            RenderTexture.ReleaseTemporary(buffer0);
        }else
        {
            Graphics.Blit(source, destination);
        }
    }
}
