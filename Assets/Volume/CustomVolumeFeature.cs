using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{

    public class CustomVolumeFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Settings
        {
            public RenderPassEvent RenderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public Settings settings;

        CustomVolumePass m_CustomVolumePass;


        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.postProcessEnabled)
            {
                var source = new RenderTargetHandle(renderer.cameraColorTarget);
                var destination = new RenderTargetHandle(renderer.cameraColorTarget);

                m_CustomVolumePass.renderPassEvent = settings.RenderPassEvent;
                m_CustomVolumePass.Setup(renderer.cameraColorTarget);
                renderer.EnqueuePass(m_CustomVolumePass);
            }
        }

        public override void Create()
        {
            if (m_CustomVolumePass == null)
            {
                m_CustomVolumePass = new CustomVolumePass();

            }

        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }


    }

}