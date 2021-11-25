using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CubemapDriver : MonoBehaviour
{
    private RenderTexture rt;
    private RenderTexture faceRT;

    private RenderTexture cubemap;
    private int size = 512;
    private int threads =8;
    [SerializeField] private float seed;

    [SerializeField] private Light light;
    [SerializeField] private ComputeShader computeShader;

    [SerializeField] private int framesPerTick = 1;
    private int framesSinceLastTick = 0;

    private bool ready = false;
    [SerializeField] private RawImage img;

    [SerializeField] private float radius;
    [SerializeField] private Material raymarchMaterial;

    private RenderTexture outputTexture;
    [SerializeField] private float glowDiv = 100.0f;
    
    void Start()
    {
        computeShader = Instantiate(computeShader);
        outputTexture = new RenderTexture(size * 4, size * 3, 0);
        outputTexture.dimension = TextureDimension.Tex2D;
        outputTexture.enableRandomWrite = true;

        faceRT = new RenderTexture(size, size, 0);
        faceRT.volumeDepth = 6;
        faceRT.dimension = TextureDimension.Tex2DArray;
        faceRT.enableRandomWrite = true;
        faceRT.isPowerOfTwo = true;
        faceRT.Create();

        if(img!=null)
            img.texture = outputTexture;

        cubemap = new RenderTexture(size, size, 0);
        cubemap.dimension = TextureDimension.Cube;
        cubemap.enableRandomWrite = true;
        cubemap.isPowerOfTwo = true;
        cubemap.Create();
        light.cookie = cubemap;
        
        Setup();
    }

    void Setup()
    {
        string kernalName = "Main";
        int kernal = computeShader.FindKernel(kernalName);
        computeShader.SetInt("nthreads", threads);
        computeShader.SetFloat("resolution", size);
        computeShader.SetTexture(kernal, "o_cube", faceRT);

        ready = true;
    }

    void Tick()
    {
        string kernalName = "Main";
        int kernal = computeShader.FindKernel(kernalName);
        var time = Time.time / 10f;
        computeShader.SetFloat("time", time+seed);
        raymarchMaterial.SetFloat("_ExternalTime", time+seed);
        computeShader.SetFloat("radius", radius);
        computeShader.SetFloat("glowDiv", glowDiv);
        computeShader.Dispatch(kernal, size / threads, size / threads, 1);
 
        //create cubemap texture
        Graphics.CopyTexture(faceRT, (int) CubemapFace.NegativeX, 0, cubemap, (int) CubemapFace.NegativeX, 0);
        Graphics.CopyTexture(faceRT, (int) CubemapFace.NegativeY, 0, cubemap, (int) CubemapFace.NegativeY, 0);
        Graphics.CopyTexture(faceRT, (int) CubemapFace.NegativeZ, 0, cubemap, (int) CubemapFace.NegativeZ, 0);
        Graphics.CopyTexture(faceRT, (int) CubemapFace.PositiveX, 0, cubemap, (int) CubemapFace.PositiveX, 0);
        Graphics.CopyTexture(faceRT, (int) CubemapFace.PositiveY, 0, cubemap, (int) CubemapFace.PositiveY, 0);
        Graphics.CopyTexture(faceRT, (int) CubemapFace.PositiveZ, 0, cubemap, (int) CubemapFace.PositiveZ, 0);
        
        //create display texture
        Graphics.CopyTexture(faceRT, (int)CubemapFace.NegativeX, 0, 0, 0, size, size, outputTexture, 0, 0, 0, size);
        Graphics.CopyTexture(faceRT, (int)CubemapFace.PositiveX, 0, 0, 0, size, size, outputTexture, 0, 0, size*2, size);
        Graphics.CopyTexture(faceRT, (int)CubemapFace.NegativeZ, 0, 0, 0, size, size, outputTexture, 0, 0, size*3, size);
        Graphics.CopyTexture(faceRT, (int)CubemapFace.PositiveZ, 0, 0, 0, size, size, outputTexture, 0, 0, size, size);
        Graphics.CopyTexture(faceRT, (int)CubemapFace.NegativeY, 0, 0, 0, size, size, outputTexture, 0, 0, size, size*2);
        Graphics.CopyTexture(faceRT, (int)CubemapFace.PositiveY, 0, 0, 0, size, size, outputTexture, 0, 0, size, 0);
    }

    void FixedUpdate()
    {
        if (!ready) return;
        
        framesSinceLastTick++;
        
        if (framesSinceLastTick < framesPerTick) return;

        framesSinceLastTick = 0;
        Tick();
    }
}
