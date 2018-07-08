using System;
using LevelEditor.Ui;
using LevelEditor.Ui.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LevelEditor.UI.Components
{
    internal sealed class Image: IDrawable2D, IImageComponent
    {
        public Image()
        {
            Visible = true;
        }

        public void SetRelativePosition(float windowWidth, float windowHeight, float xperc, float yperc, float elemWidth, float elemHeigth)
        {
            Size = Menu.RelativeToAbsolutePosition(windowWidth, windowHeight, xperc, yperc, elemWidth, elemHeigth);
        }

        public void Render(SpriteBatch batch)
        {
            if (!Visible)
            {
                return;
            }
            // Console.WriteLine("Drawing image at position " + Size.ToString());
            // Draw the pic
            batch.Draw(Texture, Size, Color.White);
        }

        public bool Clicked()
        {
            return false;
        }

        public Rectangle Size { get; set; }
        
        public bool Visible { get; set; }
        public void SetWindowWidth(int width)
        {
            throw new NotImplementedException();
        }

        public void SetWindowHeight(int height)
        {
            throw new NotImplementedException();
        }

        public void UpdateResolution(int width, int height)
        {
        }

        public void AddListener(ButtonState s, SbButtonType t, Func<bool> on)
        {
            throw new NotImplementedException();
        }
        
        public Texture2D Texture { get; set; }
    }
}
