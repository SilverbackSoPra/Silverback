using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LevelEditor.Engine.Shader
{
    internal class Shader
    {

        protected readonly Effect mShader;
        private readonly EffectPass mMainPass;

        protected Shader(ContentManager content, string shaderPath)
        {
            mShader = content.Load<Effect>(shaderPath);

            // We could have different techniques and passes in our shaders but we
            // will use the Main part, which means we have to call it Main in the shaders too.
            mMainPass = mShader.Techniques["Main"].Passes[0];

        }

        public virtual void Apply()
        {

            mMainPass.Apply();
          
        }

    }

}