using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Story + "空间扭曲鱼眼 (SpaceWarp)")]
    public class SpaceWarp : VolumeSetting
    {
        public override bool IsActive() => spaceWarpEnable.value;
        [Tooltip("启用")]
        public BoolParameter spaceWarpEnable = new BoolParameter(false, true);

        [Tooltip("鱼眼强度")]
        public ClampedFloatParameter spaceWarpIntensity = new ClampedFloatParameter(0, 0, 0.99f, true);


    }




    public class SpaceWarpRenderer : VolumeRenderer<SpaceWarp>
    {
        public override string ShaderName => "Hidden/PostProcessing/SpaceWarp";
        public override string PROFILER_TAG => "SPACEWARP";

        static class ShaderIDs
        {
            public static readonly int spaceWarpIntensityID = Shader.PropertyToID("_SpaceWarpLength");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetFloat(ShaderIDs.spaceWarpIntensityID, settings.spaceWarpIntensity.value);

            cmd.Blit(source, target, blitMaterial, 0);
        }
    }



}