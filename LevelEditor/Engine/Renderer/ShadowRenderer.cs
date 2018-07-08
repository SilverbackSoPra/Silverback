using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LevelEditor.Engine.Shader;
using System;

namespace LevelEditor.Engine.Renderer
{
    internal sealed class ShadowRenderer : IRenderer
    {

        private readonly ShadowShader mShader;
        private readonly ShadowShader mShaderSkinned;
        private readonly GraphicsDevice mGraphicsDevice;

        private bool mDisposed = false;

        public ShadowRenderer(GraphicsDevice device, ContentManager content, string shaderPath, string shaderPathSkinned)
        {

            mShader = new ShadowShader(content, shaderPath);
            mShaderSkinned = new ShadowShader(content, shaderPathSkinned);

            mGraphicsDevice = device;

        }

        public void Render(RenderTarget target, Camera camera, Scene scene)
        {

           Render(target, camera, scene, mShader, false);
           Render(target, camera, scene, mShaderSkinned, true);

        }

        private void Render(RenderTarget target, Camera camera, Scene scene, ShadowShader shader, bool skinnedRendering)
        {

            var light = scene.mSky.Light;

            shader.mLightSpaceMatrix = light.mShadow.mViewMatrix * light.mShadow.mProjectionMatrix;

            shader.Apply();

            // Now render our actor in batched mode
            foreach (var actorBatch in scene.mActorBatches)
            {

                var meshData = actorBatch.mMesh.mMeshData;

                // When using the unskinned shader we don't want to render skinned meshes
                if (meshData.mIsSkinned && !skinnedRendering)
                {
                    continue;
                }

                // When using the skinned shader we don't want to render unskinned meshes
                if (!meshData.mIsSkinned && skinnedRendering)
                {
                    continue;
                }

                mGraphicsDevice.SetVertexBuffer(actorBatch.mMesh.VertexBuffer);
                mGraphicsDevice.Indices = (actorBatch.mMesh.IndexBuffer);

                // When we render shadows we dont care about materials so we
                // can just iterate of the number of actors instead of using
                // another loop to consider all the materials
                foreach (var actor in actorBatch.mActors)
                {
                    if (!actor.mCastShadow)
                    {
                        continue;
                    }

                    shader.ApplyModelMatrix(actor.ModelMatrix);

                    if (skinnedRendering)
                    {
                        shader.ApplyBoneTransformations(actor.mAnimator.mTransformations);
                    }

                    mGraphicsDevice.DrawIndexedPrimitives(meshData.mPrimitiveType,
                        0,
                        0,
                        meshData.mTotalNumPrimitives);

                }

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
