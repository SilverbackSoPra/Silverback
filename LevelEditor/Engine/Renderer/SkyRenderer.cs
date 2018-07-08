using LevelEditor.Engine.Shader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LevelEditor.Engine.Renderer
{
    sealed class SkyRenderer : IRenderer
    {

        // The path to the other resources
        private const string SkyDomePath = "Sky/skydome";
        private const string TintPath = "Sky/firstTint";
        private const string SunMorphPath = "Sky/sun";
        private const string MoonPath = "Sky/moon";

        private readonly SkyShader mShader;

        private readonly GraphicsDevice mDevice;
        private readonly Mesh.Mesh mSkyDome;

        private readonly Texture2D mTint;
        private readonly Texture2D mSunMorph;
        private readonly Texture2D mMoon;

        private bool mDisposed = false;

        public SkyRenderer(GraphicsDevice device, ContentManager content, string shaderPath)
        {

            mDevice = device;

            mShader = new SkyShader(content, shaderPath);

            var model = content.Load<Model>(SkyDomePath);
            mSkyDome = new Mesh.Mesh(model);

            mTint = content.Load<Texture2D>(TintPath);
            mSunMorph = content.Load<Texture2D>(SunMorphPath);
            mMoon = content.Load<Texture2D>(MoonPath);

        }

        public void Render(RenderTarget target, Camera camera, Scene scene)
        {

            var light = scene.mSky.Light;

            var cameraLocation = camera.mThirdPerson ? camera.mLocation - camera.Direction * camera.mThirdPersonDistance : camera.mLocation;
            var modelMatrix = Matrix.CreateScale(camera.mFarPlane) * Matrix.CreateTranslation(cameraLocation);
            mShader.mMvpMatrix = modelMatrix * camera.mViewMatrix * camera.mProjectionMatrix;

            mShader.mSunLocation = scene.mSky.SunLocation;
            mShader.mFogColor = scene.mFog.mColor;

            mShader.mTint = mTint;
            mShader.mSunMorph = mSunMorph;
            mShader.mMoon = mMoon;

            mShader.Apply();

            mDevice.SetVertexBuffer(mSkyDome.VertexBuffer);
            mDevice.Indices = (mSkyDome.IndexBuffer);

            mDevice.DrawIndexedPrimitives(mSkyDome.mMeshData.mPrimitiveType,
                0,
                0,
                mSkyDome.mMeshData.mTotalNumPrimitives);

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
                mSkyDome.Dispose();
                mTint.Dispose();
                mSunMorph.Dispose();
                mMoon.Dispose();
            }

            mDisposed = true;

        }

    }

}
