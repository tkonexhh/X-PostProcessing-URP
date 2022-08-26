using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "ContrastV3")]
    public class ColorAdjustmentContrastV3 : VolumeSetting
    {
        public override bool IsActive() => contrast.value != Vector4.zero;
        public Vector4Parameter contrast = new Vector4Parameter(Vector4.zero);
    }

    public class ColorAdjustmentContrastV3Renderer : VolumeRenderer<ColorAdjustmentContrastV3>
    {
        public override string PROFILER_TAG => "ColorAdjustmentContrastV3";
        public override string ShaderName => "Hidden/PostProcessing/ColorAdjustment/ContrastV3";



        static class ShaderIDs
        {
            internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetVector(ShaderIDs.Contrast, settings.contrast.value);

            cmd.Blit(source, target, blitMaterial);
        }
    }

}