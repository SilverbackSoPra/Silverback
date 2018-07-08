using LevelEditor.Engine.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Engine.Debug
{
    class VisibilityGraphRenderer : IRenderer
    {

        private readonly GraphicsDevice mGraphics;

        private readonly BasicEffect mEffect;
        private bool mDisposed = false;

        public VisibilityGraphRenderer(GraphicsDevice device)
        {
            mGraphics = device;

            mEffect = new BasicEffect(device)
            {
                LightingEnabled = false,
                TextureEnabled = false,
                World = Matrix.Identity
            };

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

            var vertices = new VertexPosition[scene.mVisibilityGraph.mEdges.Count * 2];

            for (var i = 0; i < scene.mVisibilityGraph.mEdges.Count; i++)
            {
                var edge = scene.mVisibilityGraph.mEdges[i];
                vertices[i * 2].Position = new Vector3(edge.V1.Position.X, 10.0f, edge.V1.Position.Y);
                vertices[i * 2 + 1].Position = new Vector3(edge.V2.Position.X, 10.0f, edge.V2.Position.Y);
            }

            mEffect.CurrentTechnique.Passes[0].Apply();

            mGraphics.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, scene.mVisibilityGraph.mEdges.Count);

        }

        public void Dispose()
        {
            
        }

    }
}
