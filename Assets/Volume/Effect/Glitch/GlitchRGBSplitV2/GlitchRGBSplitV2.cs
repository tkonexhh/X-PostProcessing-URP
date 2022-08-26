using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "RGB颜色分离V2 (RGB SplitV2)")]
    public class GlitchRGBSplitV2 : VolumeSetting
    {
        public override bool IsActive() => Amount.value > 0;

        public FloatParameter Amount = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter Amplitude = new ClampedFloatParameter(3f, 1f, 6f);
        public FloatParameter Speed = new ClampedFloatParameter(1f, 0f, 2f);

    }


    public sealed class GlitchRGBSplitV2Renderer : VolumeRenderer<GlitchRGBSplitV2>
    {
        public override string PROFILER_TAG => "GlitchRGBSplitV2";
        public override string ShaderName => "Hidden/PostProcessing/Glitch/RGBSplitV2";


        private float TimeX = 1.0f;



        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            TimeX += Time.deltaTime;
            if (TimeX > 100)
            {
                TimeX = 0;
            }

            blitMaterial.SetVector(ShaderIDs.Params, new Vector3(TimeX * settings.Speed.value, settings.Amount.value, settings.Amplitude.value));
            cmd.Blit(source, target, blitMaterial);
        }
    }

}