using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LevelEditor.UI.Components
{
    internal interface IDrawable2D
    {

        void SetRelativePosition(float windowWidth,
            float windowHeight,
            float xperc,
            float yperc,
            float elemWidthPerc,
            float elemHeigthPerc);
        void Render(SpriteBatch batch);
        bool Clicked();
        Rectangle Size { get; set; }
        bool Visible{ get; set;}

        void SetWindowWidth(int width);
        void SetWindowHeight(int height);

        void UpdateResolution(int width, int height);
        void AddListener(ButtonState s, SbButtonType t, Func<bool> on);
    }
}
