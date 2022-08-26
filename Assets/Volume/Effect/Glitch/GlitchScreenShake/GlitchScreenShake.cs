using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "屏幕抖动故障 (Screen Shake Glitch)")]
    public class GlitchScreenShake : VolumeSetting
    {
        public override bool IsActive() => ScreenShakeIndensity.value > 0;
        public DirectionParameter ScreenShakeDirection = new DirectionParameter(Direction.Horizontal);
        public FloatParameter ScreenShakeIndensity = new ClampedFloatParameter(0f, 0f, 1f);
    }


    public class GlitchScreenShakeRenderer : VolumeRenderer<GlitchScreenShake>
    {
        public override string PROFILER_TAG => "GlitchScreenShake";
        public override string ShaderName => "Hidden/PostProcessing/Glitch/ScreenShake";

        static class ShaderIDs
        {
            internal static readonly int ScreenShakeIndensity = Shader.PropertyToID("_ScreenShake");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.ScreenShakeIndensity, settings.ScreenShakeIndensity.value * 0.25f);
            cmd.Blit(source, target, blitMaterial, (int)settings.ScreenShakeDirection.value);
        }

    }

}