using LevelEditor.Engine.Shader;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LevelEditor.Engine.Renderer
{
    internal sealed class BloomRenderer : IRenderer
    {

        private readonly BloomShader mShader;

        private readonly GraphicsDevice mDevice;

        private bool mDisposed = false;

        public BloomRenderer(GraphicsDevice device, ContentManager content, string shaderPath)
        {

            mDevice = device;

            mShader = new BloomShader(content, shaderPath);

        }


        public void Render(RenderTarget target, Camera camera, Scene scene)
        {

            mShader.mBloomThreshold = scene.mPostProcessing.mBloom.mThreshold;
            mShader.mBloomPower = scene.mPostProcessing.mBloom.mPower;
            mShader.mBloomIntensity = scene.mPostProcessing.mBloom.mIntensity;

            mShader.ApplyMain(target.mMainRenderTarget);

            mDevice.SetRenderTarget(target.mBloomRenderTarget1Vertical);

            mDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 4);

            for (var i = 0; i < 5; i++)
            {

                RenderTarget2D horizontal = null;
                RenderTarget2D vertical = null;
                RenderTarget2D source = null;

                switch (i)
                {
                    case 0: horizontal = target.mBloomRenderTarget1Horizontal;
                        vertical = target.mBloomRenderTarget1Vertical;
                        source = target.mBloomRenderTarget1Vertical;
                        break;
                    case 1:
                        horizontal = target.mBloomRenderTarget2Horizontal;
                        vertical = target.mBloomRenderTarget2Vertical;
                        source = target.mBloomRenderTarget1Vertical;
                        break;
                    case 2:
                        horizontal = target.mBloomRenderTarget3Horizontal;
                        vertical = target.mBloomRenderTarget3Vertical;
                        source = target.mBloomRenderTarget2Vertical;
                        break;
                    case 3:
                        horizontal = target.mBloomRenderTarget4Horizontal;
                        vertical = target.mBloomRenderTarget4Vertical;
                        source = target.mBloomRenderTarget3Vertical;
                        break;
                    case 4:
                        horizontal = target.mBloomRenderTarget5Horizontal;
                        vertical = target.mBloomRenderTarget5Vertical;
                        source = target.mBloomRenderTarget4Vertical;
                        break;
                }

                if (horizontal != null)
                {
                    mShader.ApplyHorizontalBlur(source, horizontal.Width);
                    mDevice.SetRenderTarget(horizontal);

                    mDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 4);

                    mShader.ApplyVerticalBlur(horizontal, vertical.Height);
                }

                mDevice.SetRenderTarget(vertical);

                mDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 4);

            }

        }

        public void Dispose()
        {
            Dispose(true);
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
