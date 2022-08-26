using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace XPostProcessing
{
    public class CustomVolumePass_BeforeRenderingTransparents : BaseVolumePass
    {
        protected override string RenderPostProcessingTag => "Custom Render PostProcessing Effects_BeforeRenderingTransparents";


        protected override void OnInit()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
            //renderPassEvent = RenderPassEvent.AfterRenderingTransparents - 1;
            //....

            AddEffect(new DepthFogRenderer());
            AddEffect(new SpaceContractionRenderer());
            AddEffect(new BulletTimeRenderer());
            AddEffect(new BulletTimeBlurRenderer());

        }
    }

}