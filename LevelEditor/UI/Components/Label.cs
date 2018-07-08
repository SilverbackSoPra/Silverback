using System;
using LevelEditor.UI.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LevelEditor.Ui.Components
{
    internal sealed class Label: ITextComponent, IDrawable2D
    {
        public string Text { get; set; }
        private SpriteFont Font { get; set; }
        public Color TextColor { get; set; }

        private Vector2 Position { get; set; }
        // public Rectangle Size { get; set; }
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
            batch.DrawString(Font, Text, Position, TextColor);
        }

        public bool Clicked()
        {
            return false;
        }

        public Rectangle Size
        {
            get { return new Rectangle(Position.ToPoint(), Position.ToPoint()); }
            set
            {
                var pos = Position;
                pos.X = value.X;
                pos.Y = value.Y;
                Position = pos;
            }
        }
    }
}
