using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "屏幕跳跃故障 (Screen Jump Glitch)")]
    public class GlitchScreenJump : VolumeSetting
    {
        public override bool IsActive() => ScreenJumpIndensity.value > 0;

        public DirectionParameter ScreenJumpDirection = new DirectionParameter(Direction.Vertical);

        public FloatParameter ScreenJumpIndensity = new ClampedFloatParameter(0f, 0f, 1f);
    }

    public class GlitchScreenJumpRenderer : VolumeRenderer<GlitchScreenJump>
    {
        public override string PROFILER_TAG => "GlitchScreenJump";
        public override string ShaderName => "Hidden/PostProcessing/Glitch/ScreenJump";


        float ScreenJumpTime;



        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            ScreenJumpTime += Time.deltaTime * settings.ScreenJumpIndensity.value * 9.8f;
            Vector2 ScreenJumpVector = new Vector2(settings.ScreenJumpIndensity.value, ScreenJumpTime);

            blitMaterial.SetVector(ShaderIDs.Params, ScreenJumpVector);

            cmd.Blit(source, target, blitMaterial, (int)settings.ScreenJumpDirection.value);
        }

    }

}