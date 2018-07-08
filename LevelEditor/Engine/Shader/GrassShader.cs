using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using OpenTK.Audio.OpenAL;

namespace LevelEditor.Engine.Shader
{
    internal sealed class GrassShader : Shader
    {

        private readonly EffectParameter mViewMatrixParameter;
        private readonly EffectParameter mProjectionMatrixParameter;
        private readonly EffectParameter mGrassTextureParameter;
        private readonly EffectParameter mFarPlaneParameter;
        private readonly EffectParameter mFogColorParameter;
        private readonly EffectParameter mFogDistanceParameter;
        private readonly EffectParameter mTimeParameter;
        private readonly EffectParameter mLightSpaceMatrixParameter;
        private readonly EffectParameter mShadowMapParameter;
        private readonly EffectParameter mShadowBiasParameter;
        private readonly EffectParameter mShadowDistanceParameter;
        private readonly EffectParameter mGlobalLightDiffuseColorParameter;
        private readonly EffectParameter mGlobalLightAmbientParameter;

        public Matrix mViewMatrix;
        public Matrix mProjectionMatrix;
        public Texture2D mGrassTexture;

        public float mFarPlane;
        public Vector3 mFogColor;
        public float mFogDistance;

        public Matrix mLightSpaceMatrix;
        public Texture2D mShadowMap;
        public float mShadowBias;
        public float mShadowDistance;

        public Vector3 mGlobalLightDiffuseColor;
        public float mGlobalLightAmbient;

        public float mTime;

        public GrassShader(ContentManager content, string shaderPath) : base(content, shaderPath)
        {

            mViewMatrixParameter = mShader.Parameters["viewMatrix"];
            mProjectionMatrixParameter = mShader.Parameters["projectionMatrix"];
            mGrassTextureParameter = mShader.Parameters["grassTexture"];

            mFarPlaneParameter = mShader.Parameters["farPlane"];
            mFogColorParameter = mShader.Parameters["fogColor"];
            mFogDistanceParameter = mShader.Parameters["fogDistance"];

            mLightSpaceMatrixParameter = mShader.Parameters["lightSpaceMatrix"];
            mShadowMapParameter = mShader.Parameters["shadowMap"];
            mShadowBiasParameter = mShader.Parameters["shadowBias"];
            mShadowDistanceParameter = mShader.Parameters["shadowDistance"];

            mGlobalLightDiffuseColorParameter = mShader.Parameters["lightDiffuseColor"];
            mGlobalLightAmbientParameter = mShader.Parameters["lightAmbient"];

            mTimeParameter = mShader.Parameters["time"];

        }

        public override void Apply()
        {

            mViewMatrixParameter.SetValue(mViewMatrix);
            mProjectionMatrixParameter.SetValue(mProjectionMatrix);
            mGrassTextureParameter.SetValue(mGrassTexture);

            mFarPlaneParameter.SetValue(mFarPlane);
            mFogColorParameter.SetValue(mFogColor);
            mFogDistanceParameter.SetValue(mFogDistance);
            
            mLightSpaceMatrixParameter.SetValue(mLightSpaceMatrix);
            mShadowMapParameter.SetValue(mShadowMap);
            mShadowBiasParameter.SetValue(mShadowBias);
            mShadowDistanceParameter.SetValue(mShadowDistance);

            mGlobalLightDiffuseColorParameter.SetValue(mGlobalLightDiffuseColor);
            mGlobalLightAmbientParameter.SetValue(mGlobalLightAmbient);

            mTimeParameter.SetValue(mTime);

            base.Apply();

        }
    }
}
