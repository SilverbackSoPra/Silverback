using Microsoft.Xna.Framework.Graphics;
using System;

namespace LevelEditor.Engine
{
    /// <summary>
    /// A render target contains render buffers for every render pass of the engine.
    /// </summary>
    internal sealed class RenderTarget : IDisposable
    {
                
        public readonly Viewport mRenderViewport;

        public RenderTarget2D mMainRenderTarget;
        public RenderTarget2D mPostProcessRenderTarget;
        public RenderTarget2D mShadowRenderTarget;
        public RenderTarget2D mBloomRenderTarget1Horizontal;
        public RenderTarget2D mBloomRenderTarget1Vertical;
        public RenderTarget2D mBloomRenderTarget2Horizontal;
        public RenderTarget2D mBloomRenderTarget2Vertical;
        public RenderTarget2D mBloomRenderTarget3Horizontal;
        public RenderTarget2D mBloomRenderTarget3Vertical;
        public RenderTarget2D mBloomRenderTarget4Horizontal;
        public RenderTarget2D mBloomRenderTarget4Vertical;
        public RenderTarget2D mBloomRenderTarget5Horizontal;
        public RenderTarget2D mBloomRenderTarget5Vertical;

        private readonly GraphicsDevice mGraphicsDevice;
        private bool mDisposed = false;

        /// <summary>
        /// Constructs a <see cref="RenderTarget"/>.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="shadowMapResolution"></param>
        public RenderTarget(GraphicsDevice device, int width, int height, int shadowMapResolution)
        {

            mGraphicsDevice = device;
            mRenderViewport = new Viewport(0, 0, width, height);

            mMainRenderTarget = new RenderTarget2D(mGraphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth16);
            mPostProcessRenderTarget = new RenderTarget2D(mGraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            mShadowRenderTarget = new RenderTarget2D(mGraphicsDevice, shadowMapResolution, shadowMapResolution, false, SurfaceFormat.Single, DepthFormat.Depth16);

            mBloomRenderTarget1Horizontal = new RenderTarget2D(mGraphicsDevice, width / 2, height / 2, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget1Vertical = new RenderTarget2D(mGraphicsDevice, width / 2, height / 2, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget2Horizontal = new RenderTarget2D(mGraphicsDevice, width / 4, height / 4, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget2Vertical = new RenderTarget2D(mGraphicsDevice, width / 4, height / 4, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget3Horizontal = new RenderTarget2D(mGraphicsDevice, width / 8, height / 8, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget3Vertical = new RenderTarget2D(mGraphicsDevice, width / 8, height / 8, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget4Horizontal = new RenderTarget2D(mGraphicsDevice, width / 16, height / 16, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget4Vertical = new RenderTarget2D(mGraphicsDevice, width / 16, height / 16, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget5Horizontal = new RenderTarget2D(mGraphicsDevice, width / 32, height / 32, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget5Vertical = new RenderTarget2D(mGraphicsDevice, width / 32, height / 32, false, SurfaceFormat.HalfVector4, DepthFormat.None);

        }

        /// <summary>
        /// Resizes the normal render buffers for scene and postprocessing
        /// </summary>
        /// <param name="width">The width of the resolution</param>
        /// <param name="height">The height of the resolution</param>
        public void Resize(int width, int height)
        {

            mMainRenderTarget.Dispose();
            mPostProcessRenderTarget.Dispose();

            mBloomRenderTarget1Horizontal.Dispose();
            mBloomRenderTarget1Vertical.Dispose();
            mBloomRenderTarget2Horizontal.Dispose();
            mBloomRenderTarget2Vertical.Dispose();
            mBloomRenderTarget3Horizontal.Dispose();
            mBloomRenderTarget3Vertical.Dispose();
            mBloomRenderTarget4Horizontal.Dispose();
            mBloomRenderTarget4Vertical.Dispose();
            mBloomRenderTarget5Horizontal.Dispose();
            mBloomRenderTarget5Vertical.Dispose();

            mMainRenderTarget = new RenderTarget2D(mGraphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth16);
            mPostProcessRenderTarget = new RenderTarget2D(mGraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);

            mBloomRenderTarget1Horizontal = new RenderTarget2D(mGraphicsDevice, width / 2, height / 2, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget1Vertical = new RenderTarget2D(mGraphicsDevice, width / 2, height / 2, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget2Horizontal = new RenderTarget2D(mGraphicsDevice, width / 4, height / 4, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget2Vertical = new RenderTarget2D(mGraphicsDevice, width / 4, height / 4, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget3Horizontal = new RenderTarget2D(mGraphicsDevice, width / 8, height / 8, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget3Vertical = new RenderTarget2D(mGraphicsDevice, width / 8, height / 8, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget4Horizontal = new RenderTarget2D(mGraphicsDevice, width / 16, height / 16, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget4Vertical = new RenderTarget2D(mGraphicsDevice, width / 16, height / 16, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget5Horizontal = new RenderTarget2D(mGraphicsDevice, width / 32, height / 32, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            mBloomRenderTarget5Vertical = new RenderTarget2D(mGraphicsDevice, width / 32, height / 32, false, SurfaceFormat.HalfVector4, DepthFormat.None);

        }

        /// <summary>
        /// Resizes the shadow map render buffer.
        /// </summary>
        /// <param name="shadowMapResolution">The resolution of the shadow map</param>
        public void Resize(int shadowMapResolution)
        {
            mShadowRenderTarget.Dispose();

            mShadowRenderTarget = new RenderTarget2D(mGraphicsDevice, shadowMapResolution, shadowMapResolution, false, SurfaceFormat.Single, DepthFormat.Depth16);
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
                mMainRenderTarget.Dispose();
                mPostProcessRenderTarget.Dispose();

                mBloomRenderTarget1Horizontal.Dispose();
                mBloomRenderTarget1Vertical.Dispose();
                mBloomRenderTarget2Horizontal.Dispose();
                mBloomRenderTarget2Vertical.Dispose();
                mBloomRenderTarget3Horizontal.Dispose();
                mBloomRenderTarget3Vertical.Dispose();
                mBloomRenderTarget4Horizontal.Dispose();
                mBloomRenderTarget4Vertical.Dispose();
                mBloomRenderTarget5Horizontal.Dispose();
                mBloomRenderTarget5Vertical.Dispose();

                mShadowRenderTarget.Dispose();
            }

            mDisposed = true;

        }


    }
}
