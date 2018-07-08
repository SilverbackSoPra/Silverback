using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LevelEditor.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace LevelEditor.UIv2.Components
{
    class Image: IDrawable2D, IMenuComponent
    {
        private bool IsVisible { get; set; }

        // Apperance
        private Rectangle mSize;
        private readonly Texture2D mTexture;
        
        private List<Event> mEvents;

        private GraphicsDevice mGraphicsDevice;
        private readonly Rectangle mRelativePosition;
        private Menu mParentMenu;

        public Image(GraphicsDevice graphicsDevice, int xPositionInPercent, int yPositionInPercent, int widthInPercent, int heightInPercent, Texture2D texture)
        {
            mGraphicsDevice = graphicsDevice;
            mRelativePosition.X = xPositionInPercent;
            mRelativePosition.Y = yPositionInPercent;
            mRelativePosition.Width = widthInPercent;
            mRelativePosition.Height = heightInPercent;

            mTexture = texture;
            
            IsVisible = true;
            mEvents = new List<Event>();
        }
        public void Render(SpriteBatch batch)
        {
            if (!IsVisible)
            {
                return;
            }
            batch.Draw(mTexture, mSize, Color.White);
        }

        public void UpdateResolution(int width, int height)
        {
            throw new NotImplementedException();
        }

        public Rectangle GetSize()
        {
            return mSize;
        }

        public void SetSize(Rectangle r)
        {
            mSize = r;
        }

        public void SetVisibility(bool v)
        {
            IsVisible = v;
        }

        public void AddTo(Menu m)
        {
            if (m.mElementList.Contains(this))
            {
                return;
            }
            // Adjust position
            mSize.X = m.Size.Width * mRelativePosition.X / 100 + m.Size.X;
            mSize.Y = m.Size.Height * mRelativePosition.Y / 100 + m.Size.Y;
            mSize.Width = m.Size.Width * mRelativePosition.Width / 100;
            mSize.Height = m.Size.Height * mRelativePosition.Height / 100;
            
            mParentMenu = m;
            // Add to menu
            m.mElementList.Add(this);
        }

        public void AddTo(ScrollList s)
        {
            if (s.Contains(this))
            {
                return;
            }
            // Adjust position
            mSize.X = s.mSize.Width * mRelativePosition.X / 100 + s.mSize.X;
            mSize.Y = s.mSize.Height * mRelativePosition.Y / 100 + s.mSize.Y;
            mSize.Width = s.mSize.Width * mRelativePosition.Width / 100;
            mSize.Height = s.mSize.Height * mRelativePosition.Height / 100;
            
            mParentMenu = s.ChildMenu;
            s.AddIDrawable2D(this);
        }

        public void CheckRegisteredEvents()
        { }

        public void AddListener(MouseButtons mb, InputState inputState, Action a)
        {}

        public void AddListener(Keys k, InputState inputState, Action a)
        {}
    }
}
