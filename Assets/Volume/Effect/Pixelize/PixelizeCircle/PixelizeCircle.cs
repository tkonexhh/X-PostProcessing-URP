using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Pixelate + "圆点像素化 (PixelizeCircle)")]
    public class PixelizeCircle : VolumeSetting
    {
        public override bool IsActive() => pixelSize.value > 0;

        public FloatParameter pixelSize = new ClampedFloatParameter(0f, 0.01f, 1f);
        public FloatParameter circleRadius = new ClampedFloatParameter(0.45f, 0.01f, 1f);
        [Tooltip("Pixel interval X")]
        public FloatParameter pixelIntervalX = new ClampedFloatParameter(1f, 0.2f, 5f);
        [Tooltip("Pixel interval Y")]
        public FloatParameter pixelIntervalY = new ClampedFloatParameter(1f, 0.2f, 5.0f);
        public ColorParameter BackgroundColor = new ColorParameter(Color.black, true, true, true);
    }


    public class PixelizeCircleRenderer : VolumeRenderer<PixelizeCircle>
    {
        public override string PROFILER_TAG => "PixelizeCircle";
        public override string ShaderName => "Hidden/PostProcessing/Pixelate/PixelizeCircle";


        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int Params2 = Shader.PropertyToID("_Params2");
            internal static readonly int BackgroundColor = Shader.PropertyToID("_BackgroundColor");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            float size = (1.01f - settings.pixelSize.value) * 300f;

            Vector4 parameters = new Vector4(size, ((Screen.width * 2 / Screen.height) * size / Mathf.Sqrt(3f)), settings.circleRadius.value, 0f);

            blitMaterial.SetVector(ShaderIDs.Params, parameters);
            blitMaterial.SetVector(ShaderIDs.Params2, new Vector2(settings.pixelIntervalX.value, settings.pixelIntervalY.value));
            blitMaterial.SetColor(ShaderIDs.BackgroundColor, settings.BackgroundColor.value);

            cmd.Blit(source, target, blitMaterial);
        }


    }
}

