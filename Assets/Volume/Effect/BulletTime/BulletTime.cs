using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.VOLUMEROOT + "子弹时间 (Bullet Time)")]
    public class BulletTime : VolumeSetting
    {
        public override bool IsActive() => bulletTimeConcenration.value > 0;

        [Tooltip("持续时间")]
        public MinFloatParameter bulletTimesTime = new MinFloatParameter(5f, 0f);
        //[Tooltip("光晕遮罩")]
        //public TextureParameter bulletTimeBloomMask = new TextureParameter(null);
        //[Tooltip("遮罩最大尺寸")]
        //public FloatParameter bulletTimeMaskMaxSize = new FloatParameter(4);
        [Tooltip("光亮范围")]
        public FloatParameter bulletSpotSize = new FloatParameter(3.5f);

        [Tooltip("过滤颜色")]
        public ColorParameter bulletTimeScreenColor = new ColorParameter(Color.blue);

        [Tooltip("特效开始阶段时长")]
        public MinFloatParameter startStepTime = new MinFloatParameter(0.5f, 0);
        [Tooltip("特效结束阶段时长")]
        public MinFloatParameter endStepTime = new MinFloatParameter(0.5f, 0);

        [Tooltip("特效曲线线")]
        public ClampedFloatParameter bulletTimeConcenration = new ClampedFloatParameter(0, 0, 1);

        public Vector3Parameter originPos = new Vector3Parameter(Vector3.one, true);

    }

    public class BulletTimeRenderer : VolumeRenderer<BulletTime>
    {
        public override string PROFILER_TAG => "Bullet Time";
        public override string ShaderName => "Hidden/PostProcessing/BulletTime2";



        static class ShaderContants
        {
            public static readonly int bulletTimeOriginPositionID = Shader.PropertyToID("_BulletTimeOriginPos");
            public static readonly int bulletTimeScreenColorID = Shader.PropertyToID("_BulletTimeColor");
            public static readonly int bulletTimeConcemtrationID = Shader.PropertyToID("_BulletTimeConcen");
            public static readonly int bulletTimeSpotSizeID = Shader.PropertyToID("_SpotSize");
            //public static readonly int bulletTimeBloomMaskID = Shader.PropertyToID("_BulletTimeBloomMask");
            //public static readonly int bulletTimeBloomMaskSizeID = Shader.PropertyToID("_BulletBloomMaskSize");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {


            //settings.bulletTimeConcenration.value = 0.98f;
            //m_BliteMaterial.SetTexture(ShaderContants.bulletTimeBloomMaskID, settings.bulletTimeBloomMask.value);
            //m_BliteMaterial.SetFloat(ShaderContants.bulletTimeBloomMaskSizeID, settings.bulletTimeMaskMaxSize.value);
            blitMaterial.SetVector(ShaderContants.bulletTimeOriginPositionID, settings.originPos.value);
            blitMaterial.SetFloat(ShaderContants.bulletTimeConcemtrationID, settings.bulletTimeConcenration.value);
            blitMaterial.SetVector(ShaderContants.bulletTimeScreenColorID, settings.bulletTimeScreenColor.value);
            //Debugger.LogError(settings.bulletSpotSize.value);
            blitMaterial.SetFloat(ShaderContants.bulletTimeSpotSizeID, settings.bulletSpotSize.value);
            //Debugger.LogError(m_BlitMaterial.GetFloat(ShaderContants.bulletTimeSpotSizeID));

            cmd.Blit(source, target, blitMaterial);
        }


    }

}