using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.Engine.Shader
{
    internal sealed class PostProcessShader : Shader
    {

        private readonly EffectParameter mAlbedoMapParameter;
        private readonly EffectParameter mBloomPassesParameter;
        private readonly EffectParameter mBloom1Parameter;
        private readonly EffectParameter mBloom2Parameter;
        private readonly EffectParameter mBloom3Parameter;
        private readonly EffectParameter mBloom4Parameter;
        private readonly EffectParameter mBloom5Parameter;
        private readonly EffectParameter mSaturationParameter;
        

        public Texture2D mAlbedoMap;
        public float mBloomPasses;
        public Texture2D mBloom1;
        public Texture2D mBloom2;
        public Texture2D mBloom3;
        public Texture2D mBloom4;
        public Texture2D mBloom5;

        public float mSaturation;

        public PostProcessShader(ContentManager content, string shaderPath) : base(content, shaderPath)
        {

            mAlbedoMapParameter = mShader.Parameters["albedoMap"];

            mBloomPassesParameter = mShader.Parameters["bloomPasses"];
            mBloom1Parameter = mShader.Parameters["bloom1"];
            mBloom2Parameter = mShader.Parameters["bloom2"];
            mBloom3Parameter = mShader.Parameters["bloom3"];
            mBloom4Parameter = mShader.Parameters["bloom4"];
            mBloom5Parameter = mShader.Parameters["bloom5"];

            mSaturationParameter = mShader.Parameters["saturation"];

        }

        public override void Apply()
        {

            mAlbedoMapParameter.SetValue(mAlbedoMap);

            mBloomPassesParameter.SetValue(mBloomPasses);
            mBloom1Parameter.SetValue(mBloom1);
            mBloom2Parameter.SetValue(mBloom2);
            mBloom3Parameter.SetValue(mBloom3);
            mBloom4Parameter.SetValue(mBloom4);
            mBloom5Parameter.SetValue(mBloom5);

            mSaturationParameter.SetValue(mSaturation);

            base.Apply();
        }

    }
}
