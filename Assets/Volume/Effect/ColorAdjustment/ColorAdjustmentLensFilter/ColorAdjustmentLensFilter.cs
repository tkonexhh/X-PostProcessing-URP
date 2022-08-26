using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "镜头滤光 (LensFilter)")]
    public class ColorAdjustmentLensFilter : VolumeSetting
    {
        public override bool IsActive() => Indensity.value > 0;

        public FloatParameter Indensity = new ClampedFloatParameter(0f, 0f, 1f);
        public ColorParameter LensColor = new ColorParameter(new Color(1.0f, 1.0f, 0.1f, 1), true, true, true);
    }


    public class ColorAdjustmentLensFilterRenderer : VolumeRenderer<ColorAdjustmentLensFilter>
    {
        public override string PROFILER_TAG => "ColorAdjustmentLensFilter";
        public override string ShaderName => "Hidden/PostProcessing/ColorAdjustment/LensFilter";


        static class ShaderIDs
        {
            internal static readonly int LensColor = Shader.PropertyToID("_LensColor");
            internal static readonly int Indensity = Shader.PropertyToID("_Indensity");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.Indensity, settings.Indensity.value);
            blitMaterial.SetColor(ShaderIDs.LensColor, settings.LensColor.value);

            cmd.Blit(source, target, blitMaterial);
        }
    }

}