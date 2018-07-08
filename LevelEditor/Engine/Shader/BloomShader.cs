using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.Engine.Shader
{
    internal sealed class BloomShader : Shader
    {

        // In this case we need two additional passes because we also need to blur the image
        private readonly EffectPass mHorizontalBlurPass;
        private readonly EffectPass mVerticalBlurPass;
        private readonly EffectPass mScale;

        private readonly EffectParameter mBloomThresholdParameter;
        private readonly EffectParameter mBloomPowerParameter;
        private readonly EffectParameter mBloomIntensityParameter;
        private readonly EffectParameter mSourceParameter;
        private readonly EffectParameter mBlurScaleParameter;

        public float mBloomThreshold;
        public float mBloomPower;
        public float mBloomIntensity;


        public BloomShader(ContentManager content, string shaderPath) : base(content, shaderPath)
        {

            mHorizontalBlurPass = mShader.Techniques["Blur"].Passes["Horizontal"];
            mVerticalBlurPass = mShader.Techniques["Blur"].Passes["Vertical"];
            mScale = mShader.Techniques["Scale"].Passes["Scale"];

            mBloomThresholdParameter = mShader.Parameters["bloomThreshold"];
            mBloomPowerParameter = mShader.Parameters["bloomPower"];
            mBloomIntensityParameter = mShader.Parameters["bloomIntensity"];
            mSourceParameter = mShader.Parameters["source"];
            mBlurScaleParameter = mShader.Parameters["blurScale"];

        }

        public void ApplyMain(Texture2D source)
        {

            mBloomThresholdParameter.SetValue(mBloomThreshold);
            mBloomPowerParameter.SetValue(mBloomPower);
            mBloomIntensityParameter.SetValue(mBloomIntensity);

            mSourceParameter.SetValue(source);

            Apply();
        }

        public void ApplyHorizontalBlur(Texture2D source, float blurScale)
        {

            mSourceParameter.SetValue(source);
            mBlurScaleParameter.SetValue(new Vector2(1.0f / blurScale));

            mHorizontalBlurPass.Apply();
        }

        public void ApplyVerticalBlur(Texture2D source, float blurScale)
        {

            mSourceParameter.SetValue(source);
            mBlurScaleParameter.SetValue(new Vector2(1.0f / blurScale));

            mVerticalBlurPass.Apply();
        }

        public void ApplyScale(Texture2D source)
        {

            mSourceParameter.SetValue(source);
            mScale.Apply();
        }


    }
}
