using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "Saturation")]
    public class ColorAdjustmentSaturation : VolumeSetting
    {
        public override bool IsActive() => saturation.value > 0;

        public FloatParameter saturation = new ClampedFloatParameter(0, 0f, 1f);// { value = 1f };
    }


    public class ColorAdjustmentSaturationRenderer : VolumeRenderer<ColorAdjustmentSaturation>
    {
        public override string PROFILER_TAG => "ColorAdjustmentSaturation";
        public override string ShaderName => "Hidden/PostProcessing/ColorAdjustment/Saturation";

        static class ShaderIDs
        {
            internal static readonly int Saturation = Shader.PropertyToID("_Saturation");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.Saturation, settings.saturation.value);

            cmd.Blit(source, target, blitMaterial);
        }
    }

}