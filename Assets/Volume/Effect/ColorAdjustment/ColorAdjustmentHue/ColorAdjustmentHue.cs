using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "色相偏移 (Hue)")]
    public class ColorAdjustmentHue : VolumeSetting
    {
        public override bool IsActive() => HueDegree.value != 0;
        public FloatParameter HueDegree = new ClampedFloatParameter(0f, -180f, 180f);
    }


    public class ColorAdjustmentHueRenderer : VolumeRenderer<ColorAdjustmentHue>
    {
        private const string PROFILER_TAG = "ColorAdjustmentHue";
        private Shader shader;
        private Material m_BlitMaterial;

        public override void Init()
        {
            shader = Shader.Find("Hidden/PostProcessing/ColorAdjustment/Hue");
            m_BlitMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        static class ShaderIDs
        {
            internal static readonly int HueDegree = Shader.PropertyToID("_HueDegree");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetFloat(ShaderIDs.HueDegree, settings.HueDegree.value);

            cmd.Blit(source, target, m_BlitMaterial);
            cmd.EndSample(PROFILER_TAG);
        }
    }

}