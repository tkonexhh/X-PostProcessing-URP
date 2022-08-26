

Shader "Hidden/PostProcessing/AwakingEye"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"


    
    float _EyeOpenValue;
    float _EyeOpenLength;


    half4 Frag(VaryingsDefault input): SV_Target
    {
        float a = _EyeOpenLength;
        float b = _EyeOpenValue;


        half4 darkColor = half4(0, 0, 0, 1);
        half4 color = GetScreenColor(input.uv);
        half x = input.uv.x - 0.5f;
        half y = input.uv.y - 0.5f;
        half oval = x * x / (a * a) + y * y / (b * b);
        
        color = lerp(color, darkColor, oval);

        return color;
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