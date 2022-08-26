using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "RGB颜色分离V4 (RGB SplitV4)")]
    public class GlitchRGBSplitV4 : VolumeSetting
    {
        public override bool IsActive() => indensity.value != 0;

        public FloatParameter indensity = new ClampedFloatParameter(0f, -1f, 1f);
        public FloatParameter speed = new ClampedFloatParameter(10f, 0f, 100f);

    }


    public sealed class GlitchRGBSplitV4Renderer : VolumeRenderer<GlitchRGBSplitV4>
    {
        public override string PROFILER_TAG => "GlitchRGBSplitV4";
        public override string ShaderName => "Hidden/PostProcessing/Glitch/RGBSplitV4";


        private float TimeX = 1.0f;


        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int Params2 = Shader.PropertyToID("_Params2");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            TimeX += Time.deltaTime;
            if (TimeX > 100)
            {
                TimeX = 0;
            }

            blitMaterial.SetVector(ShaderIDs.Params, new Vector2(settings.indensity.value * 0.1f, Mathf.Floor(TimeX * settings.speed.value)));

            cmd.Blit(source, target, blitMaterial);
        }
    }

}