Shader "Hidden/PostProcessing/DepthFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
    }

    HLSLINCLUDE
    #include "../../../../Shader/PostProcessing.hlsl"

    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertWorld
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "./../../../../../../../Shader/URP/HLSLIncludes/Common/Fog.hlsl"


            TEXTURE2D_X_FLOAT(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);


            float4 frag(VaryingsWorld input): SV_Target
            {
                // sample the texture
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                // apply fog
                float sceneRawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv);
                //TODO opengl这样就和顶点雾一致了 需要验证
                // sceneRawDepth = sceneRawDepth * 2 - 1;
                // float3 positionWS = ComputeWorldSpacePosition(input.uv, sceneRawDepth, UNITY_MATRIX_I_VP);
                // return half4(positionWS, 1);
                float eyeDepth = LinearEyeDepth(sceneRawDepth, _ZBufferParams);
                float3 positionWS = _WorldSpaceCameraPos + eyeDepth * input.interpolatedRay.xyz; //作者：lyh萌主 https: //www.bilibili.com/read/cv14565799/ 出处：bilibili
                // return half4(positionWS, 1);
                col.rgb = ApplyFog(col.rgb, positionWS);
                return col;
            }
            ENDHLSL

        }
    }
}
