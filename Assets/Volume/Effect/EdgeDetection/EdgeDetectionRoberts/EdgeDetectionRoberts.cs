using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.EdgeDetection + "Roberts")]
    public class EdgeDetectionRoberts : VolumeSetting
    {
        public override bool IsActive() => edgeWidth.value > 0;

        public ClampedFloatParameter edgeWidth = new ClampedFloatParameter(0f, 0f, 5f);
        public ColorParameter edgeColor = new ColorParameter(new Color(0.0f, 0.0f, 0.0f, 1), true, true, true);
        public ClampedFloatParameter backgroundFade = new ClampedFloatParameter(0f, 0f, 1f);
        public ColorParameter backgroundColor = new ColorParameter(new Color(1.0f, 1.0f, 1.0f, 1), true, true, true);
    }


    public sealed class EdgeDetectionRobertsRenderer : VolumeRenderer<EdgeDetectionRoberts>
    {
        public override string PROFILER_TAG => "EdgeDetectionRoberts";
        public override string ShaderName => "Hidden/PostProcessing/EdgeDetection/Roberts";


        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int EdgeColor = Shader.PropertyToID("_EdgeColor");
            internal static readonly int BackgroundColor = Shader.PropertyToID("_BackgroundColor");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetVector(ShaderIDs.Params, new Vector2(settings.edgeWidth.value, settings.backgroundFade.value));
            blitMaterial.SetColor(ShaderIDs.EdgeColor, settings.edgeColor.value);
            blitMaterial.SetColor(ShaderIDs.BackgroundColor, settings.backgroundColor.value);

            cmd.Blit(source, target, blitMaterial);
        }
    }

}