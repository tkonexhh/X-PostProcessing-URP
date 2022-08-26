using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;


namespace XPostProcessing
{
    public abstract class BaseVolumePass : ScriptableRenderPass
    {
        protected abstract string RenderPostProcessingTag { get; }
        private ProfilingSampler m_ProfilingRenderPostProcessing;

        List<AbstractVolumeRenderer> m_PostProcessingRenderers = new List<AbstractVolumeRenderer>();

        RenderTargetIdentifier m_Source;
        static RenderTargetHandle m_TempRT0;
        RenderTextureDescriptor m_Descriptor;

        public static int RTIndex = 0;

        static BaseVolumePass()
        {
            m_TempRT0.Init("_TempRT" + RTIndex);
        }


        public BaseVolumePass()
        {
            //可能会重复 所以要用来自增长

#if !UNITY_IOS
            RTIndex++;
#endif

            m_ProfilingRenderPostProcessing = new ProfilingSampler(RenderPostProcessingTag);
            OnInit();
        }

        public void Setup(in RenderTargetIdentifier source)
        {
            m_Source = source;
            // m_Destination = destination;
        }

        public void RebuildMaterial()
        {
            foreach (var renderer in m_PostProcessingRenderers)
            {
                renderer.Init();
            }
        }

        protected void AddEffect(AbstractVolumeRenderer renderer)
        {
            m_PostProcessingRenderers.Add(renderer);
            renderer.Init();
        }


        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            m_Descriptor = cameraTargetDescriptor;
            // m_Descriptor.msaaSamples = 1;
            m_Descriptor.depthBufferBits = 0;
            m_Descriptor.width = m_Descriptor.width;
            m_Descriptor.height = m_Descriptor.height;


            cmd.GetTemporaryRT(m_TempRT0.id, m_Descriptor);

            ConfigureTarget(m_TempRT0.Identifier());
            ConfigureClear(ClearFlag.None, Color.white);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(m_TempRT0.id);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {

            var cmd = CommandBufferPool.Get();
            cmd.Clear();


            // 初始化临时RT
            RenderTargetIdentifier buff0 = m_TempRT0.id;
            RenderTargetIdentifier GetSource() => m_Source;
            RenderTargetIdentifier GetTarget() => buff0;

            void Swap() => CoreUtils.Swap(ref m_Source, ref buff0);


            int count = 0;
            using (new ProfilingScope(cmd, m_ProfilingRenderPostProcessing))
            {

                foreach (var renderer in m_PostProcessingRenderers)
                {
                    if (renderer.IsActive(ref renderingData))
                    {
                        cmd.BeginSample(renderer.PROFILER_TAG);
                        renderer.Render(cmd, GetSource(), GetTarget(), ref renderingData);
                        Swap();
                        count++;
                        cmd.EndSample(renderer.PROFILER_TAG);


                    }
                }


                if (count > 0 && count % 2 != 0)
                {
                    Blit(cmd, GetSource(), GetTarget());
                }
            }


            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }




        public void Cleanup()
        {
            foreach (var renderer in m_PostProcessingRenderers)
            {
                renderer.Cleanup();
            }
        }


        protected abstract void OnInit();
    }

}