using System;
using UnityEngine;

public class TestEyeIndex : MonoBehaviour
{
    public Material material;
    private RenderTexture rt;
    
    public Camera cam;

    private void Awake()
    {
        rt = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB64);
    }

    private void OnPreRender()
    {
        //Shader.SetGlobalInt("_IsLeftEye", cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left ? 1 : 0);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }
}
