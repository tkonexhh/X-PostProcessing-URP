using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    public abstract class AbstractVolumeRenderer
    {
        public abstract string PROFILER_TAG { get; }
        public abstract bool IsActive(ref RenderingData renderingData);
        public virtual void Init() { }
        public abstract void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData);
        public abstract void Cleanup();
    }

    public abstract class VolumeRenderer<T> : AbstractVolumeRenderer where T : VolumeSetting
    {
        public abstract string ShaderName { get; }
        protected Material blitMaterial;

        public T settings => VolumeManager.instance.stack.GetComponent<T>();

        public override void Init()
        {
            var shader = Shader.Find(ShaderName);
            if (shader != null)
                blitMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        public override bool IsActive(ref RenderingData renderingData)
        {
            if (blitMaterial == null)
                return false;
            return settings.IsActive() && CheckActive(ref renderingData);
        }

        public override void Cleanup()
        {
            if (blitMaterial != null)
            {
                CoreUtils.Destroy(blitMaterial);
            }
        }

        public virtual bool CheckActive(ref RenderingData renderingData) => true;
    }

}