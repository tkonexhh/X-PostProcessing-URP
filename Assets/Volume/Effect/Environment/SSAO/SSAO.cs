using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    public enum SSAONormalQuality
    {
        Low,
        Medium,
        High
    }

    [System.Serializable]
    public sealed class SSAONormalQualityParameter : VolumeParameter<SSAONormalQuality> { public SSAONormalQualityParameter(SSAONormalQuality value, bool overrideState = false) : base(value, overrideState) { } }

    [VolumeComponentMenu(VolumeDefine.Environment + "SSAO")]
    public class SSAO : VolumeSetting
    {
        public override bool IsActive() => AOIntensity.value > 0;

        public ClampedFloatParameter AOIntensity = new ClampedFloatParameter(0.0f, 0f, 20f);
        public SSAONormalQualityParameter NormalQuality = new SSAONormalQualityParameter(SSAONormalQuality.High);
        public ClampedFloatParameter sampleRadius = new ClampedFloatParameter(0.01f, 0, 0.1f);
        public ClampedIntParameter sampleCount = new ClampedIntParameter(16, 1, 32);
        public ClampedFloatParameter blurRadius = new ClampedFloatParameter(0.1f, 0.1f, 4f);
        // public ColorParameter SSAOColor = new ColorParameter(Color.black);
        public ClampedIntParameter DownSample = new ClampedIntParameter(0, 0, 5);
        public BoolParameter debug = new BoolParameter(false);
    }


    public class SSAORenderer : VolumeRenderer<SSAO>
    {
        public override string PROFILER_TAG => "SSAO";
        public override string ShaderName => "Hidden/PostProcessing/Environment/SSAO";


        static class ShaderIDs
        {

            public static readonly int Params = Shader.PropertyToID("_Params");
            public static readonly int TempBlurBuffer1 = Shader.PropertyToID("_TempSSAOBuffer1");
            public static readonly int TempBlurBuffer2 = Shader.PropertyToID("_TempSSAOBuffer2");
            public static readonly int ScreenTex = Shader.PropertyToID("_ScreenTex");

            public const string k_NormalReconstructionLowKeyword = "_RECONSTRUCT_NORMAL_LOW";
            public const string k_NormalReconstructionMediumKeyword = "_RECONSTRUCT_NORMAL_MEDIUM";
            public const string k_NormalReconstructionHighKeyword = "_RECONSTRUCT_NORMAL_HIGH";
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            switch (settings.NormalQuality.value)
            {
                case SSAONormalQuality.Low:
                    CoreUtils.SetKeyword(blitMaterial, ShaderIDs.k_NormalReconstructionLowKeyword, true);
                    CoreUtils.SetKeyword(blitMaterial, ShaderIDs.k_NormalReconstructionMediumKeyword, false);
                    CoreUtils.SetKeyword(blitMaterial, ShaderIDs.k_NormalReconstructionHighKeyword, false);
                    break;
                case SSAONormalQuality.Medium:
                    CoreUtils.SetKeyword(blitMaterial, ShaderIDs.k_NormalReconstructionLowKeyword, false);
                    CoreUtils.SetKeyword(blitMaterial, ShaderIDs.k_NormalReconstructionMediumKeyword, true);
                    CoreUtils.SetKeyword(blitMaterial, ShaderIDs.k_NormalReconstructionHighKeyword, false);
                    break;
                case SSAONormalQuality.High:
                    CoreUtils.SetKeyword(blitMaterial, ShaderIDs.k_NormalReconstructionLowKeyword, false);
                    CoreUtils.SetKeyword(blitMaterial, ShaderIDs.k_NormalReconstructionMediumKeyword, false);
                    CoreUtils.SetKeyword(blitMaterial, ShaderIDs.k_NormalReconstructionHighKeyword, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            blitMaterial.SetVector(ShaderIDs.Params, new Vector4(settings.sampleRadius.value, settings.sampleCount.value, settings.AOIntensity.value, settings.blurRadius.value));

            int RTWidth = Screen.width >> settings.DownSample.value;
            int RTHeight = Screen.height >> settings.DownSample.value;

            cmd.GetTemporaryRT(ShaderIDs.TempBlurBuffer1, RTWidth, RTHeight, 0);
            cmd.Blit(source, ShaderIDs.TempBlurBuffer1, blitMaterial, 0);//计算AO

            cmd.GetTemporaryRT(ShaderIDs.TempBlurBuffer2, RTWidth, RTHeight, 0);
            cmd.Blit(ShaderIDs.TempBlurBuffer1, ShaderIDs.TempBlurBuffer2, blitMaterial, 1);//H模糊
            cmd.Blit(ShaderIDs.TempBlurBuffer2, ShaderIDs.TempBlurBuffer1, blitMaterial, 2);//V模糊

            if (settings.debug.value)
            {
                cmd.Blit(ShaderIDs.TempBlurBuffer1, target, blitMaterial, 4);//塞回去
            }
            else
            {
                cmd.SetGlobalTexture(ShaderIDs.ScreenTex, source);
                cmd.Blit(ShaderIDs.TempBlurBuffer1, target, blitMaterial, 3);//塞回去
            }

            cmd.ReleaseTemporaryRT(ShaderIDs.TempBlurBuffer1);
            cmd.ReleaseTemporaryRT(ShaderIDs.TempBlurBuffer2);

        }
    }
}