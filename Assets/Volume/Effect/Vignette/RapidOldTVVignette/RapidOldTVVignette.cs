using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Vignette + "老式TV渐晕 (RapidOldTVVignette)")]
    public class RapidOldTVVignette : VolumeSetting
    {
        public override bool IsActive() => vignetteIndensity.value > 0;

        public VignetteTypeParameter vignetteType = new VignetteTypeParameter(VignetteType.ClassicMode);
        public FloatParameter vignetteIndensity = new ClampedFloatParameter(0f, 0f, 5f);

        public Vector2Parameter vignetteCenter = new Vector2Parameter(new Vector2(0.5f, 0.5f));
        public ColorParameter vignetteColor = new ColorParameter(new Color(0.1f, 0.8f, 1.0f), true, true, true);
    }

    public class RapidOldTVVignetteRenderer : VolumeRenderer<RapidOldTVVignette>
    {
        public override string PROFILER_TAG => "RapidOldTVVignette";
        public override string ShaderName => "Hidden/PostProcessing/Vignette/RapidOldTVVignette";


        static class ShaderIDs
        {
            internal static readonly int VignetteIndensity = Shader.PropertyToID("_VignetteIndensity");
            internal static readonly int VignetteCenter = Shader.PropertyToID("_VignetteCenter");
            internal static readonly int VignetteColor = Shader.PropertyToID("_VignetteColor");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.VignetteIndensity, settings.vignetteIndensity.value);
            blitMaterial.SetVector(ShaderIDs.VignetteCenter, settings.vignetteCenter.value);

            if (settings.vignetteType.value == VignetteType.ColorMode)
            {
                blitMaterial.SetVector(ShaderIDs.VignetteColor, settings.vignetteColor.value);
            }

            cmd.Blit(source, target, blitMaterial, (int)settings.vignetteType.value);
        }
    }

}