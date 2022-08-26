using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "Technicolor")]
    public class ColorAdjustmentTechnicolor : VolumeSetting
    {
        public override bool IsActive() => indensity.value > 0;

        public FloatParameter indensity = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter exposure = new ClampedFloatParameter(4f, 0f, 8f);
        public FloatParameter colorBalanceR = new ClampedFloatParameter(0.2f, 0f, 1f);
        public FloatParameter colorBalanceG = new ClampedFloatParameter(0.2f, 0f, 1f);
        public FloatParameter colorBalanceB = new ClampedFloatParameter(0.2f, 0f, 1f);
    }

    public class ColorAdjustmentTechnicolorRenderer : VolumeRenderer<ColorAdjustmentTechnicolor>
    {
        public override string PROFILER_TAG => "ColorAdjustmentTechnicolor";
        public override string ShaderName => "Hidden/PostProcessing/ColorAdjustment/Technicolor";


        static class ShaderIDs
        {
            internal static readonly int exposure = Shader.PropertyToID("_Exposure");
            internal static readonly int colorBalance = Shader.PropertyToID("_ColorBalance");
            internal static readonly int indensity = Shader.PropertyToID("_Indensity");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.exposure, 8f - settings.exposure.value);
            blitMaterial.SetVector(ShaderIDs.colorBalance, Vector3.one - new Vector3(settings.colorBalanceR.value, settings.colorBalanceG.value, settings.colorBalanceB.value));
            blitMaterial.SetFloat(ShaderIDs.indensity, settings.indensity.value);

            cmd.Blit(source, target, blitMaterial);

        }
    }

}