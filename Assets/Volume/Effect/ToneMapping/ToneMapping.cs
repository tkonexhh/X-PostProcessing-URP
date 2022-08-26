using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace XPostProcessing
{
    public enum TonemappingMode
    {
        None,
        GranTurismo,
        ACES,
    }

    [System.Serializable]
    public sealed class TonemappingTypeParameter : VolumeParameter<TonemappingMode> { public TonemappingTypeParameter(TonemappingMode value, bool overrideState = false) : base(value, overrideState) { } }

    [VolumeComponentMenu(VolumeDefine.VOLUMEROOT + "Toonmapping")]
    public class ToneMapping : VolumeSetting
    {
        public override bool IsActive() => Mode.value != TonemappingMode.None;
        public override bool IsTileCompatible() => true;

        public TonemappingTypeParameter Mode = new TonemappingTypeParameter(TonemappingMode.None);
    }

    public class TonemappingRenderer : VolumeRenderer<ToneMapping>
    {
        public override string PROFILER_TAG => "Tonemapping";
        public override string ShaderName => "Hidden/PostProcessing/ToneMapping";



        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            if (settings.Mode.value == TonemappingMode.GranTurismo)
            {
                cmd.Blit(source, target, blitMaterial, 0);
            }
            else if (settings.Mode.value == TonemappingMode.ACES)
            {
                cmd.Blit(source, target, blitMaterial, 1);
            }

        }
    }

}