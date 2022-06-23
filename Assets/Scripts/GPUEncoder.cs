using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUEncoder
{
    private RenderTexture previousFrame;
    private RenderTexture diffTex;
    private ComputeShader computeShader;
    private int GetDiffKid;
    private int InitKid;
    private bool isInitialized;

    private int width;
    private int height;
    
    public GPUEncoder(int width,int height,ComputeShader computeShader)
    {
        this.width = width;
        this.height = height;

        this.previousFrame = new RenderTexture(width, height, 0);

        this.diffTex = new RenderTexture(width, height, 0);
        
        this.computeShader = computeShader;
        this.InitKid = this.computeShader.FindKernel("Init");
        this.GetDiffKid = this.computeShader.FindKernel("GetDiff");
    }

    public void UpdateFrame(RenderTexture renderTexture)
    {
        ValidateTexture(renderTexture);
        if (isInitialized)
        {
            DispatchInit(renderTexture);
        }
        
        DispatchGetDiff(renderTexture);
        
    }

    private int GetThreadGroupSize(int x, int threadSize)
    {
        if (x % threadSize == 0)
        {
            return x / threadSize;
        }

        return x / threadSize + 1;
    }

    public void DispatchInit(RenderTexture renderTexture)
    {
        this.computeShader.SetTexture(InitKid,"prevTex",this.previousFrame);
        this.computeShader.SetTexture(InitKid,"currentTex",renderTexture);
        this.computeShader.Dispatch(InitKid,GetThreadGroupSize(width,16),GetThreadGroupSize(height,16),1);
    }

    public void DispatchGetDiff(RenderTexture renderTexture)
    {
        this.computeShader.SetTexture(GetDiffKid,"prevTex",this.previousFrame);
        this.computeShader.SetTexture(GetDiffKid,"currentTex",renderTexture);
        this.computeShader.SetTexture(GetDiffKid,"diffTex",this.diffTex);
        this.computeShader.Dispatch(GetDiffKid,GetThreadGroupSize(width,16),GetThreadGroupSize(height,16),1);
    }

    public void ValidateTexture(RenderTexture renderTexture)
    {
        if (renderTexture.width == width && renderTexture.height == height)
        {
            return;
        }

        throw new Exception("Compression RenderTexture must be the size defined in initializing function");
    }
}
