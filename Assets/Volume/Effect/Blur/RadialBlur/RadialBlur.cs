using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Blur + "径向模糊 (Radial Blur)")]
    public class RadialBlur : VolumeSetting
    {
        public override bool IsActive() => BlurRadius.value > 0;
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 1f);
        public IntParameter Iteration = new ClampedIntParameter(10, 2, 30);
        public FloatParameter RadialCenterX = new ClampedFloatParameter(0.5f, 0f, 1f);
        public FloatParameter RadialCenterY = new ClampedFloatParameter(0.5f, 0f, 1f);
    }


    public class RadialBlurRenderer : VolumeRenderer<RadialBlur>
    {
        public override string PROFILER_TAG => "RadialBlur";
        public override string ShaderName => "Hidden/PostProcessing/Blur/RadialBlur";



        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetVector(ShaderIDs.Params, new Vector4(settings.BlurRadius.value * 0.02f, settings.Iteration.value, settings.RadialCenterX.value, settings.RadialCenterY.value));
            cmd.Blit(source, target, blitMaterial, 0);
        }
    }

}