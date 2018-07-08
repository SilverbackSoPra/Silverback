using LevelEditor.Engine.Shader;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LevelEditor.Engine.Renderer
{
    internal sealed class PostProcessRenderer : IRenderer
    {

        private readonly PostProcessShader mShader;

        private readonly GraphicsDevice mDevice;

        private bool mDisposed = false;

        public PostProcessRenderer(GraphicsDevice device, ContentManager content, string shaderPath)
        {
            mDevice = device;

            mShader = new PostProcessShader(content, shaderPath);

        }

        public void Render(RenderTarget target, Camera camera, Scene scene)
        {

            mShader.mAlbedoMap = target.mMainRenderTarget;

            mShader.mBloom1 = target.mBloomRenderTarget1Vertical;
            mShader.mBloom2 = target.mBloomRenderTarget2Vertical;
            mShader.mBloom3 = target.mBloomRenderTarget3Vertical;
            mShader.mBloom4 = target.mBloomRenderTarget4Vertical;
            mShader.mBloom5 = target.mBloomRenderTarget5Vertical;

            mShader.mSaturation = scene.mPostProcessing.mSaturation;

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
