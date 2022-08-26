using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "Tint")]
    public class ColorAdjustmentTint : VolumeSetting
    {
        public override bool IsActive() => indensity.value > 0;

        [Range(0.0f, 1.0f)]
        public FloatParameter indensity = new ClampedFloatParameter(0f, 0f, 1f);
        public ColorParameter colorTint = new ColorParameter(new Color(0.9f, 1.0f, 0.0f, 1), true, true, true);
    }

    public class ColorAdjustmentTintRenderer : VolumeRenderer<ColorAdjustmentTint>
    {
        public override string PROFILER_TAG => "ColorAdjustmentTint";
        public override string ShaderName => "Hidden/PostProcessing/ColorAdjustment/Tint";


        static class ShaderIDs
        {

            internal static readonly int indensity = Shader.PropertyToID("_Indensity");
            internal static readonly int ColorTint = Shader.PropertyToID("_ColorTint");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.indensity, settings.indensity.value);
            blitMaterial.SetVector(ShaderIDs.ColorTint, settings.colorTint.value);

            cmd.Blit(source, target, blitMaterial);
        }
    }

}