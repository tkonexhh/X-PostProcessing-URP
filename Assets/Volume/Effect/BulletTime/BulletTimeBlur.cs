using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.VOLUMEROOT + "子弹时间模糊 (Bullet Time Blur)")]
    public class BulletTimeBlur : VolumeSetting
    {
        public override bool IsActive() => bulletBlurControl.value > 0;
        [Tooltip("特效控制值")]
        public ClampedFloatParameter bulletBlurControl = new ClampedFloatParameter(0, 0, 1, true);
        public ClampedFloatParameter bulletUnBlurRadius = new ClampedFloatParameter(0.2f, 0, 1, true);
        public MinFloatParameter bulletBlurLength = new MinFloatParameter(0.55f, 0.1f, true);
        public Vector2Parameter bulletBlurCenterPoint = new Vector2Parameter(new Vector2(0.5f, 0.65f), true);

        public MinFloatParameter bulletBlurStartTime = new MinFloatParameter(0.2f, 0, true);
        public MinFloatParameter bulletBlurEndTime = new MinFloatParameter(0.05f, 0, true);

    }


    public class BulletTimeBlurRenderer : VolumeRenderer<BulletTimeBlur>
    {
        public override string PROFILER_TAG => "Bullet Time Blur";
        public override string ShaderName => "Hidden/PostProcessing/BulletTimeBlur22";




        static class ShaderContants
        {
            public static readonly int bulletTimeBlurControlID = Shader.PropertyToID("_BulletTimeBlurControl");
            public static readonly int bulletTimeUnBlurRadiusID = Shader.PropertyToID("_BulletTimeUnBlurRadius");
            public static readonly int bulletTimeBlurCenterPointID = Shader.PropertyToID("_BulletTimeBlurCenterPoint");
        }
        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {

            blitMaterial.SetVector(ShaderContants.bulletTimeBlurCenterPointID, settings.bulletBlurCenterPoint.value);
            blitMaterial.SetFloat(ShaderContants.bulletTimeBlurControlID, settings.bulletBlurControl.value * settings.bulletBlurLength.value);
            blitMaterial.SetFloat(ShaderContants.bulletTimeUnBlurRadiusID, settings.bulletUnBlurRadius.value);
            cmd.Blit(source, target, blitMaterial);
        }
    }

}