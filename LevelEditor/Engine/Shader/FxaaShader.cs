using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.Engine.Shader
{
    internal sealed class FxaaShader : Shader
    {

        private readonly EffectParameter mAlbedoMapParameter;
        private readonly EffectParameter mLumaThresholdParameter;
        private readonly EffectParameter mLumaThresholdMinParameter;
        private readonly EffectParameter mDebugParameter;
        private readonly EffectParameter mFramebufferResolutionParameter;

        public Texture2D mAlbedoMap;
        public float mLumaThreshold;
        public float mLumaThresholdMin;
        public bool mDebug;
        public Vector2 mFramebufferResolution;


        public FxaaShader(ContentManager content, string shaderPath) : base(content, shaderPath)
        {

            mAlbedoMapParameter = mShader.Parameters["albedoMap"];
            mLumaThresholdParameter = mShader.Parameters["lumaThreshold"];
            mLumaThresholdMinParameter = mShader.Parameters["lumaThresholdMin"];
            mDebugParameter = mShader.Parameters["debug"];
            mFramebufferResolutionParameter = mShader.Parameters["framebufferResolution"];

        }

        public override void Apply()
        {

            mAlbedoMapParameter.SetValue(mAlbedoMap);
            mLumaThresholdParameter.SetValue(mLumaThreshold);
            mLumaThresholdMinParameter.SetValue(mLumaThresholdMin);
            mDebugParameter.SetValue(mDebug);
            mFramebufferResolutionParameter.SetValue(mFramebufferResolution);

            base.Apply();
        }

    }
}
