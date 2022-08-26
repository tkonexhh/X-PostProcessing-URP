using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Pixelate + "菱形像素化 (PixelizeDiamond)")]
    public class PixelizeDiamond : VolumeSetting
    {
        public override bool IsActive() => pixelSize.value > 0;
        public FloatParameter pixelSize = new ClampedFloatParameter(0f, 0.01f, 1.0f);
    }

    public class PixelizeDiamondRenderer : VolumeRenderer<PixelizeDiamond>
    {
        public override string PROFILER_TAG => "PixelizeDiamond";
        public override string ShaderName => "Hidden/PostProcessing/Pixelate/PixelizeDiamond";


        static class ShaderIDs
        {
            internal static readonly int PixelSize = Shader.PropertyToID("_PixelSize");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.PixelSize, settings.pixelSize.value);

            cmd.Blit(source, target, blitMaterial);
        }


    }
}

