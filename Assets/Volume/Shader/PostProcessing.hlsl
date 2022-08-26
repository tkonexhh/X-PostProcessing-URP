#pragma once

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
// #include "./../../../../Shader/URP/HLSLIncludes/Common/Global.hlsl"

//Always present in every shader
TEXTURE2D(_MainTex);SAMPLER(sampler_MainTex);float4 _MainTex_TexelSize;
sampler sampler_LinearClamp;
float4x4 _FrustumCornersRay;//用于重建世界坐标


half4 GetScreenColor(float2 uv)
{
    return SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv);
}

///普通后处理
struct AttributesDefault
{
    float4 positionOS: POSITION;
    float2 uv: TEXCOORD0;
};


struct VaryingsDefault
{
    float4 positionCS: SV_POSITION;
    float2 uv: TEXCOORD0;
};


VaryingsDefault VertDefault(AttributesDefault input)
{
    VaryingsDefault output;
    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
    output.uv = input.uv;

    return output;
}

//需要到世界坐标的
struct VaryingsWorld
{
    float4 positionCS: SV_POSITION;
    float2 uv: TEXCOORD0;
    float4 interpolatedRay: TEXCOORD2;
};

VaryingsWorld VertWorld(AttributesDefault input)
{
    VaryingsWorld output;
    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
    output.uv = input.uv;


    int index = 0;
    if (input.uv.x < 0.5 && input.uv.y < 0.5)
    {
        index = 0;
    }
    else if (input.uv.x > 0.5 && input.uv.y < 0.5)
    {
        index = 1;
    }
    else if (input.uv.x > 0.5 && input.uv.y > 0.5)
    {
        index = 2;
    }
    else
    {
        index = 3;
    }

    #if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0)
            index = 3 - index;
    #endif

    output.interpolatedRay = _FrustumCornersRay[index];

    return output;
}


//------------------------------------------------------------------------------------------------------
// Generic functions
//------------------------------------------------------------------------------------------------------

float rand(float n)
{
    return frac(sin(n) * 13758.5453123 * 0.01);
}

float rand(float2 n)
{
    return frac(sin(dot(n, float2(12.9898, 78.233))) * 43758.5453);
}



