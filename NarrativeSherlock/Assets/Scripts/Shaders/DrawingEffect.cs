using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

//This script is a modified version of the BlurOptimized Script in the UnityStandardAsset pack
//[ExecuteInEditMode]
public class DrawingEffect : PostEffectsBase
{
   

    [Range(0, 2)]
    public int downsample = 1;

    public enum BlurType
    {
        StandardGauss = 0,
        SgxGauss = 1,
    }

    [Range(0.0f, 100.0f)]
    public float blurSize = 3.0f;

    [Range(1, 4)]
    public int blurIterations = 2;

    public BlurType blurType = BlurType.StandardGauss;

    public Shader blurShader = null;
    private Material blurMaterial = null;

    public Shader colorDodgeShader = null;
    private Material colorDodgeMaterial = null;

    public Shader grayscaleShader = null;
    private Material grayscaleMaterial = null;

    public Shader depthShader = null;
    private Material depthMaterial = null;

    public Shader maskShader = null;
    private Material maskMaterial = null;

    public Shader invertShader = null;
    private Material invertMaterial = null;

    public Shader   levelsShader = null;
    private Material levelsMaterial = null;


    //  public Texture paper = null;
    private Camera c;

    [Range(0, 1)]
    public float limit, strength;

    [Range(0, 10)]
    public float depthLevel = 1;
    [Range(0, 10)]
    public int  ammountOfShades = 3;
    [Range(0, 10)]
    public float maskLimit = 1;

    public AnimationCurve levelsCurve=null;
    private Texture2D levels= null;

    void Awake()
    {
        c = GetComponent<Camera>();
        colorDodgeMaterial = new Material(colorDodgeShader);
        grayscaleMaterial = new Material(grayscaleShader);
        c.depthTextureMode = DepthTextureMode.Depth;
        depthMaterial = new Material(depthShader);
        maskMaterial = new Material(maskShader);
        invertMaterial = new Material(invertShader);
        levelsMaterial = new Material(levelsShader);



         levels = new Texture2D(256, 1, TextureFormat.ARGB32, false, true);

        

    }


    public override bool CheckResources()
    {
        CheckSupport(false);

        blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);

        if (!isSupported)
            ReportAutoDisable();
        return isSupported;
    }

    public void OnDisable()
    {
        if (blurMaterial)
            DestroyImmediate(blurMaterial);
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (CheckResources() == false)
        {
            Graphics.Blit(source, destination);
            return;
        }

        for (int i = 0; i < 256; i++)
        {
            float f = levelsCurve.Evaluate((float)i / 255);
            levels.SetPixel(i, 0, new Color(f, f, f));
        }
        levels.Apply();

        //grayscale
        RenderTexture gray = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        RenderTexture colorDodge = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        RenderTexture mask = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        RenderTexture depth = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        RenderTexture lev = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        Graphics.Blit(source, gray, grayscaleMaterial);
        Graphics.Blit(gray, mask, invertMaterial);
        //Gaussian blur part

        float widthMod = 1.0f / (1.0f * (1 << downsample));

        blurMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod, -blurSize * widthMod, 0.0f, 0.0f));
        source.filterMode = FilterMode.Bilinear;

        int rtW = source.width >> downsample;
        int rtH = source.height >> downsample;

        // downsample
        RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
        rt.filterMode = FilterMode.Bilinear;
        Graphics.Blit(mask, rt, blurMaterial, 0);

        var passOffs = blurType == BlurType.StandardGauss ? 0 : 2;

        for (int i = 0; i < blurIterations; i++)
        {
            float iterationOffs = (i * 1.0f);
            blurMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0.0f, 0.0f));

            // vertical blur
            RenderTexture rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
            rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rt, rt2, blurMaterial, 1 + passOffs);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;

            // horizontal blur
            rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
            rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rt, rt2, blurMaterial, 2 + passOffs);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;
        }
        //end of Gaussian blur
        

        colorDodgeMaterial.SetTexture("_GrayScale", gray);
        Graphics.Blit(rt, colorDodge, colorDodgeMaterial);

        //Graphics.Blit( colorDodge, destination);

        depthMaterial.SetFloat("_DepthLevel", depthLevel);
        depthMaterial.SetFloat("_shadesAmmount", ammountOfShades);
         Graphics.Blit(source, depth, depthMaterial);
        // Graphics.Blit(source, destination, depthMaterial);
        maskMaterial.SetTexture("_Mask", depth);
        maskMaterial.SetFloat("_limit", maskLimit);
        Graphics.Blit(colorDodge, lev, maskMaterial);

        levelsMaterial.SetTexture("_Levels", levels);
        Graphics.Blit(lev, destination, levelsMaterial);

        RenderTexture.ReleaseTemporary(rt);
        RenderTexture.ReleaseTemporary(gray);
        RenderTexture.ReleaseTemporary(colorDodge);
        RenderTexture.ReleaseTemporary(mask);
        RenderTexture.ReleaseTemporary(depth);
        RenderTexture.ReleaseTemporary(lev);
    }
}

