using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ImageProcessing + "SharpenV2")]
    public class SharpenV2 : VolumeSetting
    {
        public override bool IsActive() => Sharpness.value > 0;
        public FloatParameter Sharpness = new ClampedFloatParameter(0f, 0f, 5f);
    }

    public class SharpenV2Renderer : VolumeRenderer<SharpenV2>
    {
        public override string PROFILER_TAG => "SharpenV2";
        public override string ShaderName => "Hidden/PostProcessing/ImageProcessing/SharpenV2";


        static class ShaderIDs
        {
            internal static readonly int Sharpness = Shader.PropertyToID("_Sharpness");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.Sharpness, settings.Sharpness.value);

            cmd.Blit(source, target, blitMaterial);
        }

    }

}