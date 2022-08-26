using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "白平衡 (WhiteBalance)")]
    public class ColorAdjustmentWhiteBalance : VolumeSetting
    {
        public override bool IsActive() => temperature.value != 0;

        public FloatParameter temperature = new ClampedFloatParameter(0f, -1f, 1f);
        public FloatParameter tint = new ClampedFloatParameter(0f, -1f, 1f);
    }

    public class ColorAdjustmentWhiteBalanceRenderer : VolumeRenderer<ColorAdjustmentWhiteBalance>
    {
        public override string PROFILER_TAG => "ColorAdjustmentWhiteBalance";
        public override string ShaderName => "Hidden/PostProcessing/ColorAdjustment/WhiteBalance";


        static class ShaderIDs
        {

            internal static readonly int Temperature = Shader.PropertyToID("_Temperature");
            internal static readonly int Tint = Shader.PropertyToID("_Tint");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.Temperature, settings.temperature.value);
            blitMaterial.SetFloat(ShaderIDs.Tint, settings.tint.value);

            cmd.Blit(source, target, blitMaterial);
        }
    }

}