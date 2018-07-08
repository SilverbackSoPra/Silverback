using LevelEditor.Engine.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LevelEditor.Engine.Debug
{
    internal sealed class BoundingRectangleRenderer : IRenderer
    {

        private readonly GraphicsDevice mGraphics;
        private readonly VertexPosition[] mVertices = new VertexPosition[4];
        private readonly int[] mIndices = { 1, 3, 0, 0, 3, 2};

        private readonly IndexBuffer mIndexBuffer;
        private readonly VertexBuffer mVertexBuffer;

        private readonly BasicEffect mEffect;
        private bool mDisposed = false;

        public BoundingRectangleRenderer(GraphicsDevice device)
        {
            mGraphics = device;

            mIndexBuffer = new IndexBuffer(device, typeof(int), mIndices.Length, BufferUsage.WriteOnly);
            mIndexBuffer.SetData(mIndices);
            mVertexBuffer = new VertexBuffer(device, VertexPosition.VertexDeclaration, mVertices.Length, BufferUsage.WriteOnly);
            mVertexBuffer.SetData(mVertices);

            mEffect = new BasicEffect(device)
            {
                LightingEnabled = false,
                TextureEnabled = false,
                World = Matrix.Identity
            };

        }

        public void Render(RenderTarget target, Camera camera, Scene scene)
        {

            mEffect.View = camera.mViewMatrix;
            mEffect.Projection = camera.mProjectionMatrix;                      

            foreach (var actorBatch in scene.mActorBatches)
            {

                foreach (var actor in actorBatch.mActors)
                {

                    foreach (var pass in mEffect.CurrentTechnique.Passes)
                    {

                        mEffect.DiffuseColor = actor.QuadTree ? new Vector3(1.0f, 0.0f, 0.0f) : new Vector3(1.0f);
                        mEffect.DiffuseColor = actor.Collision ? new Vector3(0.0f, 0.0f, 1.0f) : mEffect.DiffuseColor;
                        pass.Apply();

                        if (!actor.mRender)
                        {
                            continue;
                        }

                        var y = actor.ModelMatrix.Translation.Y;
                        
                        mVertices[0].Position = new Vector3(actor.mBoundingRectangle.V1Transformed.X, 10.0f, actor.mBoundingRectangle.V1Transformed.Y);
                        mVertices[1].Position = new Vector3(actor.mBoundingRectangle.V2Transformed.X, 10.0f, actor.mBoundingRectangle.V2Transformed.Y);
                        mVertices[2].Position = new Vector3(actor.mBoundingRectangle.V3Transformed.X, 10.0f, actor.mBoundingRectangle.V3Transformed.Y);
                        mVertices[3].Position = new Vector3(actor.mBoundingRectangle.V4Transformed.X, 10.0f, actor.mBoundingRectangle.V4Transformed.Y);  
                        
                        /*
                        var rect = actor.mBoundingRectangle.BoundingRect(1);
                        
                        mVertices[0].Position = new Vector3(rect.X, y+2.0f, rect.Y);
                        mVertices[1].Position = new Vector3(rect.X + rect.Width, y+2.0f, rect.Y);
                        mVertices[2].Position = new Vector3(rect.X, y+2.0f, rect.Y + rect.Height);
                        mVertices[3].Position = new Vector3(rect.X + rect.Width, y+2.0f, rect.Y + rect.Height);
                        */
                        

                        mVertexBuffer.SetData(mVertices);
                        mGraphics.SetVertexBuffer(mVertexBuffer);
                        mGraphics.Indices = mIndexBuffer; 

                        mGraphics.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                            0,
                            0,
                            2);

                    }

                }

            }


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
