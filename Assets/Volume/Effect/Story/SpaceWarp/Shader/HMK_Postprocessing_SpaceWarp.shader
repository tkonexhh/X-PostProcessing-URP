

Shader "Hidden/PostProcessing/SpaceWarp"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    float _SpaceWarpLength;
    //float _SpaceUnWarpRadius;
    //float2 _SpaceWarpCenterPoint;

    half4 Frag(VaryingsDefault input): SV_Target
    {
        //float4 col = GetScreenColor(input.uv);
        float2 uvDis = (input.uv - 0.5f) * 2;
        float2 direct = normalize(float3(uvDis, 0)).xy;
        float r = length(uvDis);
        
        float maxLength = sqrt(2);
        
        float radius = 1;

        float maxAngle = acos(_SpaceWarpLength / radius);
        
        float length = sqrt(1 - _SpaceWarpLength * _SpaceWarpLength);
        //float scale = 1 / length;
        r /= maxLength;
        
        float angle = asin(r * length / radius);
        float rl = angle / maxAngle;

        float2 uv = rl * direct * 0.5f * maxLength + 0.5f;

        return GetScreenColor(uv);
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