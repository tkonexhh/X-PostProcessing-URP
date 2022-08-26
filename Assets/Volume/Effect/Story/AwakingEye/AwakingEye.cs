using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Story + "眨眼苏醒")]
    public class AwakingEye : VolumeSetting
    {
        public override bool IsActive() => awakingEyeEnable.value;

        [Tooltip("启用")]
        public BoolParameter awakingEyeEnable = new BoolParameter(false, true);

        [Tooltip("眨眼幅度")]
        public ClampedFloatParameter awakingEyeOpenValue = new ClampedFloatParameter(0, 0, 1, true);
        [Tooltip("眼睛长度")]
        public ClampedFloatParameter awakingEyeOpenLength = new ClampedFloatParameter(0, 0, 1, true);
    }



    public class AwakingEyeRenderer : VolumeRenderer<AwakingEye>
    {
        public override string PROFILER_TAG => "AWAKING_EYE";
        public override string ShaderName => "Hidden/PostProcessing/AwakingEye";


        static class ShaderIDs
        {
            public static readonly int awakingEyeOpenValueID = Shader.PropertyToID("_EyeOpenValue");
            public static readonly int awakingEyeOpenLengthID = Shader.PropertyToID("_EyeOpenLength");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.awakingEyeOpenValueID, settings.awakingEyeOpenValue.value);
            blitMaterial.SetFloat(ShaderIDs.awakingEyeOpenLengthID, settings.awakingEyeOpenLength.value);

            cmd.Blit(source, target, blitMaterial);
        }
    }




}