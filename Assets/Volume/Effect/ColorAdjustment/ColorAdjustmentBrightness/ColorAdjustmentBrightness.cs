using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "Brightness")]
    public class ColorAdjustmentBrightness : VolumeSetting
    {
        public override bool IsActive() => Indensity.value != 0;
        public FloatParameter Indensity = new ClampedFloatParameter(0, -0.9f, 1f);
    }


    public class ColorAdjustmentBrightnessRenderer : VolumeRenderer<ColorAdjustmentBrightness>
    {
        public override string PROFILER_TAG => "ColorAdjustmentBrightness";
        public override string ShaderName => "Hidden/PostProcessing/ColorAdjustment/Brightness";


        static class ShaderIDs
        {
            internal static readonly int Indensity = Shader.PropertyToID("_Brightness");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.Indensity, settings.Indensity.value + 1f);

            cmd.Blit(source, target, blitMaterial);
        }
    }

}