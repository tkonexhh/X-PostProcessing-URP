using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "波动抖动故障 (Wave Jitter Glitch)")]
    public class GlitchWaveJitter : VolumeSetting
    {
        public override bool IsActive() => frequency.value > 0;

        public DirectionParameter jitterDirection = new DirectionParameter(Direction.Horizontal);

        public IntervalTypeParameter intervalType = new IntervalTypeParameter(IntervalType.Random);
        public FloatParameter frequency = new ClampedFloatParameter(0f, 0f, 50f);
        public FloatParameter RGBSplit = new ClampedFloatParameter(20f, 0f, 50f);
        public FloatParameter speed = new ClampedFloatParameter(0.25f, 0f, 1f);
        public FloatParameter amount = new ClampedFloatParameter(1f, 0f, 2f);

        public BoolParameter customResolution = new BoolParameter(false);

        public Vector2Parameter resolution = new Vector2Parameter(new Vector2(640f, 480f));
    }

    public sealed class GlitchWaveJitterRenderer : VolumeRenderer<GlitchWaveJitter>
    {
        public override string PROFILER_TAG => "GlitchWaveJitter";
        public override string ShaderName => "Hidden/PostProcessing/Glitch/WaveJitter";


        private float randomFrequency;


        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int Resolution = Shader.PropertyToID("_Resolution");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {


            UpdateFrequency(settings);

            blitMaterial.SetVector(ShaderIDs.Params, new Vector4(settings.intervalType.value == IntervalType.Random ? randomFrequency : settings.frequency.value
                   , settings.RGBSplit.value, settings.speed.value, settings.amount.value));
            blitMaterial.SetVector(ShaderIDs.Resolution, settings.customResolution.value ? settings.resolution.value : new Vector2(Screen.width, Screen.height));

            cmd.Blit(source, target, blitMaterial, (int)settings.jitterDirection.value);
        }

        void UpdateFrequency(GlitchWaveJitter settings)
        {
            if (settings.intervalType.value == IntervalType.Random)
            {
                randomFrequency = UnityEngine.Random.Range(0, settings.frequency.value);
            }

            if (settings.intervalType.value == IntervalType.Infinite)
            {
                blitMaterial.EnableKeyword("USING_FREQUENCY_INFINITE");
            }
            else
            {
                blitMaterial.DisableKeyword("USING_FREQUENCY_INFINITE");
            }
        }
    }

}