using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "Contrast")]
    public class ColorAdjustmentContrast : VolumeSetting
    {
        public override bool IsActive() => contrast.value != 0;
        public FloatParameter contrast = new ClampedFloatParameter(0, -1f, 2f);
    }

    public class ColorAdjustmentContrastRenderer : VolumeRenderer<ColorAdjustmentContrast>
    {
        public override string PROFILER_TAG => "ColorAdjustmentContrast";
        public override string ShaderName => "Hidden/PostProcessing/ColorAdjustment/Contrast";


        static class ShaderIDs
        {
            internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.Contrast, settings.contrast.value + 1f);

            cmd.Blit(source, target, blitMaterial);
        }
    }

}