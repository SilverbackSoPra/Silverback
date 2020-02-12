using System;
using LevelEditor.Engine.Debug;
using LevelEditor.Engine.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.Engine.Renderer
{

    /// <summary>
    /// Used to render a full screen texture as a triangle strip.
    /// </summary>
    internal struct Quad : IDisposable
    {
        private static readonly VertexTexture[] sVertices = { new VertexTexture(-1.0f, -1.0f),
            new VertexTexture(-1.0f, 1.0f), new VertexTexture(1.0f, -1.0f), new VertexTexture(1.0f, 1.0f) };

        public readonly VertexBuffer mBuffer;

        private bool mDisposed;

        public Quad(GraphicsDevice device)
        {
            mBuffer = new VertexBuffer(device, VertexTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);
            mBuffer.SetData(sVertices);
            mDisposed = false;
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
                mBuffer.Dispose();
            }

            mDisposed = true;

        }

    }

    /// <summary>
    /// The <see cref="MasterRenderer"/> is responsible for rendering the scene
    /// </summary>
    internal sealed class MasterRenderer : IDisposable
    {

        public bool PrePass {
            get { return mPrePass; }
            set { mPrePass = value; mForwardRenderer.PrePass = value; }
        }

        public bool DebugMode { get; set; }

        // The paths to the shaders
        private const string ForwardShaderPath = "Shader/Forward";
        private const string ForwardSkinnedShaderPath = "Shader/ForwardSkinned";
        private const string ForwardTerrainShaderPath = "Shader/ForwardTerrain";
        private const string PostProcessShaderPath = "Shader/PostProcess";
        private const string ShadowShaderPath = "Shader/Shadow";
        private const string ShadowSkinnedShaderPath = "Shader/ShadowSkinned";
        private const string FxaaShaderPath = "Shader/FXAA";
        private const string SkyShaderPath = "Shader/Sky";
        private const string PrePassShaderPath = "Shader/PrePass";
        private const string BloomShaderPath = "Shader/Bloom";
        private const string GrassShaderPath = "Shader/Grass";        

        private readonly ForwardRenderer mForwardRenderer;
        private readonly PostProcessRenderer mPostProcessRenderer;
        private readonly ShadowRenderer mShadowRenderer;
        private readonly FxaaRenderer mFxaaRenderer;
        private readonly SkyRenderer mSkyRenderer;
        private readonly PrePassRenderer mPrePassRenderer;
        private readonly BloomRenderer mBloomRenderer;
        private readonly GrassRenderer mGrassRenderer;
        private readonly BoundingRectangleRenderer mBoundingRectRenderer;
        private readonly VisibilityGraphRenderer mVisibilityGraphRenderer;

        private readonly GraphicsDevice mGraphicsDevice;

        private readonly Quad mQuad;
        private bool mPrePass;

        private bool mDisposed = false;

        /// <summary>
        /// Constructs a <see cref="MasterRenderer"/>.
        /// </summary>
        /// <param name="device">The graphics device which should already be initialized.</param>
        /// <param name="content">The content manager which should already be initialized.</param>
        public MasterRenderer(GraphicsDevice device, ContentManager content)
        {

            mGraphicsDevice = device;

            var depthStencilState = new DepthStencilState
            {
                StencilEnable = false,
                DepthBufferFunction = CompareFunction.LessEqual
            };

            mGraphicsDevice.DepthStencilState = depthStencilState;
            
            mQuad = new Quad(device);

            mForwardRenderer = new ForwardRenderer(device, content, ForwardShaderPath, ForwardSkinnedShaderPath, ForwardTerrainShaderPath);
            mPostProcessRenderer = new PostProcessRenderer(device, content, PostProcessShaderPath);
            mShadowRenderer = new ShadowRenderer(device, content, ShadowShaderPath, ShadowSkinnedShaderPath);
            mFxaaRenderer = new FxaaRenderer(device, content, FxaaShaderPath);
            mSkyRenderer = new SkyRenderer(device, content, SkyShaderPath);
            mPrePassRenderer = new PrePassRenderer(device, content, PrePassShaderPath);
            mBloomRenderer = new BloomRenderer(device, content, BloomShaderPath);
            mGrassRenderer = new GrassRenderer(device, content, GrassShaderPath);
            mBoundingRectRenderer = new BoundingRectangleRenderer(device);
            mVisibilityGraphRenderer = new VisibilityGraphRenderer(device);

        }

        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="viewport">The viewport in which the scene should be rendered</param>
        /// <param name="target">The target which can be used to be rendered on</param>
        /// <param name="camera">The camera from which perspective the scene will be rendered</param>
        /// <param name="scene">The scene to be rendered</param>
        public void Render(Viewport viewport, RenderTarget target, Camera camera, Scene scene)
        {

            var defaultViewport = mGraphicsDevice.Viewport;
            mGraphicsDevice.Viewport = target.mRenderViewport;
            var defaultRenderTarget = mGraphicsDevice.GetRenderTargets();

            mGraphicsDevice.BlendState = BlendState.Opaque;
            mGraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            var light = scene.mSky.Light;

            if (light.mShadow.mActivated)
            {
                mGraphicsDevice.SetRenderTarget(target.mShadowRenderTarget);

                mGraphicsDevice.Clear(Color.White);

                mShadowRenderer.Render(target, camera, scene);
            }

            mGraphicsDevice.SetRenderTarget(target.mMainRenderTarget);

            if (mPrePass)
            {
                mPrePassRenderer.Render(target, camera, scene);
            }

            mForwardRenderer.Render(target, camera, scene);

            mGraphicsDevice.RasterizerState = RasterizerState.CullNone;

            if (scene.mTerrain != null && scene.mTerrain.mGrass.mActivated)
            {
                mGrassRenderer.Render(target, camera, scene);
            }

            if (DebugMode)
            {
                mBoundingRectRenderer.Render(target, camera, scene);
                mVisibilityGraphRenderer.Render(target, camera, scene);
            }

            mGraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            mSkyRenderer.Render(target, camera, scene);

            mGraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            mGraphicsDevice.SetVertexBuffer(mQuad.mBuffer);

            if (scene.mPostProcessing.mBloom.Activated)
            {
                mBloomRenderer.Render(target, camera, scene);
            }

            // Check which framebuffer we need to use.
            // If we want to have Anti-Aliasing we need to bind the post-process render target
            if (scene.mPostProcessing.mFxaa.Activated)
            {
                mGraphicsDevice.SetRenderTarget(target.mPostProcessRenderTarget);
            }
            else
            {
                mGraphicsDevice.SetRenderTargets(defaultRenderTarget);

                mGraphicsDevice.Viewport = viewport;
            }

            mPostProcessRenderer.Render(target, camera, scene);

            if (scene.mPostProcessing.mFxaa.Activated)
            {
                mGraphicsDevice.SetRenderTargets(defaultRenderTarget);
                mGraphicsDevice.Viewport = viewport;

                mFxaaRenderer.Render(target, camera, scene);
            }

            mGraphicsDevice.Viewport = defaultViewport;

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
                mForwardRenderer.Dispose();
                mPostProcessRenderer.Dispose();
                mShadowRenderer.Dispose();
                mFxaaRenderer.Dispose();
                mSkyRenderer.Dispose();
                mPrePassRenderer.Dispose();
                mBloomRenderer.Dispose();
                mGrassRenderer.Dispose();
                mBoundingRectRenderer.Dispose();
                mVisibilityGraphRenderer.Dispose();
                mQuad.Dispose();
            }

            mDisposed = true;

        }

    }

}