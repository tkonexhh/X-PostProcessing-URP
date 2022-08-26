using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "Tint PlayerPos")]
    public class ColorAdjustmentTintPlayerPos : VolumeSetting
    {
        public override bool IsActive() => indensity.value > 0;

        [Range(0.0f, 1.0f)]
        public ClampedFloatParameter indensity = new ClampedFloatParameter(0f, 0f, 1f);
        public ColorParameter colorTint = new ColorParameter(new Color(0.9f, 1.0f, 0.0f, 1), true, true, true);
        // public ClampedFloatParameter range = new ClampedFloatParameter(5f, 0.1f, 20f);
        public static float range = 5.0f;
        public static float OriginRange = 5.0f;
    }

    public class ColorAdjustmentTintPlayerPosRenderer : VolumeRenderer<ColorAdjustmentTintPlayerPos>
    {
        public override string PROFILER_TAG => "ColorAdjustmentTintPlayerPos";
        public override string ShaderName => "Hidden/PostProcessing/ColorAdjustment/TintPlayerPos";


        static class ShaderIDs
        {

            internal static readonly int indensity = Shader.PropertyToID("_Indensity");
            internal static readonly int ColorTint = Shader.PropertyToID("_ColorTint");
            internal static readonly int range = Shader.PropertyToID("_Range");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.indensity, settings.indensity.value);
            // blitMaterial.SetFloat(ShaderIDs.range, settings.range.value);
            blitMaterial.SetFloat(ShaderIDs.range, ColorAdjustmentTintPlayerPos.range);
            blitMaterial.SetVector(ShaderIDs.ColorTint, settings.colorTint.value);

            cmd.Blit(source, target, blitMaterial);
        }
    }

}