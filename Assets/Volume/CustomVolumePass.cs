using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.Linq;

namespace XPostProcessing
{
    public class CustomVolumePass : BaseVolumePass
    {
        protected override string RenderPostProcessingTag => "Custom Render PostProcessing Effects";


        protected override void OnInit()// CustomVolumePass():base()
        {
            // var customVolumes = VolumeManager.instance.baseComponentTypeArray
            //     .Where(t => t.IsSubclassOf(typeof(VolumeSetting)))
            //     .Select(t => VolumeManager.instance.stack.GetComponent(t) as VolumeSetting)
            //     .ToList();
            // Debug.LogError("customVolumesL:" + customVolumes.Count);

            //SSAO
            AddEffect(new SSAORenderer());
            // AddEffect(new DepthFog2Renderer());
            AddEffect(new CloudShadowRenderer());
            AddEffect(new LightShaftRenderer());
            AddEffect(new BloomRenderer());
            //故障
            AddEffect(new GlitchRGBSplitRenderer());
            AddEffect(new GlitchRGBSplitV2Renderer());
            AddEffect(new GlitchRGBSplitV3Renderer());
            AddEffect(new GlitchRGBSplitV4Renderer());
            AddEffect(new GlitchRGBSplitV5Renderer());
            AddEffect(new GlitchDigitalStripeRenderer());
            AddEffect(new GlitchImageBlockRenderer());
            AddEffect(new GlitchImageBlockV2Renderer());
            AddEffect(new GlitchImageBlockV3Renderer());
            AddEffect(new GlitchImageBlockV4Renderer());
            AddEffect(new GlitchLineBlockRenderer());
            AddEffect(new GlitchTileJitterRenderer());
            AddEffect(new GlitchScanLineJitterRenderer());
            AddEffect(new GlitchAnalogNoiseRenderer());
            AddEffect(new GlitchScreenJumpRenderer());
            AddEffect(new GlitchScreenShakeRenderer());
            AddEffect(new GlitchWaveJitterRenderer());
            //边缘检测
            AddEffect(new EdgeDetectionRobertsRenderer());
            AddEffect(new EdgeDetectionRobertsNeonRenderer());
            AddEffect(new EdgeDetectionRobertsNeonV2Renderer());
            AddEffect(new EdgeDetectionScharrRenderer());
            AddEffect(new EdgeDetectionScharrNeonRenderer());
            AddEffect(new EdgeDetectionScharrNeonV2Renderer());
            AddEffect(new EdgeDetectionSobelRenderer());
            AddEffect(new EdgeDetectionSobelNeonRenderer());
            AddEffect(new EdgeDetectionSobelNeonV2Renderer());
            //像素化
            AddEffect(new PixelizeCircleRenderer());
            AddEffect(new PixelizeDiamondRenderer());
            AddEffect(new PixelizeHexagonRenderer());
            AddEffect(new PixelizeHexagonGridRenderer());
            AddEffect(new PixelizeLeafRenderer());
            AddEffect(new PixelizeLedRenderer());
            AddEffect(new PixelizeQuadRenderer());
            AddEffect(new PixelizeSectorRenderer());
            AddEffect(new PixelizeTriangleRenderer());


            AddEffect(new IrisBlurRenderer());
            AddEffect(new RainRippleRenderer());


            AddEffect(new SurfaceSnowRenderer());

            AddEffect(new EdageOutlineRenderer());


            //鱼眼扭曲
            AddEffect(new SpaceWarpRenderer());

            //Blur
            AddEffect(new GaussianBlurRenderer());
            AddEffect(new BoxBlurRenderer());
            AddEffect(new KawaseBlurRenderer());
            AddEffect(new DualBoxBlurRenderer());
            AddEffect(new DualGaussianBlurRenderer());
            AddEffect(new DualKawaseBlurRenderer());
            AddEffect(new DualTentBlurRenderer());
            AddEffect(new BokehBlurRenderer());
            AddEffect(new TiltShiftBlurRenderer());
            AddEffect(new IrisBlurRenderer());
            AddEffect(new IrisBlurV2Renderer());
            AddEffect(new GrainyBlurRenderer());
            AddEffect(new RadialBlurRenderer());
            AddEffect(new RadialBlurV2Renderer());
            AddEffect(new DirectionalBlurRenderer());

            //Image Processing
            AddEffect(new SharpenV1Renderer());
            AddEffect(new SharpenV2Renderer());
            AddEffect(new SharpenV3Renderer());



            //Vignette
            AddEffect(new RapidOldTVVignetteRenderer());
            AddEffect(new RapidOldTVVignetteV2Renderer());
            AddEffect(new RapidVignetteRenderer());
            AddEffect(new RapidVignetteV2Renderer());
            AddEffect(new AuroraVignetteRenderer());

            AddEffect(new AwakingEyeRenderer());

            //色彩调整
            AddEffect(new ColorAdjustmentBleachBypassRenderer());
            AddEffect(new ColorAdjustmentBrightnessRenderer());
            AddEffect(new ColorAdjustmentContrastRenderer());
            AddEffect(new ColorAdjustmentContrastV2Renderer());
            AddEffect(new ColorAdjustmentContrastV3Renderer());
            AddEffect(new ColorAdjustmentHueRenderer());
            AddEffect(new ColorAdjustmentLensFilterRenderer());
            AddEffect(new ColorAdjustmentSaturationRenderer());
            AddEffect(new ColorAdjustmentTechnicolorRenderer());
            AddEffect(new ColorAdjustmentTintRenderer());
            AddEffect(new ColorAdjustmentWhiteBalanceRenderer());
            AddEffect(new ColorAdjustmentColorReplaceRenderer());
            AddEffect(new ScreenBinarizationRenderer());

            AddEffect(new ColorAdjustmentTintPlayerPosRenderer());

            AddEffect(new BlackWhiteRenderer());

            AddEffect(new TonemappingRenderer());
        }



    }
}