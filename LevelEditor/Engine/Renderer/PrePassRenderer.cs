using LevelEditor.Engine.Shader;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LevelEditor.Engine.Renderer
{
    internal sealed class PrePassRenderer : IRenderer
    {

        private readonly PrePassShader mShader;
        private readonly GraphicsDevice mGraphicsDevice;

        private bool mDisposed = false;

        public PrePassRenderer(GraphicsDevice device, ContentManager content, string shaderPath)
        {

            mShader = new PrePassShader(content, shaderPath);

            mGraphicsDevice = device;

        }

        public void Render(RenderTarget target, Camera camera, Scene scene)
        {

            mShader.mViewMatrix = camera.mViewMatrix;
            mShader.mProjectionMatrix = camera.mProjectionMatrix;
            mShader.Apply();

            foreach (var actorBatch in scene.mActorBatches)
            {

                var meshData = actorBatch.mMesh.mMeshData;

                if (meshData.mIsSkinned)
                {
                    continue;
                }

                mGraphicsDevice.SetVertexBuffer(actorBatch.mMesh.VertexBuffer);
                mGraphicsDevice.Indices = actorBatch.mMesh.IndexBuffer;

                foreach (var actor in actorBatch.mActors)
                {

                    if (!actor.mRender)
                    {
                        continue;
                    }

                    mShader.ApplyModelMatrix(actor.ModelMatrix);

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
