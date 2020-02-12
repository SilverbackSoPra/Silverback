using LevelEditor.Engine.Mesh;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.Engine.Shader
{

    internal sealed class ForwardShader : Shader
    {
        
        private readonly EffectParameter mModelMatrixParameter;
        private readonly EffectParameter mViewMatrixParameter;
        private readonly EffectParameter mProjectionMatrixParameter;
        private readonly EffectParameter mDiffuseMapParameter;
        private readonly EffectParameter mHasDiffuseMapParameter;
        private readonly EffectParameter mDiffuseColorParameter;
        private readonly EffectParameter mGlobalLightDirectionParameter;
        private readonly EffectParameter mGlobalLightColorParameter;
        private readonly EffectParameter mGlobalLightAmbientParameter;
        private readonly EffectParameter mLightSpaceMatrixParameter;
        private readonly EffectParameter mShadowMapParameter;
        private readonly EffectParameter mShadowBiasParameter;
        private readonly EffectParameter mShadowNumSamplesParameter;
        private readonly EffectParameter mShadowSampleRangeParameter;
        private readonly EffectParameter mShadowDistanceParameter;
        private readonly EffectParameter mShadowResolutionParameter;
        private readonly EffectParameter mBonesTransformationParameter;
        private readonly EffectParameter mSpecularIntensityParameter;
        private readonly EffectParameter mSpecularHardnessParameter;
        private readonly EffectParameter mUpVectorParameter;
        private readonly EffectParameter mFarPlaneParameter;
        private readonly EffectParameter mFogColorParameter;
        private readonly EffectParameter mFogDistanceParameter;
        private readonly EffectParameter mActorColorParameter;

        public Matrix mViewMatrix;
        public Matrix mProjectionMatrix;

        public Vector3 mGlobalLightDirection;
        public Vector3 mGlobalLightColor;
        public float mGlobalLightAmbient;

        public Matrix mLightSpaceMatrix;
        public Texture2D mShadowMap;
        public float mShadowBias;
        public int mShadowNumSamples;
        public float mShadowSampleRange;
        public float mShadowDistance;
        public float mShadowResolution;
        public Vector3 mUp;
        public float mFarPlane;
        public Vector3 mFogColor;
        public float mFogDistance;

        public ForwardShader(ContentManager content, string shaderPath) : base(content, shaderPath)
        { 

            mModelMatrixParameter = mShader.Parameters["modelMatrix"];
            mViewMatrixParameter = mShader.Parameters["viewMatrix"];
            mProjectionMatrixParameter = mShader.Parameters["projectionMatrix"];

            mDiffuseMapParameter = mShader.Parameters["diffuseMap"];
            mHasDiffuseMapParameter = mShader.Parameters["hasDiffuseMap"];
            mDiffuseColorParameter = mShader.Parameters["diffuseColor"];
            mGlobalLightDirectionParameter = mShader.Parameters["lightDirection"];
            mGlobalLightColorParameter = mShader.Parameters["lightColor"];
            mGlobalLightAmbientParameter = mShader.Parameters["lightAmbient"];

            mLightSpaceMatrixParameter = mShader.Parameters["lightSpaceMatrix"];
            mShadowMapParameter = mShader.Parameters["shadowMap"];
            mShadowBiasParameter = mShader.Parameters["shadowBias"];
            mShadowNumSamplesParameter = mShader.Parameters["shadowNumSamples"];
            mShadowSampleRangeParameter = mShader.Parameters["shadowSampleRange"];
            mShadowDistanceParameter = mShader.Parameters["shadowDistance"];
            mShadowResolutionParameter = mShader.Parameters["shadowResolution"];

            mBonesTransformationParameter = mShader.Parameters["bonesTransformation"];

            mSpecularIntensityParameter = mShader.Parameters["specularIntensity"];
            mSpecularHardnessParameter = mShader.Parameters["specularHardness"];

            mUpVectorParameter = mShader.Parameters["upVector"];

            mFarPlaneParameter = mShader.Parameters["farPlane"];
            mFogColorParameter = mShader.Parameters["fogColor"];
            mFogDistanceParameter = mShader.Parameters["fogDistance"];

            mActorColorParameter = mShader.Parameters["actorColor"];

        }

        
        public override void Apply()
        {

            // TODO: We should check whether the matrices are not null
            mViewMatrixParameter.SetValue(mViewMatrix);
            mProjectionMatrixParameter.SetValue(mProjectionMatrix);

            mGlobalLightDirectionParameter.SetValue(mGlobalLightDirection);
            mGlobalLightColorParameter.SetValue(mGlobalLightColor);
            mGlobalLightAmbientParameter.SetValue(mGlobalLightAmbient);

            mLightSpaceMatrixParameter.SetValue(mLightSpaceMatrix);
            mShadowMapParameter.SetValue(mShadowMap);
            mShadowBiasParameter.SetValue(mShadowBias);
            mShadowNumSamplesParameter.SetValue(mShadowNumSamples);
            mShadowSampleRangeParameter.SetValue(mShadowSampleRange);
            mShadowDistanceParameter.SetValue(mShadowDistance);
            mShadowResolutionParameter.SetValue(mShadowResolution);

            mUpVectorParameter?.SetValue(mUp);

            mFarPlaneParameter.SetValue(mFarPlane);
            mFogColorParameter.SetValue(mFogColor);
            mFogDistanceParameter.SetValue(mFogDistance);

            base.Apply();

        }
        
        public void ApplyMaterial(Material material)
        {

            mDiffuseMapParameter?.SetValue(material.mDiffuseMap);

            mHasDiffuseMapParameter?.SetValue(material.mDiffuseMap != null);
            mDiffuseColorParameter.SetValue(material.mDiffuseColor);

            mSpecularIntensityParameter?.SetValue(material.mSpecularIntensitiy);
            mSpecularHardnessParameter?.SetValue(material.mSpecularHardness);

            base.Apply();
        }

        public void ApplyActor(Matrix modelMatrix, Vector3 color)
        {
            mModelMatrixParameter?.SetValue(modelMatrix);
            mActorColorParameter?.SetValue(color);
            base.Apply();
        }

        public void ApplyBoneTransformations(Matrix[] transformations)
        {
            if (mBonesTransformationParameter == null)
            {
                return;
            }

            mBonesTransformationParameter.SetValue(transformations);
            base.Apply();
        }

    }
}