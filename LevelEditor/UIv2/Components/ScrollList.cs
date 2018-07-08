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
    internal sealed class ScrollList: IDrawable2D, IMenuComponent
    {
        private bool mIsVisible;

        // Apperance
        public Rectangle mSize;
        private readonly Texture2D mTexture;

        private int mTotalHeight;
        private int mCurrentYPos;
        private readonly int mScrollSpeed;

        // ScrollBar
        private Rectangle mScrollBar;
        private Rectangle mScrollBarPoint;

        private readonly Texture2D mScrollBarTexture2D;
        private readonly Texture2D mScrollBarPointTexture2D;

        private List<Event> mEvents;
        
        private readonly GraphicsDevice mGraphicsDevice;
        private readonly Rectangle mRelativePosition;
        private Menu mParentMenu;
        public Menu ChildMenu { get; }

        public ScrollList(GraphicsDevice device, int xPositionInPercent, int yPositionInPercent, int widthInPercent, int heightInPercent, Texture2D texture, int scrollSpeed)
        {
            mGraphicsDevice = device;
            mRelativePosition.X = xPositionInPercent;
            mRelativePosition.Y = yPositionInPercent;
            mRelativePosition.Width = widthInPercent;
            mRelativePosition.Height = heightInPercent;

            mTexture = texture;

            mScrollSpeed = scrollSpeed;

            mIsVisible = true;
            mEvents = new List<Event>();
            ChildMenu = new Menu(mGraphicsDevice, 0, 0, 100, 100);

            mTotalHeight = 0;
            mCurrentYPos = 0;

            mScrollBar = new Rectangle();
            mScrollBarPoint = new Rectangle();
            mScrollBarTexture2D = Menu.CreateTexture2D(mGraphicsDevice, 20, 100, pixel => Color.Aqua);
            mScrollBarPointTexture2D = Menu.CreateTexture2D(mGraphicsDevice, 20, 100, pixel => Color.DarkRed);
        }

        public void Render(SpriteBatch batch)
        {
            if (!mIsVisible)
            {
                return;
            }
            // Draw Border around box
            batch.Draw(mTexture, mSize, Color.White);

            batch.End();

            // Offset all elements by mYPos
            ChildMenu.Render(batch);

            batch.Begin(depthStencilState: DepthStencilState.Default);
            batch.Draw(mScrollBarTexture2D, mScrollBar, Color.White);
            batch.Draw(mScrollBarPointTexture2D, mScrollBarPoint, Color.White);
        }

        public void CheckRegisteredEvents()
        {
            ChildMenu.CheckRegisteredEvents();

            if (!mSize.Contains(Mouse.GetState().Position))
            {
                return;
            }
            // Make the ScrollList solid
            InputManager.MouseLeftButtonDown();
            InputManager.MouseRightButtonDown();

            // Scroll
            
            var pos = Mouse.GetState().ScrollWheelValue;

            // This should not happen, but just in case!
            if (pos == mCurrentYPos)
            {
                return;
            }

            if (pos > mCurrentYPos)
            {
                if (ChildMenu.mElementList.Count > 0)
                {
                    if (mSize.Y + mSize.Height < ChildMenu.mElementList[ChildMenu.mElementList.Count - 1].GetSize().Y + ChildMenu.mElementList[ChildMenu.mElementList.Count - 1].GetSize().Height)
                    {
                        foreach (var elem in ChildMenu.mElementList)
                        {
                            // Offset
                            var size = elem.GetSize();
                            size.Offset(0, (-1) * mScrollSpeed);
                            elem.SetSize(size);
                            // Draw
                            if (elem.GetSize().Y < mSize.Y || elem.GetSize().Y + elem.GetSize().Height > mSize.Y + mSize.Height)
                            {
                                elem.SetVisibility(false);
                                continue;
                            }

                            elem.SetVisibility(true);
                        }

                        // mYPos += 1;
                    }
                }
            }
            else
            {
                if (ChildMenu.mElementList.Count > 0)
                {
                    if (mSize.Y - ChildMenu.mElementList[0].GetSize().Y > 0)
                    {
                        foreach (var elem in ChildMenu.mElementList)
                        {
                            // Offset
                            var size = elem.GetSize();
                            size.Offset(0, mScrollSpeed);
                            elem.SetSize(size);
                            // Draw
                            if (elem.GetSize().Y < mSize.Y || elem.GetSize().Y + elem.GetSize().Height > mSize.Y + mSize.Height)
                            {
                                elem.SetVisibility(false);
                                continue;
                            }

                            elem.SetVisibility(true);
                        }
                    }
                }
            }
            mCurrentYPos = pos;

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
            mSize.Width = (m.Size.Width * mRelativePosition.Width / 100) * 95 / 100;
            mSize.Height = m.Size.Height * mRelativePosition.Height / 100;

            mScrollBar.X = mSize.X + mSize.Width;
            mScrollBar.Y = mSize.Y;
            mScrollBar.Width = (m.Size.Width * mRelativePosition.Width / 100) * 5 / 100;
            mScrollBar.Height = mSize.Height;

            mScrollBarPoint.X = mScrollBar.X;
            mScrollBarPoint.Y = mScrollBar.Y + mCurrentYPos;
            mScrollBarPoint.Width = mScrollBar.Width;
            mScrollBarPoint.Height = mScrollBar.Width; // Need to figure out something better

            m.mElementList.Add(this);
            mParentMenu = m;
            mParentMenu.NonSolid();
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
            mParentMenu.NonSolid();
            s.AddIDrawable2D(this);
        }
        
        /// <summary>
        /// Do NOT use this method, it is for internal use only.
        /// </summary>
        /// <param name="d"></param>
        internal void AddIDrawable2D(IDrawable2D d)
        {
            ChildMenu.mElementList.Add(d);
        }

        public bool Contains(IDrawable2D d)
        {
            return ChildMenu.mElementList.Contains(d);
        }

        public void UpdateResolution(int width, int height)
        {
            var size = mSize;

            size.X = mRelativePosition.X * width / 100 + mParentMenu.Size.X;
            size.Y = mRelativePosition.Y * height / 100 + mParentMenu.Size.Y;
            size.Width = mRelativePosition.Width * width / 100;
            size.Height = mRelativePosition.Height * height / 100;

            mSize = size;
            ChildMenu.UpdateResolution(mSize.Width, mSize.Height);
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

        public void AddListener(MouseButtons mb, InputState inputState, Action a)
        {
        }

        public void AddListener(Keys k, InputState inputState, Action a)
        {
        }
    }
}
