using LevelEditor.Engine.Renderer;
using LevelEditor.Pathfinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Engine.Debug
{
    internal class VisibilityGraphRenderer : IRenderer
    {

        private readonly GraphicsDevice mGraphics;

        private readonly BasicEffect mEffect;
        private bool mDisposed = false;

        private VertexPosition[] mVertices;

        public VisibilityGraphRenderer(GraphicsDevice device)
        {
            mGraphics = device;

            mEffect = new BasicEffect(device)
            {
                LightingEnabled = false,
                TextureEnabled = false,
                World = Matrix.Identity
            };

            mVertices = new VertexPosition[1];

        }

        public void Render(RenderTarget target, Camera camera, Scene scene)
        {

            if (scene.mVisibilityGraph == null)
            {
                return;
            }

            mEffect.View = camera.mViewMatrix;
            mEffect.Projection = camera.mProjectionMatrix;
            mEffect.DiffuseColor = new Vector3(0.0f, 0.0f, 1.0f);

            
            var verticesCount = scene.mVisibilityGraph.mEdges.Count * 2;
            if (mVertices.Length != verticesCount)
            {
                mVertices = new VertexPosition[verticesCount];

                for (var i = 0; i < scene.mVisibilityGraph.mEdges.Count; i++)
                {
                    var edge = scene.mVisibilityGraph.mEdges[i];
                    mVertices[i * 2].Position = new Vector3(edge.V1.Position.X, 10.0f, edge.V1.Position.Y);
                    mVertices[i * 2 + 1].Position = new Vector3(edge.V2.Position.X, 10.0f, edge.V2.Position.Y);
                }
            }

            mEffect.CurrentTechnique.Passes[0].Apply();

            mGraphics.DrawUserPrimitives(PrimitiveType.LineList, mVertices, 0, scene.mVisibilityGraph.mEdges.Count);
            /*
            if (VisibilityGraph.mDrawable.Count != 0)
            {
                var vertices = new VertexPosition[VisibilityGraph.mDrawable.Count * 2];

                for (var i = 0; i < VisibilityGraph.mDrawable.Count; i++)
                {
                    var edge = VisibilityGraph.mDrawable[i];
                    vertices[i * 2].Position = new Vector3(edge.V1.Position.X, 10.0f, edge.V1.Position.Y);
                    vertices[i * 2 + 1].Position = new Vector3(edge.V2.Position.X, 10.0f, edge.V2.Position.Y);
                }

                mEffect.CurrentTechnique.Passes[0].Apply();

                mGraphics.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, VisibilityGraph.mDrawable.Count);
            }
            */
        }

        public void Dispose()
        {
            
        }

    }
}
