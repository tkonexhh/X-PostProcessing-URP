using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "错位图块故障V2 (Image Block GlitchV2)")]
    public class GlitchImageBlockV2 : VolumeSetting
    {
        public override bool IsActive() => Fade.value > 0;

        public FloatParameter Fade = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter Speed = new ClampedFloatParameter(0.5f, 0f, 1f);
        public FloatParameter Amount = new ClampedFloatParameter(1f, 0f, 10f);
        public FloatParameter BlockLayer1_U = new ClampedFloatParameter(2f, 0f, 50f);
        public FloatParameter BlockLayer1_V = new ClampedFloatParameter(16f, 0f, 50f);
        public FloatParameter BlockLayer1_Indensity = new ClampedFloatParameter(8f, 0f, 50f);
        public FloatParameter RGBSplitIndensity = new ClampedFloatParameter(2f, 0f, 50f);

        public BoolParameter BlockVisualizeDebug = new BoolParameter(false);
    }

    public class GlitchImageBlockV2Renderer : VolumeRenderer<GlitchImageBlockV2>
    {
        public override string PROFILER_TAG => "GlitchImageBlockV2";
        public override string ShaderName => "Hidden/PostProcessing/Glitch/ImageBlockV2";


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

            blitMaterial.SetVector(ShaderIDs.Params, new Vector3(TimeX * settings.Speed.value, settings.Amount.value, settings.Fade.value));
            blitMaterial.SetVector(ShaderIDs.Params2, new Vector4(settings.BlockLayer1_U.value, settings.BlockLayer1_V.value, settings.BlockLayer1_Indensity.value, settings.RGBSplitIndensity.value));


            if (settings.BlockVisualizeDebug.value)
            {
                //debug
                cmd.Blit(source, target, blitMaterial, 1);
            }
            else
            {
                cmd.Blit(source, target, blitMaterial, 0);
            }

        }
    }

}