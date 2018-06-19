using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

//This script is a modified version of the BlurOptimized Script in the UnityStandardAsset pack
//[ExecuteInEditMode]
public class DrawingEffect2 : MonoBehaviour
{
    public Texture h0, h1;
    [Range(0,1)]
    public float extraLight;
    public Shader shader;

     void Start()
    {
        Shader.SetGlobalTexture("_Hatch0", h0);
        Shader.SetGlobalTexture("_Hatch1", h1);
        Shader.SetGlobalFloat("_Addlight", extraLight);
        GetComponent<Camera>().SetReplacementShader(shader, "RenderType");
    }
}