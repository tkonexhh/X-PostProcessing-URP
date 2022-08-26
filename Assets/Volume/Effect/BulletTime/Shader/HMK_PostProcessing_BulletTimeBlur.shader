

Shader "Hidden/PostProcessing/BulletTimeBlur22"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../Shader/PostProcessing.hlsl"

    CBUFFER_START(UnityPerMaterial)
    float _BulletTimeBlurControl;
    float _BulletTimeUnBlurRadius;
    float2 _BulletTimeBlurCenterPoint;
    CBUFFER_END


    float4 Frag(VaryingsDefault i): SV_Target
    {
        float4 col = GetScreenColor(i.uv);
        float2 uvDis = i.uv - _BulletTimeBlurCenterPoint;
        float blurRadius = saturate(length(uvDis) - _BulletTimeUnBlurRadius);
        //float blurStren = 0.3f;
        //float orginStren = 1 - 0.3;
        float4 outColor = col;

        // for (int m = 1; m < 3; m++)
        // {
        //     float2 suv = i.uv - uvDis * blurRadius * _BulletTimeBlurControl * m;
        //     outColor += GetScreenColor(suv);
        // }
        float2 suv1 = i.uv - uvDis * blurRadius * 0.5;
        float2 suv2 = i.uv - uvDis * blurRadius * 0.5 * 2;
        outColor += GetScreenColor(suv1);
        outColor += GetScreenColor(suv2);

        outColor /= 3;
        outColor.a = 1;

        return outColor;
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