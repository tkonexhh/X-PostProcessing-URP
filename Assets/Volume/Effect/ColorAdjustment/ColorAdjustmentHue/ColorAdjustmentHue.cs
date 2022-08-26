using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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
        public override string PROFILER_TAG => "ColorAdjustmentHue";
        public override string ShaderName => "Hidden/PostProcessing/ColorAdjustment/Hue";


        static class ShaderIDs
        {
            internal static readonly int HueDegree = Shader.PropertyToID("_HueDegree");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.HueDegree, settings.HueDegree.value);

            cmd.Blit(source, target, blitMaterial);
        }
    }

}