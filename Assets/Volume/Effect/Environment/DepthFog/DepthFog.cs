using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Environment + "深度雾 (Depth Fog)")]
    public class DepthFog : VolumeSetting
    {
        public override bool IsActive() => enable.value;

        public BoolParameter enable = new BoolParameter(false);
    }


    public class DepthFogRenderer : VolumeRenderer<DepthFog>
    {

        public override string PROFILER_TAG => "Depth Fog";
        public override string ShaderName => "Hidden/PostProcessing/DepthFog";

        public static bool IsEnable = true;

        static class ShaderContants
        {

        }

        public override bool CheckActive(ref RenderingData renderingData) => IsEnable;

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            cmd.Blit(source, target, blitMaterial);
        }



    }
}