using System;
using System.Collections.Generic;
using LevelEditor.UI.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LevelEditor.Ui.Components
{
    internal class TextBox : ITextComponent, IDrawable2D, IImageComponent
    {
        private SpriteFont Font { get; set; }

        public readonly List<TextBox> mTextbox1;

        public string Text { get; set; }
        public Color TextColor { get; set; }
        private Vector2 Position { get; set; }

        public bool Clicked()
        {
            throw new NotImplementedException();
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

            batch.Draw(Texture, Position, Size, TextColor);
            batch.DrawString(Font, Text, Position, TextColor);
        }
        
        public Texture2D Texture { get; set; }
    }
}
