

Shader "Hidden/PostProcessing/Environment/Bloom"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"
    #include "./../../../../../../../Shader/URP/HLSLIncludes/Common/HMK_Common.hlsl"

    float _Intensity;
    float2 _Params;
    float _SamplerScale;

    #define Threshold           _Params.x
    #define ThresholdKnee       _Params.y


    TEXTURE2D(_BloomTex);SAMPLER(sampler_BloomTex);
    TEXTURE2D(_SourceTex);SAMPLER(sampler_SourceTex);

    half3 SafeHDR(half3 c)
    {
        return min(c, 65504.0);
    }
    half4 SafeHDR(half4 c)
    {
        return min(c, 65504.0);
    }

    half4 DecodeHDR(half4 c)
    {
        return c;
    }

    half4 FragPrefilter(VaryingsDefault input): SV_Target
    {
        half3 SceneColor = SafeHDR(GetScreenColor(input.uv));
        half brightness = max(SceneColor.r, max(SceneColor.g, SceneColor.b));
        half softness = clamp(brightness - Threshold + ThresholdKnee, 0.0, 2.0 * ThresholdKnee);
        softness = (softness * softness) / (4.0 * ThresholdKnee + 1e-4);
        half multiplier = max(brightness - Threshold, softness) / max(brightness, 1e-4);
        SceneColor *= multiplier;

        SceneColor = max(SceneColor, 0);
        return half4(SceneColor, 1);
    }

    //////////////

    struct VaryingsBlur
    {
        float4 positionCS: SV_POSITION;
        float2 uv: TEXCOORD0;
        float2 neighbours[4]: TEXCOORD1;
    };

    VaryingsBlur VertBlur(AttributesDefault input)
    {
        VaryingsBlur output;
        output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
        output.uv = input.uv;
        output.neighbours[0] = input.uv - float2(_MainTex_TexelSize.x * _SamplerScale, 0.0);
        output.neighbours[1] = input.uv + float2(_MainTex_TexelSize.x * _SamplerScale, 0.0);
        output.neighbours[2] = input.uv - float2(0.0, _MainTex_TexelSize.y * _SamplerScale);
        output.neighbours[3] = input.uv + float2(0.0, _MainTex_TexelSize.y * _SamplerScale);
        return output;
    }

    half3 BoxFilter(VaryingsBlur input)
    {
        half3 sum;
        // Box filter
        half3 s1 = GetScreenColor(input.neighbours[0]).rgb;
        half3 s2 = GetScreenColor(input.neighbours[1]).rgb;
        half3 s3 = GetScreenColor(input.neighbours[2]).rgb;
        half3 s4 = GetScreenColor(input.neighbours[3]).rgb;

        sum = (s1 + s2 + s3 + s4) * 0.25;

        return sum;
    }

    half4 FragBlur(VaryingsBlur input): SV_Target
    {
        return half4(BoxFilter(input), 1);
    }


    /////////////////

    half4 FragFinal(VaryingsBlur input): SV_Target
    {
        half3 boxfilter = BoxFilter(input);
        return half4(boxfilter * _Intensity, 1);
    }

    //////////
    half4 FragCombine(VaryingsDefault input): SV_Target
    {
        half3 bloom = GetScreenColor(input.uv).rgb;
        half3 source = SAMPLE_TEXTURE2D(_SourceTex, sampler_SourceTex, input.uv).rgb;
        // bloom = LinearToGammaSpace(bloom);
        return half4(source + bloom, 1);
    }

    half4 FragBlurH(VaryingsDefault input): SV_Target
    {
        float texelSize = _MainTex_TexelSize.x * 2.0;
        float2 uv = input.uv;

        // 9-tap gaussian blur on the downsampled source
        half3 c0 = DecodeHDR(GetScreenColor(uv - float2(texelSize * 4.0, 0.0)));
        half3 c1 = DecodeHDR(GetScreenColor(uv - float2(texelSize * 3.0, 0.0)));
        half3 c2 = DecodeHDR(GetScreenColor(uv - float2(texelSize * 2.0, 0.0)));
        half3 c3 = DecodeHDR(GetScreenColor(uv - float2(texelSize * 1.0, 0.0)));
        half3 c4 = DecodeHDR(GetScreenColor(uv));
        half3 c5 = DecodeHDR(GetScreenColor(uv + float2(texelSize * 1.0, 0.0)));
        half3 c6 = DecodeHDR(GetScreenColor(uv + float2(texelSize * 2.0, 0.0)));
        half3 c7 = DecodeHDR(GetScreenColor(uv + float2(texelSize * 3.0, 0.0)));
        half3 c8 = DecodeHDR(GetScreenColor(uv + float2(texelSize * 4.0, 0.0)));

        half3 color = c0 * 0.01621622 + c1 * 0.05405405 + c2 * 0.12162162 + c3 * 0.19459459
        + c4 * 0.22702703
        + c5 * 0.19459459 + c6 * 0.12162162 + c7 * 0.05405405 + c8 * 0.01621622;

        return half4(SafeHDR(color), 1);
    }

    half4 FragBlurV(VaryingsDefault input): SV_Target
    {
        float texelSize = _MainTex_TexelSize.y;
        float2 uv = input.uv;

        // Optimized bilinear 5-tap gaussian on the same-sized source (9-tap equivalent)
        half3 c0 = DecodeHDR(GetScreenColor(uv - float2(0.0, texelSize * 3.23076923)));
        half3 c1 = DecodeHDR(GetScreenColor(uv - float2(0.0, texelSize * 1.38461538)));
        half3 c2 = DecodeHDR(GetScreenColor(uv));
        half3 c3 = DecodeHDR(GetScreenColor(uv + float2(0.0, texelSize * 1.38461538)));
        half3 c4 = DecodeHDR(GetScreenColor(uv + float2(0.0, texelSize * 3.23076923)));

        half3 color = c0 * 0.07027027 + c1 * 0.31621622
        + c2 * 0.22702703
        + c3 * 0.31621622 + c4 * 0.07027027;

        return half4(SafeHDR(color), 1);
    }



    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }


        //提取高亮区域
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment FragPrefilter

            ENDHLSL

        }

        //模糊
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertBlur
            #pragma fragment FragBlur

            ENDHLSL

        }

        
        //Final
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertBlur
            #pragma fragment FragFinal

            ENDHLSL

        }

        //Combine
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment FragCombine

            ENDHLSL

        }


        //高质量模糊
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment FragBlurH

            ENDHLSL

        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment FragBlurV

            ENDHLSL

        }
    }
}