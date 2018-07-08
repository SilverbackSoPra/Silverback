using LevelEditor.Engine.Shader;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace LevelEditor.Engine.Renderer
{
    internal sealed class FxaaRenderer : IRenderer
    {

        private readonly FxaaShader mShader;

        private readonly GraphicsDevice mDevice;
        private bool mDisposed = false;

        public FxaaRenderer(GraphicsDevice device, ContentManager content, string shaderPath)
        {

            mDevice = device;

            mShader = new FxaaShader(content, shaderPath);

        }

        public void Render(RenderTarget target, Camera camera, Scene scene)
        {

            mShader.mAlbedoMap = target.mPostProcessRenderTarget;
            mShader.mLumaThreshold = scene.mPostProcessing.mFxaa.mLumaThreshold;
            mShader.mLumaThresholdMin = scene.mPostProcessing.mFxaa.mLumaThresholdMin;
            mShader.mDebug = scene.mPostProcessing.mFxaa.mDebugMode;
            mShader.mFramebufferResolution = new Vector2(target.mPostProcessRenderTarget.Width, target.mPostProcessRenderTarget.Height);

            mShader.Apply();

            mDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 4);

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {

            if (mDisposed)
            {
                return;
            }

            if (disposing)
            {
            }

            mDisposed = true;

        }

    }

}
