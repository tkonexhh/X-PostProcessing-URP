using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{

    [VolumeComponentMenu(VolumeDefine.Glitch + "错位线条故障 (Line Block Glitch)")]
    public class GlitchLineBlock : VolumeSetting
    {
        public override bool IsActive() => frequency.value > 0;

        public DirectionParameter blockDirection = new DirectionParameter(Direction.Horizontal);

        public IntervalTypeParameter intervalType = new IntervalTypeParameter(IntervalType.Random);

        public FloatParameter frequency = new ClampedFloatParameter(0f, 0f, 25f);
        public FloatParameter Amount = new ClampedFloatParameter(0.5f, 0f, 1f);

        public FloatParameter LinesWidth = new ClampedFloatParameter(1f, 0.1f, 10f);

        public FloatParameter Speed = new ClampedFloatParameter(0.8f, 0f, 1f);

        public FloatParameter Offset = new ClampedFloatParameter(1f, 0f, 13f);

        public FloatParameter Alpha = new ClampedFloatParameter(1f, 0f, 1f);
    }


    public class GlitchLineBlockRenderer : VolumeRenderer<GlitchLineBlock>
    {

        public override string PROFILER_TAG => "GlitchLineBlock";
        public override string ShaderName => "Hidden/PostProcessing/Glitch/LineBlock";


        private float TimeX = 1.0f;
        private float randomFrequency;
        private int frameCount = 0;



        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int Params2 = Shader.PropertyToID("_Params2");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            UpdateFrequency(settings);

            TimeX += Time.deltaTime;
            if (TimeX > 100)
            {
                TimeX = 0;
            }

            blitMaterial.SetVector(ShaderIDs.Params, new Vector3(
                settings.intervalType.value == IntervalType.Random ? randomFrequency : settings.frequency.value,
                TimeX * settings.Speed.value * 0.2f, settings.Amount.value));

            blitMaterial.SetVector(ShaderIDs.Params2, new Vector3(settings.Offset.value, 1 / settings.LinesWidth.value, settings.Alpha.value));

            cmd.Blit(source, target, blitMaterial, (int)settings.blockDirection.value);
        }

        void UpdateFrequency(GlitchLineBlock settings)
        {
            if (settings.intervalType.value == IntervalType.Random)
            {
                if (frameCount > settings.frequency.value)
                {

                    frameCount = 0;
                    randomFrequency = UnityEngine.Random.Range(0, settings.frequency.value);
                }
                frameCount++;
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