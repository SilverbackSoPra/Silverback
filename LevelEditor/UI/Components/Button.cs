using System;
using System.Collections.Generic;
using LevelEditor.Events;
using LevelEditor.UI.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LevelEditor.Ui.Components
{
    internal sealed class Button : ITextComponent, IImageComponent, IDrawable2D
    {
        private struct OnHolder
        {
            public SbButtonType mType;
            public ButtonState mState;
            public Func<bool> mFunc;
            public readonly List<Button> mButton1;
        }

        private Rectangle mSize;
        private Vector2 mPosition;
        private ButtonState mLastStateLeftClick;
        private ButtonState mLastStateRightClick;
        private bool mMouseInside;

        private readonly List<OnHolder> mFuncs = new List<OnHolder>();

        private int ScreenWidth { get; set; }
        private int ScreenHeight { get; set; }

        public Button()
        {
            mLastStateLeftClick = ButtonState.Released;
            mLastStateRightClick = ButtonState.Released;
            Visible = true;
            mMouseInside = false;

        }

        private SpriteFont mFont;

        public Texture2D Texture { get; set; }

        public string Text { get; set; }
        public Color TextColor { get; set; }

        public Rectangle Size
        {
            get { return mSize; }
            set
            {
                mSize = value;
                mPosition.X = mSize.X + mSize.Width / 2.0f - mFont.MeasureString(Text).X / 2.0f;
                mPosition.Y = mSize.Y + mSize.Height / 2.0f - mFont.MeasureString(Text).Y / 2.0f;

                UpdateResolution(ScreenWidth, ScreenHeight);
            }
        }

        public bool Visible { get; set; }

        public void SetWindowWidth(int width)
        {
            // ScreenWidth = width;
            UpdateResolution(width, ScreenHeight);
        }

        public void SetWindowHeight(int height)
        {
            // ScreenHeight = height;
            UpdateResolution(ScreenWidth, height);
        }

        public void UpdateResolution(int width, int height)
        {
            if (ScreenWidth != 0 && ScreenHeight != 0)
            {
                var size = mSize;
                size.X = size.X * width / ScreenWidth;
                size.Y = size.Y * height / ScreenHeight;
                size.Width = size.Width * width / ScreenWidth;
                size.Height = size.Height * height / ScreenHeight;
                mPosition.X = size.X + size.Width / 2.0f - mFont.MeasureString(Text).X / 2.0f;
                mPosition.Y = size.Y + size.Height / 2.0f - mFont.MeasureString(Text).Y / 2.0f;
                mSize = size;
            }
            ScreenWidth = width;
            ScreenHeight = height;
        }

        public void AddListener(ButtonState s, SbButtonType t, Func<bool> f)
        {
            var holder = new OnHolder
            {
                mType = t,
                mState = s,
                mFunc = f
            };
            mFuncs.Add(holder);
        }

        public bool Clicked()
        {
            if (!Size.Contains(Mouse.GetState().Position))
            {
                mMouseInside = false;
                return false;
            }

            mMouseInside = true;

            foreach (var h in mFuncs)
            {
                if (!(h.mType == SbButtonType.RightClick && h.mState == ButtonState.Pressed && InputManager.MouseRightButtonPressed()) &&
                    !(h.mType == SbButtonType.RightClick && h.mState == ButtonState.Released && InputManager.MouseRightButtonReleased()) &&
                    !(h.mType == SbButtonType.LeftClick && h.mState == ButtonState.Pressed && InputManager.MouseLeftButtonPressed()) &&
                    !(h.mType == SbButtonType.LeftClick && h.mState == ButtonState.Released && InputManager.MouseLeftButtonReleased()))
                {
                    continue;
                }

                if (h.mFunc())
                {
                    return true;
                }
            }

            // if (e.Type != InputType.Mouse)
            // {
            //     return false;
            // }
            // 
            // var m = InputEvent.MouseState;
            // // Check if the Mouse coordinates are outside the menu element
            // if (mSize.X > m.Position.X)
            // {
            //     mMouseInside = false;
            //     return false;
            // }
            // if (mSize.X + mSize.Width < m.Position.X)
            // {
            //     mMouseInside = false;
            //     return false;
            // }
            // if (mSize.Y > m.Position.Y)
            // {
            //     mMouseInside = false;
            //     return false;
            // }
            // if (mSize.Y + mSize.Height < m.Position.Y)
            // {
            //     mMouseInside = false;
            //     return false;
            // }
            // 
            // mMouseInside = true;
            // foreach (var h in mFuncs)
            // {
            //     // Check if the conditions match the required ones
            //     if ((m.RightButton != h.mState || h.mType != SbButtonType.RightClick) &&
            //         (m.LeftButton != h.mState || h.mType != SbButtonType.LeftClick))
            //     {
            //         continue;
            //     }
            // 
            //     // Check if the requiered click is a left click
            //     if (h.mType == SbButtonType.LeftClick)
            //     {
            //         // Check if the required action is pressed
            //         if (h.mState == ButtonState.Pressed && !InputManager.MouseLeftButtonPressed())
            //         {
            //             continue;
            //         }
            //         // Check if the required action is relesed
            //         if (h.mState == ButtonState.Released && !InputManager.MouseLeftButtonReleased())
            //         {
            //             continue;
            //         }
            //     }
            // 
            //     // Check if the requiered click is a right click
            //     if (h.mType == SbButtonType.RightClick)
            //     {
            //         // Check if the required action is pressed
            //         if (h.mState == ButtonState.Pressed && !InputManager.MouseRightButtonPressed())
            //         {
            //             continue;
            //         }
            //         // Check if the required action is relesed
            //         if (h.mState == ButtonState.Released && !InputManager.MouseRightButtonReleased())
            //         {
            //             continue;
            //         }
            //     }
            // 
            //     if (h.mFunc())
            //     {
            //         Console.WriteLine("Clicked on " + Text);
            //         return true;
            //     }
            //     Console.WriteLine("Clicked on " + Text);
            // }
            // 
            // mLastStateLeftClick = m.LeftButton;
            // mLastStateRightClick = m.RightButton;

            return true;
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

            if (mMouseInside)
            {
                batch.Draw(Texture, mSize, Color.White);
            }
            else
            {
                batch.Draw(Texture, mSize, Color.White * .5f);
            }
            batch.DrawString(mFont, Text, mPosition, TextColor);

        }
    }

}
