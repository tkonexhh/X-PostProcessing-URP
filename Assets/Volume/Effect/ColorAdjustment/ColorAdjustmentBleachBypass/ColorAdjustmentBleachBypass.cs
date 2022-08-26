using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "BleachBypass")]
    public class ColorAdjustmentBleachBypass : VolumeSetting
    {
        public override bool IsActive() => Indensity.value > 0;
        public FloatParameter Indensity = new ClampedFloatParameter(0, 0f, 1f);
    }

    public class ColorAdjustmentBleachBypassRenderer : VolumeRenderer<ColorAdjustmentBleachBypass>
    {
        public override string PROFILER_TAG => "ColorAdjustmentBleachBypass";
        public override string ShaderName => "Hidden/PostProcessing/ColorAdjustment/BleachBypass";


        static class ShaderIDs
        {
            internal static readonly int Indensity = Shader.PropertyToID("_Indensity");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.Indensity, settings.Indensity.value);

            cmd.Blit(source, target, blitMaterial);
        }
    }

}