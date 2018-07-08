
using System;

namespace LevelEditor.Engine.Renderer
{
    internal interface IRenderer : IDisposable
    {

        void Render(RenderTarget target, Camera camera, Scene scene);

    }
}
