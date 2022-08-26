using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Skill + "黑白闪 (BlackWhite)")]
    public class BlackWhite : VolumeSetting
    {
        public override bool IsActive() => Enable.value;

        public BoolParameter Enable = new BoolParameter(false);
        public Vector2Parameter Center = new Vector2Parameter(new Vector2(0.5f, 0.5f));
        public ColorParameter TintColor = new ColorParameter(Color.white);
        public ClampedFloatParameter Threshold = new ClampedFloatParameter(0.51f, 0.51f, 0.99f);


        public TextureParameter NoiseTex = new TextureParameter(null);
        public ClampedFloatParameter TillingX = new ClampedFloatParameter(0.1f, 0, 20);
        public ClampedFloatParameter TillingY = new ClampedFloatParameter(5, 0, 20);
        public ClampedFloatParameter Speed = new ClampedFloatParameter(0.1f, -10, 10);

        public TextureParameter DissolveTex = new TextureParameter(null);

        public ClampedIntParameter Change = new ClampedIntParameter(0, 0, 1);

    }

    public class BlackWhiteRenderer : VolumeRenderer<BlackWhite>
    {
        public override string ShaderName => "Hidden/PostProcessing/Skill/BlackWhite";
        public override string PROFILER_TAG => "BlackWhite";

        static class ShaderIDs
        {
            public static readonly int ParamsID = Shader.PropertyToID("_Params");
            public static readonly int Params2ID = Shader.PropertyToID("_Params2");
            public static readonly int ColorID = Shader.PropertyToID("_Color");
            public static readonly int NoiseTexID = Shader.PropertyToID("_NoiseTex");
            public static readonly int DissolveTexID = Shader.PropertyToID("_DissolveTex");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData)
        {
            blitMaterial.SetTexture(ShaderIDs.NoiseTexID, settings.NoiseTex.value);
            blitMaterial.SetTexture(ShaderIDs.DissolveTexID, settings.DissolveTex.value);
            blitMaterial.SetColor(ShaderIDs.ColorID, settings.TintColor.value);
            blitMaterial.SetVector(ShaderIDs.ParamsID, new Vector4(settings.Threshold.value, settings.Center.value.x, settings.Center.value.y, 0));
            blitMaterial.SetVector(ShaderIDs.Params2ID, new Vector4(settings.TillingX.value, settings.TillingY.value, settings.Speed.value, settings.Change.value));

            cmd.Blit(source, target, blitMaterial, 0);
        }

    }
}
