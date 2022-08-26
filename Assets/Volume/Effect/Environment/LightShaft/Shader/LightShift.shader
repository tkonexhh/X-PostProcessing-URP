

Shader "Hidden/PostProcessing/Environment/LightShift"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    half4 _BloomTintAndThreshold;
    half4 _LightShaftParameters;

    #define BloomIntensity _LightShaftParameters.y
    #define InvOcclusionDepthRange _LightShaftParameters.x
    //光源对应的屏幕坐标
    #define TextureSpaceBlurOrigin _LightShaftParameters.zw



    #include "../../../../Shader/PostProcessing.hlsl"

    TEXTURE2D_X_FLOAT(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);

    half4 Frag(VaryingsDefault input): SV_Target
    {
        half3 SceneColor = GetScreenColor(input.uv);
        half SceneDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv);
        SceneDepth = Linear01Depth(SceneDepth, _ZBufferParams);
        
        float EdgeMask = 1.0f - input.uv.x * (1.0f - input.uv.x) * input.uv.y * (1.0f - input.uv.y) * 8.0f;
        EdgeMask = EdgeMask * EdgeMask * EdgeMask * EdgeMask;
        // return EdgeMask;

        // Only bloom colors over BloomThreshold
        float Luminance = max(dot(SceneColor, half3(.3f, .59f, .11f)), 6.10352e-5);
        float AdjustedLuminance = max(Luminance - _BloomTintAndThreshold.a, 0.0f);
        half3 BloomColor = BloomIntensity * SceneColor / Luminance * AdjustedLuminance * 2.0f;
        // return half4(BloomColor, 1);

        // Only allow bloom from pixels whose depth are in the far half of OcclusionDepthRange
        float BloomDistanceMask = saturate((SceneDepth - 0.5f / InvOcclusionDepthRange) * InvOcclusionDepthRange);
        // float BloomDistanceMask = (1 - step(SceneDepth, InvOcclusionDepthRange)) * SceneDepth;
        
        // return BloomDistanceMask;
        // Setup a mask that is 0 at TextureSpaceBlurOrigin and increases to 1 over distance
        //TextureSpaceBlurOrigin is the center of the blur in texture space
        //需要考虑AspectRatio 先定为1
        float AspectRatioAndInvAspectRatio = 1;
        float BlurOriginDistanceMask = 1.0f - saturate(length(TextureSpaceBlurOrigin.xy - input.uv * AspectRatioAndInvAspectRatio) * 2.0f);
        // return BlurOriginDistanceMask * BlurOriginDistanceMask * BloomDistanceMask;
        // float2 d = input.uv - TextureSpaceBlurOrigin.xy;
        // float limit = 1 - saturate(saturate(length(d) / 1.414) * 2);
        // limit = pow(limit, 3);
        // return limit;
        // Calculate bloom color with masks applied
        half3 rgb = BloomColor * _BloomTintAndThreshold.rgb * BloomDistanceMask * (1.0f - EdgeMask) * BlurOriginDistanceMask * BlurOriginDistanceMask;
        // half3 rgb = BloomColor * _BloomTintAndThreshold.rgb * (1.0f - EdgeMask) * limit;
        return half4(rgb, 1);

        // return GetScreenColor(input.uv);

    }

    /////////////////////
    int _SampleDistance;
    #define SC 8.0

    half4 FragBlur(VaryingsDefault input): SV_TARGET
    {
        half3 SceneColor = GetScreenColor(input.uv);
        float2 d = input.uv - TextureSpaceBlurOrigin;
        float p = 0.01;

        half3 rgb = SceneColor;
        float2 uvd = d * p * _SampleDistance / SC;
        for (int idx = 1; idx <= SC; idx++)
        {
            rgb += GetScreenColor(input.uv - uvd * idx);
        }
        rgb /= (SC + 1);
        

        
        return half4(rgb, 1);
    }


    /////////////////////
    TEXTURE2D(_BluredTexture);SAMPLER(sampler_BluredTexture);
    // TEXTURE2D(_MaskTexture);SAMPLER(sampler_MaskTexture);
    half _Attenuation;

    half4 FragFinal(VaryingsDefault input): SV_TARGET
    {
        half3 originColor = GetScreenColor(input.uv);
        half4 blured = SAMPLE_TEXTURE2D(_BluredTexture, sampler_BluredTexture, input.uv);
        // half mask = SAMPLE_TEXTURE2D(_MaskTexture, sampler_MaskTexture, input.uv).r;
        return half4(originColor + blured.rgb * _Attenuation, 1);
    }


    /////////////

    half4 FragMask(VaryingsDefault input): SV_TARGET
    {
        half3 originColor = GetScreenColor(input.uv);
        half l = originColor.r * .299 + originColor.g * .587 + originColor.b * .114;
        return half4(l, l, l, 1);
    }

    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        
        Cull Off
        ZWrite Off
        ZTest Always

        //提取高亮区域
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag

            ENDHLSL

        }

        //径向模糊
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment FragBlur

            ENDHLSL

        }

        //颜色相加
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment FragFinal

            ENDHLSL

        }

        //提取Mask
        Pass
        {

            // ColorMask R

            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment FragMask

            ENDHLSL

        }
    }
}