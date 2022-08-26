

Shader "Hidden/PostProcessing/ColorAdjustment/TintPlayerPos"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    uniform half _Indensity;
    uniform half4 _ColorTint;
    uniform half _Range;

    TEXTURE2D_X_FLOAT(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);

    
    half4 Frag(VaryingsWorld input): SV_Target
    {
        half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
        
        half3 tintColor = lerp(sceneColor.rgb, sceneColor.rgb * _ColorTint.rgb, _Indensity);

        float sceneRawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv);
        float eyeDepth = LinearEyeDepth(sceneRawDepth, _ZBufferParams);
        float3 positionWS = _WorldSpaceCameraPos + eyeDepth * input.interpolatedRay.xyz;
        
        float dis = distance(positionWS, _PlayerPosition);
        dis = saturate(dis / _Range);
        dis = dis * dis;
        dis = 1 - dis;
        half3 finalColor = lerp(tintColor, sceneColor, dis);

        return half4(finalColor, 1.0);
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

            #pragma vertex VertWorld
            #pragma fragment Frag

            ENDHLSL

        }
    }
}