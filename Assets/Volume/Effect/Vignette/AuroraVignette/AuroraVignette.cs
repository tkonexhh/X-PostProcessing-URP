using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Vignette + "极光渐晕 (Aurora Vignette)")]
    public class AuroraVignette : VolumeSetting
    {
        public override bool IsActive() => vignetteArea.value > 0;

        public FloatParameter vignetteArea = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter vignetteSmothness = new ClampedFloatParameter(0.5f, 0f, 1f);
        public FloatParameter vignetteFading = new ClampedFloatParameter(1f, 0f, 1f);
        public FloatParameter colorChange = new ClampedFloatParameter(0.1f, 0.1f, 1f);
        public FloatParameter colorFactorR = new ClampedFloatParameter(1f, 0f, 2f);
        public FloatParameter colorFactorG = new ClampedFloatParameter(1f, 0f, 2f);
        public FloatParameter colorFactorB = new ClampedFloatParameter(1f, 0f, 2f);
        public FloatParameter flowSpeed = new ClampedFloatParameter(1f, -2f, 2f);
    }

    public class AuroraVignetteRenderer : VolumeRenderer<AuroraVignette>
    {
        public override string PROFILER_TAG => "AuroraVignette";
        public override string ShaderName => "Hidden/PostProcessing/Vignette/AuroraVignette";


        private float TimeX = 1.0f;



        static class ShaderIDs
        {
            internal static readonly int vignetteArea = Shader.PropertyToID("_VignetteArea");
            internal static readonly int vignetteSmothness = Shader.PropertyToID("_VignetteSmothness");
            internal static readonly int colorChange = Shader.PropertyToID("_ColorChange");
            internal static readonly int colorFactor = Shader.PropertyToID("_ColorFactor");
            internal static readonly int TimeX = Shader.PropertyToID("_TimeX");
            internal static readonly int vignetteFading = Shader.PropertyToID("_Fading");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            TimeX += Time.deltaTime;
            if (TimeX > 100)
            {
                TimeX = 0;
            }

            blitMaterial.SetFloat(ShaderIDs.vignetteArea, settings.vignetteArea.value);
            blitMaterial.SetFloat(ShaderIDs.vignetteSmothness, settings.vignetteSmothness.value);
            blitMaterial.SetFloat(ShaderIDs.colorChange, settings.colorChange.value * 10f);
            blitMaterial.SetVector(ShaderIDs.colorFactor, new Vector3(settings.colorFactorR.value, settings.colorFactorG.value, settings.colorFactorB.value));
            blitMaterial.SetFloat(ShaderIDs.TimeX, TimeX * settings.flowSpeed.value);
            blitMaterial.SetFloat(ShaderIDs.vignetteFading, settings.vignetteFading.value);

            cmd.Blit(source, target, blitMaterial);
        }
    }

}