using System;
using System.Collections.Generic;
using LevelEditor.Events;
using LevelEditor.UI.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LevelEditor.Ui.Components
{
    sealed class ScrollList: IDrawable2D, IImageComponent, ITextComponent
    {
        private readonly List<IDrawable2D> mElementList = new List<IDrawable2D>();

        private int mScrollSpeed = 1;

        private int mCurrentYPos;
        private int mTotalSubElementHight;
        
        private int mScreenWidth = 1920;
        private int mScreenHeight = 1080;

        private GraphicsDevice mGraphicsDevice;

        public ScrollList(GraphicsDevice graphicsDevice)
        {
            mGraphicsDevice = graphicsDevice;
            Visible = true;
        }

        public void AddElement(IDrawable2D elem)
        {
            // Offet the element
            var size = elem.Size;
            size.Offset(Size.X, Size.Y);
            elem.Size = size;
            // Change the visibility
            if (elem.Size.Y < Size.Y || elem.Size.Y + elem.Size.Height > Size.Y + Size.Height)
            {
                elem.Visible = false;
            }
            else
            {
                elem.Visible = true;
            }
            mElementList.Add(elem);

            mTotalSubElementHight += elem.Size.Height;
        }

        public void RemoveElement(IDrawable2D elem)
        {
            mElementList.Remove(elem);
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
            // Draw Border around box
            batch.Draw(Texture, Size, Color.White);
            
            
            // Offset all elements by mYPos
            foreach (var elem in mElementList)
            {
                elem.Render(batch);
            }
        }
        
        public bool Clicked()
        {
            // Check if the Mouse is outside the ScrollList Element
            if (!Size.Contains(Mouse.GetState().Position))
            {
                return false;
            }

            foreach (var element in mElementList)
            {
                element.Clicked();
            }

            return Scroll();

        }

        public Rectangle Size { get; set; }

        private bool Scroll()
        {
            if (!Visible)
            {
                return false;
            }

            var pos = InputEvent.MouseState.ScrollWheelValue;

            // This should not happen, but just in case!
            if (pos == mCurrentYPos)
            {
                return true;
            }

            if (pos > mCurrentYPos)
            {
                if (mElementList.Count > 0)
                {
                    if (Size.Y + Size.Height < mElementList[mElementList.Count - 1].Size.Y + mElementList[mElementList.Count - 1].Size.Height)
                    {
                        foreach (var elem in mElementList)
                        {
                            // Offset
                            var size = elem.Size;
                            size.Offset(0, (-1) * mScrollSpeed);
                            elem.Size = size;
                            // Draw
                            if (elem.Size.Y < Size.Y || elem.Size.Y + elem.Size.Height > Size.Y + Size.Height)
                            {
                                elem.Visible = false;
                                continue;
                            }

                            elem.Visible = true;
                        }

                        // mYPos += 1;
                    }
                }
            }
            else
            {
                if (mElementList.Count > 0)
                {
                    if (Size.Y - mElementList[0].Size.Y > 0)
                    {
                        foreach (var elem in mElementList)
                        {
                            // Offset
                            var size = elem.Size;
                            size.Offset(0, mScrollSpeed);
                            elem.Size = size;
                            // Draw
                            if (elem.Size.Y < Size.Y || elem.Size.Y + elem.Size.Height > Size.Y + Size.Height)
                            {
                                elem.Visible = false;
                                continue;
                            }

                            elem.Visible = true;
                        }
                    }
                }
            }
            mCurrentYPos = pos;
            
            return true;
        }

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
            if (mScreenWidth != 0 && mScreenHeight != 0)
            {
                var size = Size;
                size.X = size.X * width / mScreenWidth;
                size.Y = size.Y * height / mScreenHeight;
                size.Width = size.Width * width / mScreenWidth;
                size.Height = size.Height * height / mScreenHeight;
                Size = size;
            }

            foreach (var elem in mElementList)
            {
                try
                {
                    elem.UpdateResolution(width, height);
                }
                catch (NotImplementedException)
                { }
            }

            mScreenWidth = width;
            mScreenHeight = height;
        }

        public void AddListener(ButtonState s, SbButtonType t, Func<bool> on)
        {
            throw new NotImplementedException();
        }
        
        public string Text { get; set; }
        public Color TextColor { get; set; }
        public Texture2D Texture { get; set; }
    }
}
