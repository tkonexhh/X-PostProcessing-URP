using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "RGB颜色分离V5 (RGB SplitV5)")]
    public class GlitchRGBSplitV5 : VolumeSetting
    {
        public override bool IsActive() => Amplitude.value > 0;
        public FloatParameter Amplitude = new ClampedFloatParameter(0f, 0f, 5f);
        public FloatParameter Speed = new ClampedFloatParameter(0.1f, 0f, 1f);

    }


    public sealed class GlitchRGBSplitV5Renderer : VolumeRenderer<GlitchRGBSplitV5>
    {
        public override string PROFILER_TAG => "GlitchRGBSplitV5";
        public override string ShaderName => "Hidden/PostProcessing/Glitch/RGBSplitV5";

        private Texture2D m_NoiseTex;


        public override void Init()
        {
            base.Init();
            m_NoiseTex = Resources.Load("X-Noise256") as Texture2D;
        }

        static class ShaderIDs
        {
            internal static readonly int NoiseTex = Shader.PropertyToID("_NoiseTex");
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetVector(ShaderIDs.Params, new Vector2(settings.Amplitude.value, settings.Speed.value));
            if (m_NoiseTex != null)
            {
                blitMaterial.SetTexture(ShaderIDs.NoiseTex, m_NoiseTex);
            }

            cmd.Blit(source, target, blitMaterial);
        }
    }

}