using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LevelEditor.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace LevelEditor.UIv2.Components
{
    /// <inheritdoc>
    ///     <cref></cref>
    /// </inheritdoc>
    internal sealed class Slider: IDrawable2D, IMenuComponent
    {
        private bool mIsVisible;

        private readonly int mRangeStart;
        private readonly int mRangeEnd;

        private float RangeValue
        {
            get
            {
                var val = 0.0f;
                // Get the amount of possible values
                // E.g. 0 to 100 --> 101 possible values (100 - 0 + 1)
                val = mRangeEnd - mRangeStart;
                // Get the x value per rangepoint
                val = (mSize.Width - mSizePoint.Width) / val;
                // 
                return (mSizePoint.X - mSize.X) / val + mRangeStart;
            }
            set
            {
                mSizePoint.X = (int)value; // * (mSize.Width / (mRangeEnd - mRangeStart));
                foreach (var fn in mChangeActions)
                {
                    fn(RangeValue);
                }
            }
        }

        private readonly List<Action<float>> mChangeActions;
        
        // Apperance
        private Rectangle mSize;
        private readonly Texture2D mTexture;
        
        // Apperance Point
        private Rectangle mSizePoint;
        private readonly Texture2D mTextureSliderPloint;
        
        private List<Event> mEvents;

        private GraphicsDevice mGraphicsDevice;
        private readonly Rectangle mRelativePosition;
        private Menu mParentMenu;

        public Slider(GraphicsDevice device,
            Menu m,
            int xPositionInPercent,
            int yPositionInPercent,
            int widthInPercent,
            int heightInPercent,
            Texture2D texture,
            Texture2D textureSliderPloint,
            int rangeStart, int rangeEnd, int rangeValue)
        {
            mGraphicsDevice = device;
            mRelativePosition.X = xPositionInPercent;
            mRelativePosition.Y = yPositionInPercent;
            mRelativePosition.Width = widthInPercent;
            mRelativePosition.Height = heightInPercent;
            
            mEvents = new List<Event>();
            mChangeActions = new List<Action<float>>();

            mTexture = texture;
            mTextureSliderPloint = textureSliderPloint;
            
            mIsVisible = true;

            mRangeStart = rangeStart;
            mRangeEnd = rangeEnd;

            AddTo(m);

            SetValue(rangeValue);
            // RangeValue = mSize.X + rangeValue * (mSize.Width / (mRangeEnd - mRangeStart));
        }

        public Slider(GraphicsDevice device,
            Menu m,
            int xPositionInPercent,
            int yPositionInPercent,
            int widthInPercent,
            int heightInPercent,
            Texture2D texture,
            Texture2D textureSliderPloint,
            int rangeStart, int rangeEnd, float rangeValue)
        {
            mGraphicsDevice = device;
            mRelativePosition.X = xPositionInPercent;
            mRelativePosition.Y = yPositionInPercent;
            mRelativePosition.Width = widthInPercent;
            mRelativePosition.Height = heightInPercent;
            
            mEvents = new List<Event>();
            mChangeActions = new List<Action<float>>();

            mTexture = texture;
            mTextureSliderPloint = textureSliderPloint;
            
            mIsVisible = true;

            mRangeStart = rangeStart;
            mRangeEnd = rangeEnd;

            AddTo(m);

            SetValue(rangeValue);
        }

        public Slider(GraphicsDevice device,
            ScrollList s,
            int xPositionInPercent,
            int yPositionInPercent,
            int widthInPercent,
            int heightInPercent,
            Texture2D texture,
            Texture2D textureSliderPloint,
            int rangeStart, int rangeEnd, float rangeValue)
        {
            mGraphicsDevice = device;
            mRelativePosition.X = xPositionInPercent;
            mRelativePosition.Y = yPositionInPercent;
            mRelativePosition.Width = widthInPercent;
            mRelativePosition.Height = heightInPercent;
            
            mEvents = new List<Event>();
            mChangeActions = new List<Action<float>>();

            mTexture = texture;
            mTextureSliderPloint = textureSliderPloint;
            
            mIsVisible = true;

            mRangeStart = rangeStart;
            mRangeEnd = rangeEnd;

            AddTo(s);

            SetValue(rangeValue);
        }

        public void Render(SpriteBatch batch)
        {
            if (!mIsVisible)
            {
                return;
            }

            if (mSize.Contains(Mouse.GetState().Position))
            {
                batch.Draw(mTexture, mSize, Color.White);
            }
            else
            {
                batch.Draw(mTexture, mSize, Color.White * .5f);
            }

            batch.Draw(mTextureSliderPloint, mSizePoint, Color.White);
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

            mSizePoint.X = mSize.X;
            mSizePoint.Y = mSize.Y;
            mSizePoint.Width = mSize.Height*3/4;
            mSizePoint.Height = mSize.Height * 3 / 4;

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

        public void UpdateResolution(int width, int height)
        {
            if (mParentMenu == null)
            {
                return;
            }
            // Update the the buttons position
            var size = mSize;

            size.X = mRelativePosition.X * width / 100 + mParentMenu.Size.X;
            size.Y = mRelativePosition.Y * height / 100 + mParentMenu.Size.Y;
            size.Width = mRelativePosition.Width * width / 100;
            size.Height = mRelativePosition.Height * height / 100;

            mSize = size;

            mSizePoint.X = mSize.X + (int)Math.Round((RangeValue - mRangeStart) * ((mSize.Width - mSizePoint.Width) / (float)(mRangeEnd - mRangeStart)));
            mSizePoint.Y = mSize.Y + mSize.Height* 1 / 8;
            mSizePoint.Width = mSize.Height * 3 / 4;
            mSizePoint.Height = mSize.Height * 3 / 4;

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
            mIsVisible = v;
        }

        public void CheckRegisteredEvents()
        {
            // Check if the mouse is inside
            if (mSize.X + mSize.Width < mSizePoint.X + mSizePoint.Width)
            {
                RangeValue = mSize.X + mSize.Width - mSizePoint.Width;
                return;
            }
            if (mSize.X > mSizePoint.X)
            {
                RangeValue = mSize.X;
                return;
            }

            if (!mSize.Contains(Mouse.GetState().Position))
            {
                return;
            }

            if (InputManager.MouseLeftButtonPressed() || InputManager.MouseLeftButtonDown())
            {
            }
            else if (InputManager.MouseLeftButtonReleased())
            {
            }
            else
            {
                return;
            }

            // mSizePoint.X = Mouse.GetState().Position.X;
            var mousePosition = Mouse.GetState().Position.X;
            var max = mSize.X + mSize.Width - mSizePoint.Width;
            RangeValue = mousePosition > max ? max : mousePosition;
        }

        public void AddListener(MouseButtons mb, InputState inputState, Action a)
        {
        }

        public void AddListener(Keys k, InputState inputState, Action a)
        {
        }
        public void AddOnChangeListeners(params Action<float>[] fn)
        {
            mChangeActions.AddRange(fn);
            SetValue(RangeValue);
        }
        public void RemoveOnChangeListener(Action<float> fn)
        {
            mChangeActions.Remove(fn);
        }

        private void SetValue(float value)
        {
            RangeValue = mSize.X + (value - mRangeStart) * ((mSize.Width - mSizePoint.Width) / (float)(mRangeEnd - mRangeStart));
        }
    }
}
