

Shader "Hidden/PostProcessing/Skill/BlackWhite"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
        _NoiseTex ("NoiseTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    float4 _Params;
    float4 _Params2;
    float3 _Color;

    TEXTURE2D(_NoiseTex);SAMPLER(sampler_NoiseTex);
    TEXTURE2D(_DissolveTex);SAMPLER(sampler_DissolveTex);

    #define GreyThreshold _Params.x
    #define Center _Params.yz

    #define NoiseTillingX _Params2.x
    #define NoiseTillingY _Params2.y
    #define NoiseSpeed _Params2.z
    #define ChangeRate _Params2.w

    half luminance(half3 color)
    {
        return dot(color, half3(0.222, 0.707, 0.071));
    }



    half4 Frag(VaryingsDefault input): SV_Target
    {
        half grey = luminance(GetScreenColor(input.uv).rgb);

        // return grey;

        //极坐标纹理
        float2 centerdUV = input.uv - Center;
        float2 polarUV = float2(length(centerdUV) * NoiseTillingX * 2, atan2(centerdUV.x, centerdUV.y) * (1.0 / TWO_PI) * NoiseTillingY);
        polarUV += _Time.y * NoiseSpeed.xx;
        half polarColor = luminance(SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, polarUV).rgb);

        half dissloveColor = SAMPLE_TEXTURE2D(_DissolveTex, sampler_DissolveTex, polarUV * 0.5).r;
        polarColor *= dissloveColor;

        // return polarColor * grey;
        grey = grey + grey * polarColor;

        grey = lerp(grey, 1 - grey, ChangeRate);

        // grey = polarColor;
        grey = saturate(grey);
        half3 finalColor = saturate(grey.rrr * _Color);
        return smoothstep(1 - GreyThreshold, GreyThreshold, half4(finalColor, 1));
        
        // return sceneColor;

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