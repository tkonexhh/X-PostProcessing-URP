using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace XPostProcessing
{

    public enum BloomQuailtyType
    {
        Low,
        High,
    }

    [System.Serializable]
    public sealed class BloomQuailtyParameter : VolumeParameter<BloomQuailtyType> { public BloomQuailtyParameter(BloomQuailtyType value, bool overrideState = false) : base(value, overrideState) { } }

    [VolumeComponentMenu(VolumeDefine.Environment + "泛光 (Bloom)")]
    public class Bloom : VolumeSetting
    {
        public override bool IsActive() => Intensity.value > 0;
        public BloomQuailtyParameter BloomQuailty = new BloomQuailtyParameter(BloomQuailtyType.Low);
        public MinFloatParameter Intensity = new MinFloatParameter(0f, 0f);
        public ClampedFloatParameter Threshold = new ClampedFloatParameter(0.2f, 0f, 4f);
        public ClampedIntParameter SkipIterations = new ClampedIntParameter(0, 0, 9);
        public ClampedFloatParameter SampleScale = new ClampedFloatParameter(1f, 0f, 5f);
    }

    public class BloomRenderer : VolumeRenderer<Bloom>
    {
        public override string PROFILER_TAG => "Bloom";
        public override string ShaderName => "Hidden/PostProcessing/Environment/Bloom";

        public static bool IsEnable = true;

        const int MaxIterations = 10;
        public static int[] m_BloomMipUp;
        public static int[] m_BloomMipDown;

        static class ShaderIDs
        {
            public static readonly int PrefilterID = Shader.PropertyToID("_Prefilter");
            public static readonly int BloomTexID = Shader.PropertyToID("_BloomTex");
            public static readonly int SourceTexID = Shader.PropertyToID("_SourceTex");
            public static readonly int IntensityID = Shader.PropertyToID("_Intensity");
            public static readonly int SampleScaleID = Shader.PropertyToID("_SamplerScale");
            public static readonly int ParamsID = Shader.PropertyToID("_Params");
        }

        public override bool CheckActive(ref RenderingData renderingData) => IsEnable;


        public override void Init()
        {
            base.Init();

            m_BloomMipUp = new int[MaxIterations];
            m_BloomMipDown = new int[MaxIterations];

            for (int i = 0; i < MaxIterations; i++)
            {
                m_BloomMipUp[i] = Shader.PropertyToID("_BloomMipUp" + i);
                m_BloomMipDown[i] = Shader.PropertyToID("_BloomMipDown" + i);
            }
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            //Bloom的原理就是降采样 降采样 在升采样 升采样的过程
            var sourceTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            int RTWidth = 0;
            int RTHeight = 0;

            if (settings.BloomQuailty.value == BloomQuailtyType.High)
            {
                RTWidth = sourceTargetDescriptor.width >> 1;
                RTHeight = sourceTargetDescriptor.height >> 1;
            }
            else
            {
                RTWidth = sourceTargetDescriptor.width / 3;
                RTHeight = sourceTargetDescriptor.height / 3;
            }

            // blur iteration times
            int maxSize = Mathf.Max(RTWidth, RTHeight);
            int iterations = Mathf.FloorToInt(Mathf.Log(maxSize, 2f) - 1);
            iterations -= settings.SkipIterations.value;


            if (settings.BloomQuailty.value == BloomQuailtyType.Low)
                iterations -= 2;

            // Debug.LogError("iterations:" + iterations);
            int mipCount = Mathf.Clamp(iterations, 1, MaxIterations);

            cmd.GetTemporaryRT(ShaderIDs.PrefilterID, RTWidth, RTHeight, 0, FilterMode.Bilinear);


            //提取高亮范围
            float threshold = Mathf.GammaToLinearSpace(settings.Threshold.value);
            float thresholdKnee = threshold * 0.5f; // Hardcoded soft knee
            var param = new Vector4(threshold, thresholdKnee, 0, 0);
            blitMaterial.SetVector(ShaderIDs.ParamsID, param);
            blitMaterial.SetFloat(ShaderIDs.SampleScaleID, settings.SampleScale.value);
            cmd.Blit(source, ShaderIDs.PrefilterID, blitMaterial, 0);



            //降采样过程
            int lastDown = ShaderIDs.PrefilterID;
            for (int i = 0; i < mipCount; i++)
            {
                RTWidth = Mathf.Max(1, RTWidth >> 1);
                RTHeight = Mathf.Max(1, RTHeight >> 1);
                int mipDown = m_BloomMipDown[i];
                cmd.GetTemporaryRT(mipDown, RTWidth, RTHeight, 0, FilterMode.Bilinear);

                if (settings.BloomQuailty.value == BloomQuailtyType.Low)
                    cmd.Blit(lastDown, mipDown, blitMaterial, 1);
                else
                {
                    cmd.Blit(lastDown, mipDown, blitMaterial, 4);
                    cmd.Blit(mipDown, lastDown, blitMaterial, 5);
                }

                lastDown = mipDown;
            }

            //升采样
            for (int i = mipCount - 1; i >= 0; i--)
            {
                int highMip = m_BloomMipDown[i];
                int dst = m_BloomMipUp[i];
                RTWidth = Mathf.Max(1, RTWidth << 1);
                RTHeight = Mathf.Max(1, RTHeight << 1);
                cmd.GetTemporaryRT(dst, RTWidth, RTHeight, 0, FilterMode.Bilinear);
                cmd.Blit(highMip, dst, blitMaterial, 1);
                lastDown = dst;
            }

            //Final
            cmd.GetTemporaryRT(ShaderIDs.BloomTexID, RTWidth << 1, RTHeight << 1, 0, FilterMode.Bilinear);
            blitMaterial.SetFloat(ShaderIDs.IntensityID, settings.Intensity.value);
            cmd.Blit(lastDown, ShaderIDs.BloomTexID, blitMaterial, 2);

            //Combine
            cmd.SetGlobalTexture(ShaderIDs.SourceTexID, source);
            cmd.Blit(ShaderIDs.BloomTexID, target, blitMaterial, 3);


            for (int i = 0; i < mipCount; i++)
            {
                cmd.ReleaseTemporaryRT(m_BloomMipUp[i]);
                cmd.ReleaseTemporaryRT(m_BloomMipDown[i]);
            }
            cmd.ReleaseTemporaryRT(ShaderIDs.BloomTexID);
        }

    }

}