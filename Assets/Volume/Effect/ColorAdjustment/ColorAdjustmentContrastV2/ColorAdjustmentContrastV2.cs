using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "ContrastV2")]
    public class ColorAdjustmentContrastV2 : VolumeSetting
    {
        public override bool IsActive() => contrast.value != 0;
        public FloatParameter contrast = new ClampedFloatParameter(0, -1f, 5f);
        public FloatParameter ContrastFactorR = new ClampedFloatParameter(0f, -1f, 1f);
        public FloatParameter ContrastFactorG = new ClampedFloatParameter(0f, -1f, 1f);
        public FloatParameter ContrastFactorB = new ClampedFloatParameter(0f, -1f, 1f);
    }

    public class ColorAdjustmentContrastV2Renderer : VolumeRenderer<ColorAdjustmentContrastV2>
    {
        public override string PROFILER_TAG => "ColorAdjustmentContrastV2";
        public override string ShaderName => "Hidden/PostProcessing/ColorAdjustment/ContrastV2";


        static class ShaderIDs
        {
            internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
            internal static readonly int ContrastFactorRGB = Shader.PropertyToID("_ContrastFactorRGB");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.Contrast, settings.contrast.value + 1f);
            blitMaterial.SetVector(ShaderIDs.ContrastFactorRGB, new Vector3(settings.ContrastFactorR.value, settings.ContrastFactorG.value, settings.ContrastFactorB.value));

            cmd.Blit(source, target, blitMaterial);
        }
    }

}