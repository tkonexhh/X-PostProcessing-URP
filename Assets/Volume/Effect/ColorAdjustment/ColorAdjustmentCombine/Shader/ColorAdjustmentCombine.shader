

Shader "Hidden/PostProcessing/ColorAdjustmentCombine"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
     
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    half4 Frag(VaryingsDefault input): SV_Target
    {

        return GetScreenColor(input.uv);
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