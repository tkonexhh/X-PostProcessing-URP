using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.VOLUMEROOT + "抗锯齿 FXAA")]
    public class PostFXAA : VolumeSetting
    {
        public override bool IsActive() => enable.value;


        public BoolParameter enable = new BoolParameter(true);

        public ClampedFloatParameter fixedThreshold = new ClampedFloatParameter(0.0312f, 0.0312f, 0.0833f);
        public ClampedFloatParameter relativeThreshold = new ClampedFloatParameter(0.063f, 0.063f, 0.333f);
        public ClampedFloatParameter subpixelBlending = new ClampedFloatParameter(0, 0, 2);
        public ClampedIntParameter fxaaLevel = new ClampedIntParameter(0, 0, 2);

    }


    public class PostFXAARenderer : VolumeRenderer<PostFXAA>
    {
        public override string PROFILER_TAG => "PostFXAA";
        public override string ShaderName => "Hidden/PostProcessing/FXAA";



        static class ShaderIDs
        {
            public static readonly int fxaaConfigID = Shader.PropertyToID("_FXAAConfig");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            Vector4 fxaaConfig = Vector4.zero;
            fxaaConfig.x = settings.fixedThreshold.value;
            fxaaConfig.y = settings.relativeThreshold.value;
            fxaaConfig.z = settings.subpixelBlending.value;

            if (settings.fxaaLevel.value == 0)
            {
                cmd.EnableShaderKeyword("FXAA_QUALITY_LOW");
                cmd.DisableShaderKeyword("FXAA_QUALITY_MEDIUM");
            }
            else if (settings.fxaaLevel.value == 1)
            {
                cmd.EnableShaderKeyword("FXAA_QUALITY_MEDIUM");
                cmd.DisableShaderKeyword("FXAA_QUALITY_LOW");
            }
            else
            {
                cmd.DisableShaderKeyword("FXAA_QUALITY_LOW");
                cmd.DisableShaderKeyword("FXAA_QUALITY_MEDIUM");
            }


            blitMaterial.SetVector(ShaderIDs.fxaaConfigID, fxaaConfig);

            cmd.Blit(source, target, blitMaterial);
        }
    }

}