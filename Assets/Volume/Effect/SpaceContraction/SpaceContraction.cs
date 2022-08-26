using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.VOLUMEROOT + "空间收缩 SpaceContraction")]
    public class SpaceContraction : VolumeSetting
    {
        public override bool IsActive() => raderWaveTimeLine.value > 0 && raderWaveTimeLine.value < 1;

        [Tooltip("特效强度")]
        public ClampedFloatParameter raderWaveIntensity = new ClampedFloatParameter(0, 0, 1);
        [Tooltip("探测波颜色")]
        public ColorParameter raderWaveColor = new ColorParameter(Color.white);
        [Tooltip("压暗强度")]
        public FloatParameter raderAreaColor = new FloatParameter(0.2f);
        [Tooltip("弧线线颜色")]
        public ColorParameter curveColor = new ColorParameter(Color.blue);

        //[Tooltip("探测距离")]
        //public MinFloatParameter raderWaveMaxDistance = new MinFloatParameter(40,0);
        [Tooltip("探测宽度")]
        public FloatParameter raderWaveWidth = new FloatParameter(10);
        //[Tooltip("传递速度")]
        //public MinFloatParameter raderWaveSpeed = new MinFloatParameter(1, 0);
        [Tooltip("最远距离")]
        public FloatParameter maxDistance = new FloatParameter(500.0f);
        [Tooltip("最大持续时间")]
        public FloatParameter postSpeed = new FloatParameter(1.0f);
        [Tooltip("路程计算公式参数0: speedParam0 * pow(speedParam1,time) + speedParam2 * time")]
        public FloatParameter speedParam0 = new FloatParameter(1.0f);
        [Tooltip("公式加速部分： pow(speedParam1, time)")]
        public FloatParameter speedParam1 = new FloatParameter(10.0f);
        [Tooltip("公式匀速部分： speedParam2 * Time")]
        public FloatParameter speedParam2 = new FloatParameter(10.0f);
        [Tooltip("网格线颜色")]
        public ColorParameter gridColor = new ColorParameter(Color.blue);
        [Tooltip("网格线半径")]
        public FloatParameter gridRadius = new FloatParameter(40.0f);
        [Tooltip("网格宽度")]
        public FloatParameter gridLength = new FloatParameter(3.0f);

        [Tooltip("网格线开使时间")]
        public FloatParameter gridStartTime = new FloatParameter(0.3f);
        [Tooltip("网格线扩散速度")]
        public FloatParameter gridSpeed = new FloatParameter(40.0f);
        //[Tooltip("边缘贴图")]
        //public TextureParameter edgeTexture = new TextureParameter(null);
        [HideInInspector]
        public Vector4Parameter originPos = new Vector4Parameter(Vector4.zero, true);
        [Tooltip("传递时间"), HideInInspector]
        public MinFloatParameter raderWaveTime = new MinFloatParameter(10, 0, true);
        [Tooltip("探测距离"), HideInInspector]
        public FloatParameter raderWaveDistance = new FloatParameter(0, true);
        [Tooltip("时间线"), HideInInspector]
        public ClampedFloatParameter raderWaveTimeLine = new ClampedFloatParameter(0, 0, 1, true);
        [HideInInspector]
        public Vector4Parameter positionBuffer0 = new Vector4Parameter(Vector4.zero, true);
        [HideInInspector]
        public Vector4Parameter positionBuffer1 = new Vector4Parameter(Vector4.zero, true);
        [HideInInspector]
        public Vector4Parameter positionBuffer2 = new Vector4Parameter(Vector4.zero, true);
        [HideInInspector]
        public Vector4Parameter positionBuffer3 = new Vector4Parameter(Vector4.zero, true);


    }


    public class SpaceContractionRenderer : VolumeRenderer<SpaceContraction>
    {
        public override string ShaderName => "Hidden/PostProcessing/SpaceContraction";

        public override string PROFILER_TAG => "Space Contraction";

        static class ShaderContants
        {
            public static readonly int raderOriginPosID = Shader.PropertyToID("_OriginPosition");
            public static readonly int raderWaveColorID = Shader.PropertyToID("_WaveColor");
            public static readonly int raderMaxDistanceID = Shader.PropertyToID("_MaxDistance");
            public static readonly int raderWaveWidthID = Shader.PropertyToID("_RaderWaveWidth");
            public static readonly int raderWaveAreaColorID = Shader.PropertyToID("_SCBrightness");
            public static readonly int curveColorID = Shader.PropertyToID("_CurveColor");
            public static readonly int gridColorID = Shader.PropertyToID("_GridColor");
            public static readonly int raderwaveTimeLineID = Shader.PropertyToID("_RaderWaveTimeLine");
            public static readonly int raderwaveTimeCountID = Shader.PropertyToID("_RaderWaveTImeCount");
            public static readonly int raderWaveIntensityID = Shader.PropertyToID("_RaderWaveIntensity");
            public static readonly int gridStartTimeID = Shader.PropertyToID("_GridStartTime");
            public static readonly int gridSpeedID = Shader.PropertyToID("_GridSpeed");
            public static readonly int gridRadiusID = Shader.PropertyToID("_GridRadius");
            public static readonly int gridLengthID = Shader.PropertyToID("_GirdLength");
            public static readonly int positionBufferID = Shader.PropertyToID("_PositionBuffer0");
            public static readonly int areaBrightnessID = Shader.PropertyToID("_AreaBrightness");
            public static readonly int positionBuffer1ID = Shader.PropertyToID("_PositionBuffer1");
            //public static readonly int edgeTextureID = Shader.PropertyToID("_EdgTexture");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            Vector4 vec = settings.originPos.value;
            vec.w = settings.raderWaveDistance.value;

            blitMaterial.SetVector(ShaderContants.raderOriginPosID, vec);
            blitMaterial.SetVector(ShaderContants.raderWaveColorID, settings.raderWaveColor.value);
            blitMaterial.SetFloat(ShaderContants.raderMaxDistanceID, settings.maxDistance.value);
            blitMaterial.SetFloat(ShaderContants.raderWaveAreaColorID, settings.raderAreaColor.value);
            blitMaterial.SetFloat(ShaderContants.areaBrightnessID, 1);
            blitMaterial.SetFloat(ShaderContants.raderWaveIntensityID, settings.raderWaveIntensity.value);
            blitMaterial.SetFloat(ShaderContants.raderWaveWidthID, settings.raderWaveWidth.value);
            blitMaterial.SetFloat(ShaderContants.gridStartTimeID, settings.gridStartTime.value);
            blitMaterial.SetFloat(ShaderContants.gridRadiusID, settings.gridRadius.value);
            blitMaterial.SetFloat(ShaderContants.gridSpeedID, settings.gridSpeed.value);
            blitMaterial.SetColor(ShaderContants.curveColorID, settings.curveColor.value);
            blitMaterial.SetColor(ShaderContants.gridColorID, settings.gridColor.value);
            blitMaterial.SetFloat(ShaderContants.gridLengthID, settings.gridLength.value);
            blitMaterial.SetFloat(ShaderContants.raderwaveTimeLineID, settings.raderWaveTimeLine.value);
            blitMaterial.SetFloat(ShaderContants.raderwaveTimeCountID, settings.raderWaveTime.value);
            blitMaterial.SetVector(ShaderContants.positionBufferID, settings.positionBuffer0.value);
            blitMaterial.SetVector(ShaderContants.positionBuffer1ID, settings.positionBuffer1.value);
            //blitMaterial.SetTexture(ShaderContants.edgeTextureID, settings.edgeTexture.value);

            cmd.Blit(source, target, blitMaterial);




        }
    }

}