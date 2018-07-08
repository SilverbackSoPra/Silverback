using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.Engine.Shader
{
    internal sealed class ShadowShader : Shader
    {

        private readonly EffectParameter mModelMatrixParameter;
        private readonly EffectParameter mLightSpaceMatrixParameter;
        private readonly EffectParameter mBonesTransformationParameter;

        public Matrix mLightSpaceMatrix;

        public ShadowShader(ContentManager content, string shaderPath) : base(content, shaderPath)
        {

            mModelMatrixParameter = mShader.Parameters["modelMatrix"];
            mLightSpaceMatrixParameter = mShader.Parameters["lightSpaceMatrix"];

            mBonesTransformationParameter = mShader.Parameters["bonesTransformation"];

        }

        public override void Apply()
        {

            mLightSpaceMatrixParameter.SetValue(mLightSpaceMatrix);

            base.Apply();

        }

        public void ApplyModelMatrix(Matrix modelMatrix)
        {
            mModelMatrixParameter.SetValue(modelMatrix);
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
