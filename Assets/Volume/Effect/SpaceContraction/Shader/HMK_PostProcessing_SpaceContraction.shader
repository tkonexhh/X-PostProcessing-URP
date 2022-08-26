


Shader "Hidden/PostProcessing/SpaceContraction"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }

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
            float4 _OriginPosition;
            float4 _WaveColor;
            float4 _CurveColor;
            float4 _GridColor;
            float _MaxDistance;
            float _SCBrightness;
            float _AreaBrightness;
            float _RaderWaveWidth;
            float _RaderWaveTimeLine;
            float _RaderWaveTImeCount;
            float _RaderWaveIntensity;
            float _GridStartTime;
            float _GridSpeed;
            float _GridRadius;
            float _GirdLength;
            float4 _PositionBuffer0;
            float4 _PositionBuffer1;
            CBUFFER_END

            //TEXTURE2D(_EdgTexture);SAMPLER(sampler_EdgTexture);

            //TEXTURE2D_X_FLOAT(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);
            float3 GetWorldPosBYDepth(float2 uv, float3 ray)
            {
                float sceneRawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
                float eyeDepth = LinearEyeDepth(sceneRawDepth, _ZBufferParams);
                float3 worldPos = _WorldSpaceCameraPos.xyz + eyeDepth * ray; //作者：lyh萌主 https: //www.bilibili.com/read/cv14565799/ 出处：bilibili
                return worldPos;
            }

            float3 GetWorldPosBYDepth(float2 uv)
            {
                float sceneRawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);

                #if !UNITY_REVERSED_Z
                    sceneRawDepth = sceneRawDepth * 2 - 1;
                #endif
                
                float3 worldPos = ComputeWorldSpacePosition(uv, sceneRawDepth, UNITY_MATRIX_I_VP);
                return worldPos;
            }

            float4 frag(VaryingsWorld input): SV_Target
            {
                half4 var_MainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                float3 worldPos = GetWorldPosBYDepth(input.uv, input.interpolatedRay.xyz);
                //float sceneRawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv);
                
                float3 cameraPos = _WorldSpaceCameraPos.xyz;
                //float eyeDepth = LinearEyeDepth(sceneRawDepth, _ZBufferParams);
                //float3 worldPos = cameraPos + eyeDepth * input.interpolatedRay.xyz; //作者：lyh萌主 https: //www.bilibili.com/read/cv14565799/ 出处：bilibili
                
                
                float cameraDis = distance(cameraPos, worldPos);
                
                //坐标与中心点距离
                float dis = distance(worldPos, _OriginPosition.xyz);

                //坐标与扩散边界距离
                float rdis = dis - _OriginPosition.w;
                clip(-rdis);
                //效果遮罩系数，只在范围内生效
                float wi = 1 - saturate(rdis * 100);
                //扩散边界颜色淡出系数
                float power = saturate(1 + rdis / (_RaderWaveWidth + _OriginPosition.w * 0.005));
                
                float wavePower = saturate(1 + rdis / (_RaderWaveWidth * 100 + _OriginPosition.w * 0.3)) * 0.3;

                //power = 1 - step(power, 0.01);
                float areaIntensity = saturate((dis - 200) / _MaxDistance);
                
                //范围内颜色压暗系数。
                float areaPower = 1 - lerp(1 - _AreaBrightness, 0, areaIntensity) * wi;
                //areaPower = 1;
                //环行线淡出系数
                //float afterDis = dis - _PositionBuffer0.w;
                float disPower = saturate(1 + (rdis / 150));
                
                //float3 waveColor = float3(0.1f, 0.1f, 1.0f);
                float3 waveColor = _WaveColor.rgb;
                

                float cdis = distance(worldPos, _PositionBuffer1.xyz);
                float cdis2 = cdis - _PositionBuffer0.w;
                float cdisPower = saturate(1 + (cdis2 / 150));

                
                float grid = _GirdLength;

                //float texelWidth = 2;

                float rpo = 0;
                float rpo1 = 0;

                float3 curveColor = _CurveColor.rgb;
                float3 gridColor = _GridColor.rgb;
                //gridColor = float3(0.1, 0.8, 1.0);

                float xo = floor(worldPos.x / grid);
                float zo = floor(worldPos.z / grid);

                //float cdis = dis - _PositionBuffer0.w;

                //float diso = floor(dis / grid);
                //float diso = floor(cdis / (_OriginPosition.w - 0.1));
                
                //射线重建世界坐标（采样旁边的像素时坐标有误差）
                float3 wposl = GetWorldPosBYDepth(input.uv + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y), input.interpolatedRay.xyz);
                float3 wposr = GetWorldPosBYDepth(input.uv + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y), input.interpolatedRay.xyz);
                float3 wposu = GetWorldPosBYDepth(input.uv + float2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y), input.interpolatedRay.xyz);
                float3 wposd = GetWorldPosBYDepth(input.uv + float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y), input.interpolatedRay.xyz);
                //矩阵重建世界坐标（零时使用）
                // float3 wposl = GetWorldPosBYDepth(input.uv + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y));
                // float3 wposr = GetWorldPosBYDepth(input.uv + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y));
                // float3 wposu = GetWorldPosBYDepth(input.uv + float2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y));
                // float3 wposd = GetWorldPosBYDepth(input.uv + float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y));

                float xl = floor(wposl.x / grid);
                float xr = floor(wposr.x / grid);
                float xu = floor(wposu.x / grid);
                float xd = floor(wposd.x / grid);

                float zl = floor(wposl.z / grid);
                float zr = floor(wposr.z / grid);
                float zu = floor(wposu.z / grid);
                float zd = floor(wposd.z / grid);
                
                rpo1 += abs(xl - xo);
                rpo1 += abs(xr - xo);
                rpo1 += abs(xu - xo);
                rpo1 += abs(xd - xo);
                rpo1 += abs(zl - zo);
                rpo1 += abs(zr - zo);
                rpo1 += abs(zu - zo);
                rpo1 += abs(zd - zo);
                
                // float disl = distance(wposl, _PositionBuffer1.xyz);
                // float disr = distance(wposr, _PositionBuffer1.xyz);
                // float disu = distance(wposu, _PositionBuffer1.xyz);
                // float disd = distance(wposd, _PositionBuffer1.xyz);

                // float dl = floor(disl / grid);
                // float dr = floor(disr / grid);
                // float du = floor(disu / grid);
                // float dd = floor(disd / grid);

                
                //rpo += abs(dl - diso);
                //rpo += abs(dr - diso);
                //rpo += abs(du - diso);
                //rpo += abs(dd - diso);
                

                
                //rpo /= 2;
                //rpo = saturate(rpo * 0.5) * 20;

                rpo1 /= 4;
                //rpo1 = saturate(rpo1 * 0.5) * 2;
                rpo1 = saturate(rpo1 * 0.5) * 2;
                //rpo1 *= farPower;

                float gdis = distance(worldPos, _PositionBuffer0.xyz);
                
                float odisPower = saturate(gdis / _GridRadius);
                
                float gridPower = 1 - odisPower;

                float farpower = 1 - saturate((_OriginPosition.w - _MaxDistance) / 100);

                float btness = 1 - (wi * (1 - _SCBrightness));

                float3 areaRes = (var_MainTex.rgb * btness) * lerp(1, areaPower, _RaderWaveIntensity);
                
                float3 waveRes = waveColor.rgb * (saturate(power) * 18 + wavePower);
                
                
                //float3 curveRes = rpo * _CurveColor * cdisPower * (1 - areaIntensity);
                
                waveRes *= farpower;

                float cfarPower = 1 - saturate((_PositionBuffer0.w - _MaxDistance) * 0.01);
                
                float cwi = 1 - saturate(cdis2 * 100);
                //curveRes *= cwi * cfarPower * odisPower;

                float3 gridRes = rpo1 * gridPower * gridColor.rgb * 2;

                float3 addColor = (waveRes + gridRes) * wi * _RaderWaveIntensity;
                float3 res = areaRes + addColor;
                
                
                return float4(res, 1);
            }
            
            ENDHLSL

        }
    }
    FallBack "Diffuse"
}
