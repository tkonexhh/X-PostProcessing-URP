using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Blur + "Kawase模糊 (Kawase Blur)")]
    public class KawaseBlur : VolumeSetting
    {
        public override bool IsActive() => BlurRadius.value > 0;
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 5f);
        public IntParameter Iteration = new ClampedIntParameter(6, 1, 15);
        public FloatParameter RTDownScaling = new ClampedFloatParameter(1f, 1f, 8f);
    }

    public class KawaseBlurRenderer : VolumeRenderer<KawaseBlur>
    {
        public override string PROFILER_TAG => "KawaseBlur";
        public override string ShaderName => "Hidden/PostProcessing/Blur/KawaseBlur";


        static class ShaderIDs
        {
            internal static readonly int BlurRadius = Shader.PropertyToID("_BlurOffset");
            internal static readonly int BufferRT1 = Shader.PropertyToID("_BufferRT1");
            internal static readonly int BufferRT2 = Shader.PropertyToID("_BufferRT2");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {


            int RTWidth = (int)(Screen.width / settings.RTDownScaling.value);
            int RTHeight = (int)(Screen.height / settings.RTDownScaling.value);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT1, RTWidth, RTHeight, 0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT2, RTWidth, RTHeight, 0, FilterMode.Bilinear);

            // downsample screen copy into smaller RT
            cmd.Blit(source, ShaderIDs.BufferRT1);


            bool needSwitch = true;
            for (int i = 0; i < settings.Iteration.value; i++)
            {
                blitMaterial.SetFloat(ShaderIDs.BlurRadius, i / settings.RTDownScaling.value + settings.BlurRadius.value);
                cmd.Blit(needSwitch ? ShaderIDs.BufferRT1 : ShaderIDs.BufferRT2, needSwitch ? ShaderIDs.BufferRT2 : ShaderIDs.BufferRT1, blitMaterial);
                needSwitch = !needSwitch;
            }

            blitMaterial.SetFloat(ShaderIDs.BlurRadius, settings.Iteration.value / settings.RTDownScaling.value + settings.BlurRadius.value);
            cmd.Blit(needSwitch ? ShaderIDs.BufferRT1 : ShaderIDs.BufferRT2, target, blitMaterial);

            // release
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT1);
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT2);
        }
    }

}