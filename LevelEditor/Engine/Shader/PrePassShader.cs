using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.Engine.Shader
{
    sealed class PrePassShader : Shader
    {

        private readonly EffectParameter mModelMatrixParameter;
        private readonly EffectParameter mViewMatrixParameter;
        private readonly EffectParameter mProjectionMatrixParameter;

        public Matrix mViewMatrix;
        public Matrix mProjectionMatrix;

        public PrePassShader(ContentManager content, string shaderPath) : base(content, shaderPath)
        {

            mModelMatrixParameter = mShader.Parameters["modelMatrix"];
            mViewMatrixParameter = mShader.Parameters["viewMatrix"];
            mProjectionMatrixParameter = mShader.Parameters["projectionMatrix"];

        }

        public override void Apply()
        {

            mViewMatrixParameter.SetValue(mViewMatrix);
            mProjectionMatrixParameter.SetValue(mProjectionMatrix);

            base.Apply();

        }

        public void ApplyModelMatrix(Matrix modelMatrix)
        {
            mModelMatrixParameter?.SetValue(modelMatrix);
            base.Apply();
        }

    }

}
