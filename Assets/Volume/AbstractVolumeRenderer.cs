using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    public abstract class AbstractVolumeRenderer
    {
        public abstract bool IsActive();
        public virtual void Init() { }
        public abstract void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target);
    }

    public abstract class VolumeRenderer<T> : AbstractVolumeRenderer where T : VolumeSetting
    {
        public T settings => VolumeManager.instance.stack.GetComponent<T>();
        public override bool IsActive() => settings.IsActive();

        // public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        // {

        //     if (settings.IsActive())
        //         return;

        //     Render(cmd, source, target);
        // }

        // public abstract void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target);

    }

}