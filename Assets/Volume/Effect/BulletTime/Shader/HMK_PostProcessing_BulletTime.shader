

Shader "Hidden/PostProcessing/BulletTime2"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }

        HLSLINCLUDE

        ENDHLSL

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            
            Cull Back
            
            HLSLPROGRAM

            #pragma vertex VertWorld
            #pragma fragment frag

            #include "../../../Shader/PostProcessing.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
            float4 _BulletTimeOriginPos;
            //float _BlurQuality;
            //float _BlurPower;
            float4 _BulletTimeColor;
            float _BulletTimeConcen;
            //float _BulletBloomMaskSize;
            float _SpotSize;
            CBUFFER_END
            //TEXTURE2D(_BulletTimeBloomMask);
            //SAMPLER(sampler_BulletTimeBloomMask);

            float3 GetWorldPosBYDepth(float2 uv, float3 ray)
            {
                float sceneRawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
                float eyeDepth = LinearEyeDepth(sceneRawDepth, _ZBufferParams);
                float3 worldPos = _WorldSpaceCameraPos.xyz + eyeDepth * ray; //作者：lyh萌主 https: //www.bilibili.com/read/cv14565799/ 出处：bilibili
                return worldPos;
            }
            
            float4 frag(VaryingsWorld input): SV_Target
            {

                float3 worldPos = GetWorldPosBYDepth(input.uv, input.interpolatedRay.xyz);
                float dis = distance(worldPos, _BulletTimeOriginPos.xyz);
                //float dis = distance(worldPos, float3(1567, 335, 1124));
                dis = saturate(dis / _SpotSize);
                //dis = frac(dis * 0.1);
                //dis = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv);

                float4 col = GetScreenColor(input.uv);
                //屏幕颜色部分
                float3 btColor = lerp(1, _BulletTimeColor.rgb, _BulletTimeConcen);

                //bloomMask部分
                //float2 sUV = (input.uv - float2(0.5, 0.5)) * (1 / (_BulletTimeConcen + 0.01)) / _BulletBloomMaskSize + float2(0.5, 0.5);
                //sUV = saturate(sUV);
                //float3 mask = SAMPLE_TEXTURE2D(_BulletTimeBloomMask, sampler_BulletTimeBloomMask, sUV).rgb;

                float3 bcol = col.rgb * btColor;
                col.rgb = lerp(col.rgb, bcol, dis);
                //col.rgb += mask;

                //return float4(dis, dis, dis, 1);
                return col;
            }
            
            ENDHLSL

        }
    }
    FallBack "Diffuse"
}
