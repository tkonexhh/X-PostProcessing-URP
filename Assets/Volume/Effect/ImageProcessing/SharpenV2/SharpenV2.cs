using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ImageProcessing + "SharpenV2")]
    public class SharpenV2 : VolumeSetting
    {
        public override bool IsActive() => Sharpness.value > 0;
        public FloatParameter Sharpness = new ClampedFloatParameter(0f, 0f, 5f);
    }

    public class SharpenV2Renderer : VolumeRenderer<SharpenV2>
    {
        private const string PROFILER_TAG = "SharpenV2";
        private Shader shader;
        private Material m_BlitMaterial;

        private float randomFrequency;


        public override void Init()
        {
            shader = Shader.Find("Hidden/PostProcessing/ImageProcessing/SharpenV2");
            m_BlitMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        static class ShaderIDs
        {
            internal static readonly int Sharpness = Shader.PropertyToID("_Sharpness");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;


            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetFloat(ShaderIDs.Sharpness, settings.Sharpness.value);

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }

    }

}