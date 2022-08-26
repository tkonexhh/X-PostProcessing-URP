using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Environment + "深度雾2 (Depth Fog2)")]
    public class DepthFog2 : VolumeSetting
    {
        public override bool IsActive() => fogStrength.value > 0;

        [Tooltip("雾浓度")]
        public ClampedFloatParameter fogStrength = new ClampedFloatParameter(0f, 0.00f, 1f, true);
        [Tooltip("雾颜色")]
        public ColorParameter fogColor = new ColorParameter(Color.white);

        [Tooltip("雾变化率")]
        public ClampedFloatParameter fogHeightScale = new ClampedFloatParameter(10f, 1f, 50f);
        [Tooltip("雾生效开始距离")]
        public FloatParameter fogEnableDistance = new FloatParameter(10);
        [Tooltip("雾生效渐变区域")]
        public FloatParameter fogEnableDistanceArea = new FloatParameter(20);
        [Tooltip("雾基础高度,浓度为1的高度")]
        public FloatParameter fogBaseLevel = new FloatParameter(0);

        public ClampedFloatParameter sunScatting = new ClampedFloatParameter(0, 0, 1);
        public ColorParameter warmColor = new ColorParameter(Color.white);
        public ColorParameter coldColor = new ColorParameter(Color.white);

        //public ClampedFloatParameter distanceFogStrength = new ClampedFloatParameter(0,0,1);	
    }


    public class DepthFog2Renderer : VolumeRenderer<DepthFog2>
    {
        public override string PROFILER_TAG => "Depth Fog2";
        public override string ShaderName => "Hidden/PostProcessing/DepthFog2";


        static class ShaderContants
        {
            public static readonly int fogColorID = Shader.PropertyToID("_DepthFogColor");
            public static readonly int fogParameterID = Shader.PropertyToID("_FogParameter");
            public static readonly int fogBaseLevelID = Shader.PropertyToID("_FogBaseLevel");
            public static readonly int sunScattingID = Shader.PropertyToID("_SunScatting");
            public static readonly int warmColorID = Shader.PropertyToID("_WarmColor");
            public static readonly int coldColorID = Shader.PropertyToID("_ColdColor");
            //public static readonly int distanceFogStrengthID = Shader.PropertyToID("_DisFogStrength");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetColor(ShaderContants.fogColorID, settings.fogColor.value);
            Vector4 fogParam;
            fogParam.x = settings.fogStrength.value;
            fogParam.y = settings.fogHeightScale.value;
            fogParam.z = settings.fogEnableDistance.value;
            fogParam.w = settings.fogEnableDistanceArea.value;
            blitMaterial.SetVector(ShaderContants.fogParameterID, fogParam);
            blitMaterial.SetFloat(ShaderContants.fogBaseLevelID, settings.fogBaseLevel.value);
            blitMaterial.SetFloat(ShaderContants.sunScattingID, settings.sunScatting.value);
            blitMaterial.SetVector(ShaderContants.warmColorID, settings.warmColor.value);
            blitMaterial.SetVector(ShaderContants.coldColorID, settings.coldColor.value);
            //m_BlitMaterial.SetFloat(ShaderContants.distanceFogStrengthID,settings.distanceFogStrength.value);
            cmd.Blit(source, target, blitMaterial);

        }


    }
}