using LevelEditor.Engine.Shader;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using LevelEditor.Engine.Helper;
using Microsoft.Xna.Framework;

namespace LevelEditor.Engine.Renderer
{
    internal sealed class GrassRenderer : IRenderer
    {

        private readonly GrassShader mShader;

        private readonly GraphicsDevice mDevice;

        private readonly Mesh.Mesh mGrassMesh;
        private readonly Texture2D mGrassTexture;

        private readonly VertexBufferBinding[] mBufferBindings;
        private VertexBuffer mGrassInstancingBuffer;

        private List<Grass.Patch> mVisiblePatches;
        private GrassInstancing[] mVisibleInstances;

        private bool mDisposed = false;

        private static readonly SamplerState sShadowMap = new SamplerState
        {
            AddressU = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Clamp,
            AddressW = TextureAddressMode.Clamp,
            Filter = TextureFilter.Linear,
            ComparisonFunction = CompareFunction.LessEqual,
            FilterMode = TextureFilterMode.Comparison
        };

        public GrassRenderer(GraphicsDevice device, ContentManager content, string shaderPath)
        {

            mShader = new GrassShader(content, shaderPath);

            var model = content.Load<Model>("Grass/Grass");
            mGrassMesh = new Mesh.Mesh(model);

            mGrassTexture = content.Load<Texture2D>("Grass/GrassTexture");

            mDevice = device;
            mVisiblePatches = new List<Grass.Patch>();

            mBufferBindings = new VertexBufferBinding[2];
            mBufferBindings[0] = new VertexBufferBinding(mGrassMesh.VertexBuffer);
            mGrassInstancingBuffer = new VertexBuffer(mDevice, GrassInstancing.VertexDeclaration, 1, BufferUsage.WriteOnly);

        }

        public void Render(RenderTarget target, Camera camera, Scene scene)
        {

            if (scene.mTerrain == null)
            {
                return;
            }

            var light = scene.mSky.Light;

            mDevice.SamplerStates[1] = sShadowMap;

            if (mGrassInstancingBuffer.VertexCount != scene.mTerrain.mGrass.mGrassInstances.Length)
            {
                mGrassInstancingBuffer.Dispose();
                mGrassInstancingBuffer = new VertexBuffer(mDevice,
                    GrassInstancing.VertexDeclaration,
                    scene.mTerrain.mGrass.mGrassInstances.Length,
                    BufferUsage.WriteOnly);
                mBufferBindings[1] = new VertexBufferBinding(mGrassInstancingBuffer, 0, 1);
                mVisibleInstances = new GrassInstancing[scene.mTerrain.mGrass.mGrassInstances.Length];
            }

            mVisiblePatches.Clear();

            foreach (var patch in scene.mTerrain.mGrass.mPatches)
            {
                if (patch.mRender)
                {
                    mVisiblePatches.Add(patch);
                }
            }

            if (mVisiblePatches.Count == 0)
            {
                return;
            }

            mVisiblePatches = mVisiblePatches.OrderBy(ele => ele.mDistance).ToList();

            var index = 0;

            foreach (var patch in mVisiblePatches)
            {
                for (var i = patch.mPatchInstancesOffset;
                    i < patch.mPatchInstancesOffset + Grass.GrassPatchInstancesCount;
                    i++)
                {
                    mVisibleInstances[index++] = scene.mTerrain.mGrass.mGrassInstances[i];
                }
            }

            var visibleInstancesCount = mVisiblePatches.Count * Grass.GrassPatchInstancesCount;

            mGrassInstancingBuffer.SetData(mVisibleInstances, 0, visibleInstancesCount);

            mDevice.Indices = mGrassMesh.IndexBuffer;
            mDevice.SetVertexBuffers(mBufferBindings);            

            mShader.mViewMatrix = camera.mViewMatrix;
            mShader.mProjectionMatrix = camera.mProjectionMatrix;
            mShader.mFarPlane = camera.mFarPlane;

            mShader.mGrassTexture = mGrassTexture;

            mShader.mFogColor = scene.mFog.mColor;
            mShader.mFogDistance = Math.Min(scene.mFog.mDistance, camera.mFarPlane - 10.0f);

            // We calculate the light on the CPU because the normal is always upfacing, which means the diffuse color will always be the same
            var grassNormal = Vector3.Normalize(Vector3.TransformNormal(Vector3.Up, camera.mViewMatrix));
            var lightDirection = Vector3.Normalize(Vector3.Transform(light.mLocation, camera.mViewMatrix) - Vector3.Transform(Vector3.Zero, camera.mViewMatrix));
            var lightDiffuseColor = Vector3.Dot(grassNormal, lightDirection) * light.mColor;
            mShader.mGlobalLightDiffuseColor = lightDiffuseColor;
            mShader.mGlobalLightAmbient = light.mAmbient;

            if (light.mShadow.mActivated)
            {
                mShader.mLightSpaceMatrix = Matrix.Invert(camera.mViewMatrix) * light.mShadow.mViewMatrix *
                                           light.mShadow.mProjectionMatrix;
                mShader.mShadowMap = target.mShadowRenderTarget;
                mShader.mShadowBias = light.mShadow.mBias;
                mShader.mShadowDistance = light.mShadow.mDistance;
            }

            mShader.mTime = (float)Statistic.Time / 1000.0f;

            mShader.Apply();

            mDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, mGrassMesh.mMeshData.mTotalNumPrimitives, visibleInstancesCount);
            
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
                mGrassMesh.Dispose();
                mGrassTexture.Dispose();
            }

            mDisposed = true;

        }

    }

}
