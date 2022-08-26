using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Environment + "上帝之光 (LightShaft)")]
    public class LightShaft : VolumeSetting
    {
        public override bool IsActive() => BloomIntensity.value > 0;

        public ClampedFloatParameter BloomIntensity = new ClampedFloatParameter(0f, 0f, 5f);
        public ClampedFloatParameter Threshold = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter OcclusionDepthRange = new ClampedFloatParameter(0.1f, 0f, 1f);
        public ColorParameter BloomTint = new ColorParameter(Color.white);

        //降采样 默认一次
        public ClampedIntParameter DownSample = new ClampedIntParameter(1, 0, 5);

        public ClampedFloatParameter Attenuation = new ClampedFloatParameter(1, 0, 10);

        // public TransformParameter virtualLight = new TransformParameter(null);

        public static Transform virtualLight;
    }


    public class LightShaftRenderer : VolumeRenderer<LightShaft>
    {
        public override string PROFILER_TAG => "LightShift";
        public override string ShaderName => "Hidden/PostProcessing/Environment/LightShift";

        public static bool IsEnable = true;

        static class ShaderIDs
        {
            public static readonly int LightShaftTempId = Shader.PropertyToID("_LightShaftTempTex");
            public static readonly int TempBlurBuffer1 = Shader.PropertyToID("_TempBlurBuffer1");
            public static readonly int TempBlurBuffer2 = Shader.PropertyToID("_TempBlurBuffer2");

            public static readonly int BloomTintAndThreshold = Shader.PropertyToID("_BloomTintAndThreshold");
            public static readonly int LightShaftParameters = Shader.PropertyToID("_LightShaftParameters");

            public static readonly int SampleDistance = Shader.PropertyToID("_SampleDistance");

            public static readonly int BluredTexture = Shader.PropertyToID("_BluredTexture");
            // public static readonly int MaskTexture = Shader.PropertyToID("_MaskTexture");
            public static readonly int Attenuation = Shader.PropertyToID("_Attenuation");
        }


        public override bool CheckActive(ref RenderingData renderingData)
        {
            if (!IsEnable)
                return false;


            //必须要有主光源
            int mainLightIndex = renderingData.lightData.mainLightIndex;
            if (mainLightIndex == -1)
                return false;

            //其次如果太背离光源，也不执行
            var mainLight = renderingData.lightData.visibleLights[mainLightIndex].light;
            var cameraDir = renderingData.cameraData.camera.transform.forward;
            var mainLightDir = -mainLight.transform.forward;
            if (Vector3.Dot(cameraDir, mainLightDir) < 0.4f)
                return false;

            return true;
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            //获取主光源
            int mainLightIndex = renderingData.lightData.mainLightIndex;
            var mainLight = renderingData.lightData.visibleLights[mainLightIndex].light;
            var camera = renderingData.cameraData.camera;
            //将光源坐标转化为屏幕坐标

            Vector3 sunPos = Vector3.zero;
            if (LightShaft.virtualLight != null)
            {
                Vector3 direction = -LightShaft.virtualLight.localToWorldMatrix.GetColumn(2);
                sunPos = camera.transform.position + direction * camera.farClipPlane;
            }
            else
                sunPos = mainLight.transform.position + mainLight.transform.forward * camera.farClipPlane;

            Vector3 sunScreenPos = camera.WorldToScreenPoint(sunPos);
            //第一步 降采样提取屏幕高亮区域 这一步和Bloom一致 是不是可以两个效果合并成一个
            int RTWidth = Screen.width >> settings.DownSample.value;
            int RTHeight = Screen.height >> settings.DownSample.value;
            cmd.GetTemporaryRT(ShaderIDs.LightShaftTempId, RTWidth, RTHeight, 0, FilterMode.Bilinear);


            // blitMaterial.SetVector(ShaderIDs.BloomTintAndThreshold, new Vector4(settings.BloomTint.value.r, settings.BloomTint.value.g, settings.BloomTint.value.b, settings.Threshold.value));
            var bloomTint = new Vector3(mainLight.color.r, mainLight.color.g, mainLight.color.b);
            blitMaterial.SetVector(ShaderIDs.BloomTintAndThreshold, new Vector4(bloomTint.x, bloomTint.y, bloomTint.z, settings.Threshold.value));
            blitMaterial.SetVector(ShaderIDs.LightShaftParameters, new Vector4(settings.OcclusionDepthRange.value, settings.BloomIntensity.value, sunScreenPos.x / camera.pixelWidth, sunScreenPos.y / camera.pixelHeight));
            cmd.Blit(source, ShaderIDs.LightShaftTempId, blitMaterial, 0);
            // cmd.Blit(ShaderIDs.LightShaftTempId, target);



            //第二步 使用径向模糊
            int sampleDistance = 25;

            cmd.GetTemporaryRT(ShaderIDs.TempBlurBuffer1, RTWidth, RTHeight, 0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(ShaderIDs.TempBlurBuffer2, RTWidth, RTHeight, 0, FilterMode.Bilinear);

            //1
            blitMaterial.SetInt(ShaderIDs.SampleDistance, sampleDistance);
            cmd.Blit(ShaderIDs.LightShaftTempId, ShaderIDs.TempBlurBuffer1, blitMaterial, 1);
            //2

            blitMaterial.SetInt(ShaderIDs.SampleDistance, sampleDistance * 2);
            cmd.Blit(ShaderIDs.TempBlurBuffer1, ShaderIDs.TempBlurBuffer2, blitMaterial, 1);

            //3
            // m_BlitMaterial.SetInt(ShaderIDs.SampleDistance, sampleDistance * 4);
            // cmd.Blit(ShaderIDs.TempBlurBuffer2, ShaderIDs.TempBlurBuffer1, m_BlitMaterial, 1);

            //最终Blit回去
            cmd.SetGlobalTexture(ShaderIDs.BluredTexture, ShaderIDs.TempBlurBuffer2);
            blitMaterial.SetFloat(ShaderIDs.Attenuation, settings.Attenuation.value);
            cmd.Blit(source, target, blitMaterial, 2);


            cmd.ReleaseTemporaryRT(ShaderIDs.LightShaftTempId);
            cmd.ReleaseTemporaryRT(ShaderIDs.TempBlurBuffer1);
            cmd.ReleaseTemporaryRT(ShaderIDs.TempBlurBuffer2);
        }
    }
}


