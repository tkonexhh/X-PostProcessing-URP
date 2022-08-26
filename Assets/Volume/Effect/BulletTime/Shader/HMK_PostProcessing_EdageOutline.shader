

Shader "Hidden/PostProcessing/EdageOutline"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
        _SampleDistance("Sample Distance",Float) = 1.0

    }
     
    HLSLINCLUDE

    #include "../../../Shader/PostProcessing.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

    //float4 _MainTex_TexelSize;
    float _SampleDistance;
    float _CheckValue;

    half GetEdge(float2 uv){
        float2 offUV[4] = {
                    _MainTex_TexelSize.xy * half2(1,1) * _SampleDistance,
                    _MainTex_TexelSize.xy * half2(-1,-1) * _SampleDistance,
                    _MainTex_TexelSize.xy * half2(-1,1) * _SampleDistance,
                    _MainTex_TexelSize.xy * half2(1,-1) * _SampleDistance,
                };

         float sceneRawDepth0 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv + offUV[0]);
         float sceneRawDepth1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv + offUV[1]);
         float sceneRawDepth2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv + offUV[2]);
         float sceneRawDepth3 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv + offUV[3]);

        //  float3 zCS0 = ComputeViewSpacePosition(uv,sceneRawDepth0,_InvProjMatrix).z;
        //  float3 zCS1 = ComputeViewSpacePosition(uv,sceneRawDepth1,_InvProjMatrix).z;
        //  float3 zCS2 = ComputeViewSpacePosition(uv,sceneRawDepth2,_InvProjMatrix).z;
        //  float3 zCS3 = ComputeViewSpacePosition(uv,sceneRawDepth3,_InvProjMatrix).z;

         float zCS0 = LinearEyeDepth(sceneRawDepth0,_ZBufferParams);
         float zCS1 = LinearEyeDepth(sceneRawDepth1,_ZBufferParams);
         float zCS2 = LinearEyeDepth(sceneRawDepth2,_ZBufferParams);
         float zCS3 = LinearEyeDepth(sceneRawDepth3,_ZBufferParams);

         int dp0 = abs(zCS0 - zCS1) < _CheckValue;
         int dp1 = abs(zCS2 - zCS3) < _CheckValue;

        return dp0 * dp1;

    }



    float4 Frag(VaryingsDefault i): SV_Target
    {
        float4 col = GetScreenColor(i.uv);
        float edge = GetEdge(i.uv );
        col.rgb = lerp(col.rgb,0,1 - edge);
        return col;
    }

    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        
        Cull Off
        ZWrite Off
        ZTest Always

        
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag

            ENDHLSL

        }
    }
}