using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "扫描线抖动故障 (Scane Line Jitter Glitch)")]
    public class GlitchScanLineJitter : VolumeSetting
    {
        public override bool IsActive() => frequency.value > 0;

        public DirectionParameter JitterDirection = new DirectionParameter(Direction.Horizontal);

        public IntervalTypeParameter intervalType = new IntervalTypeParameter(IntervalType.Random);
        public FloatParameter frequency = new ClampedFloatParameter(0f, 0f, 25f);
        public FloatParameter JitterIndensity = new ClampedFloatParameter(0.1f, 0f, 1f);
    }

    public class GlitchScanLineJitterRenderer : VolumeRenderer<GlitchScanLineJitter>
    {
        public override string PROFILER_TAG => "GlitchScanLineJitter";
        public override string ShaderName => "Hidden/PostProcessing/Glitch/ScanLineJitter";


        private float randomFrequency;


        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int JitterIndensity = Shader.PropertyToID("_ScanLineJitter");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            UpdateFrequency(settings);

            float displacement = 0.005f + Mathf.Pow(settings.JitterIndensity.value, 3) * 0.1f;
            float threshold = Mathf.Clamp01(1.0f - settings.JitterIndensity.value * 1.2f);

            //sheet.properties.SetVector(ShaderIDs.Params, new Vector3(settings.amount, settings.speed, );

            blitMaterial.SetVector(ShaderIDs.Params, new Vector3(displacement, threshold, settings.intervalType.value == IntervalType.Random ? randomFrequency : settings.frequency.value));

            cmd.Blit(source, target, blitMaterial, (int)settings.JitterDirection.value);
        }

        void UpdateFrequency(GlitchScanLineJitter settings)
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