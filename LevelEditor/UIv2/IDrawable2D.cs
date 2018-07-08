using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.UIv2
{
    internal interface IDrawable2D
    {
        void Render(SpriteBatch batch);

        void CheckRegisteredEvents();

        void UpdateResolution(int width, int height);

        Rectangle GetSize();
        void SetSize(Rectangle r);

        void SetVisibility(bool v);
    }
}
