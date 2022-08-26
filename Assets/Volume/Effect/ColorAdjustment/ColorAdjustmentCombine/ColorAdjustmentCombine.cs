using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.ColorAdjustment + "ColorAdjustmentCombine")]
    public class ColorAdjustmentCombine : VolumeSetting
    {
        public override bool IsActive() => Enable.value;
        public BoolParameter Enable = new BoolParameter(false);

        //集合的ColorAdjustment
        //屏幕灰化
        public ClampedFloatParameter Saturation = new ClampedFloatParameter(0, 0f, 1f);
    }

}