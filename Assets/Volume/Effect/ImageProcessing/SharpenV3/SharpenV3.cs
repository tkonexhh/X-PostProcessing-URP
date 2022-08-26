using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ImageProcessing + "SharpenV3")]
    public class SharpenV3 : VolumeSetting
    {
        public override bool IsActive() => Sharpness.value > 0;
        public FloatParameter Sharpness = new ClampedFloatParameter(0f, 0f, 5f);
    }

    public class SharpenV3Renderer : VolumeRenderer<SharpenV3>
    {
        public override string PROFILER_TAG => "SharpenV3";
        public override string ShaderName => "Hidden/PostProcessing/ImageProcessing/SharpenV3";


        static class ShaderIDs
        {
            internal static readonly int CentralFactor = Shader.PropertyToID("_CentralFactor");
            internal static readonly int SideFactor = Shader.PropertyToID("_SideFactor");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.CentralFactor, 1.0f + (3.2f * settings.Sharpness.value));
            blitMaterial.SetFloat(ShaderIDs.SideFactor, 0.8f * settings.Sharpness.value);

            cmd.Blit(source, target, blitMaterial);
        }

    }

}