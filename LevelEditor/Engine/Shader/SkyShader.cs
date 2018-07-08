using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.Engine.Shader
{
    internal sealed class SkyShader : Shader
    {

        private readonly EffectParameter mMvpMatrixParameter;
        private readonly EffectParameter mSunLocationParameter;
        private readonly EffectParameter mTintParameter;
        private readonly EffectParameter mSunMorphParameter;
        private readonly EffectParameter mMoonParameter;
        private readonly EffectParameter mFogColorParameter;


        public Matrix mMvpMatrix;
        public Vector3 mSunLocation;
        public Vector3 mFogColor;

        public Texture2D mTint;
        public Texture2D mSunMorph;
        public Texture2D mMoon;

        public SkyShader(ContentManager content, string shaderPath) : base(content, shaderPath)
        {

            mMvpMatrixParameter = mShader.Parameters["MVPMatrix"];
            mSunLocationParameter = mShader.Parameters["sunLocation"];
            mTintParameter = mShader.Parameters["tint"];
            mSunMorphParameter = mShader.Parameters["sunMorph"];
            mMoonParameter = mShader.Parameters["moon"];
            mFogColorParameter = mShader.Parameters["fogColor"];

        }

        public override void Apply()
        {

            mMvpMatrixParameter.SetValue(mMvpMatrix);
            mSunLocationParameter.SetValue(mSunLocation);
            mFogColorParameter.SetValue(mFogColor);

            mTintParameter.SetValue(mTint);
            mSunMorphParameter.SetValue(mSunMorph);
            mMoonParameter.SetValue(mMoon);

            base.Apply();

        }

    }

}
