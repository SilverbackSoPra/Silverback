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
    public sealed class Inputbox: IDrawable2D, IMenuComponent
    {
        private bool mIsVisible;
        private string mText;

        private List<string> mTextList;
        private List<Vector2> mTextSizeList;

        private Texture2D mTexture;

        public int mCursorPos;

        public string Text
        {
            get
            {
                return mText;
            }
            set
            {
                mText = value;

                mTextList.Clear();
                mTextList = new List<string>();
                mTextSizeList.Clear();
                mTextSizeList = new List<Vector2>();

                mTextList.Add(value);
                mTextSizeList.Add(new Vector2());
                ResizeTextLines();
                return;
            }
        }
        

        private Rectangle mSize;

        // Text
        private Vector2 mTextSize;
        public Color mTextColor;
        private SpriteFont mFont;

        private GraphicsDevice mGraphicsDevice;
        private readonly Rectangle mRelativePosition;
        private Menu mParentMenu;

        public FontManager.FontType mFontType;

        public FontManager.FontType FontType
        {
            get { return mFontType; }
            set
            {
                if (value != mFontType)
                {
                    mFont = FontManager.Get(value, mGraphicsDevice.Viewport.Width, mGraphicsDevice.Viewport.Height);
                }
                mFontType = value;

            }
        }

        private int mWindowWidth;
        private int mWindowHeight;

        private Texture2D mDebugTexture2D;

        public Inputbox(GraphicsDevice device, int xPositionInPercent, int yPositionInPercent, int widthInPercent, int heightInPercent, Texture2D texture, string text, SpriteFont font, Color textColor)
        {
            mGraphicsDevice = device;
            mRelativePosition.X = xPositionInPercent;
            mRelativePosition.Y = yPositionInPercent;
            mRelativePosition.Width = widthInPercent;
            mRelativePosition.Height = heightInPercent;

            mTextList = new List<string>();
            mTextSizeList = new List<Vector2>();

            mFont = font;
            mTextColor = textColor;
            Text = text;

            mTexture = texture;

            mIsVisible = true;
            mCursorPos = text.Length;

            mDebugTexture2D = Menu.CreateTexture2D(mGraphicsDevice, 200, 100, pixel => Color.Aqua);
            FontType = FontManager.FontType.Default;
            mWindowWidth = mGraphicsDevice.Viewport.Width;
            mWindowHeight = mGraphicsDevice.Viewport.Height;
        }

        public void Render(SpriteBatch batch)
        {
            if (!mIsVisible)
            {
                return;
            }
            if (InputManager.GetInputboxTarget() == this)
            {
                batch.Draw(mTexture, mSize, Color.White);
            }
            else
            {
                batch.Draw(mTexture, mSize, Color.White * .2f);
            }

            for (var i = 0; i < mTextList.Count && i < mTextSizeList.Count; i++)
            {
                batch.DrawString(mFont, mTextList[i], mTextSizeList[i], mTextColor);
            }
            
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

            // Adjust the text position
            Text = mText;

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

            Text = mText;

            mParentMenu = s.ChildMenu;
            s.AddIDrawable2D(this);
        }


        public void UpdateResolution(int width, int height)
        {
            if (mParentMenu == null)
            {
                return;
            }
            // Adjust position
            var size = mSize;

            size.X = mRelativePosition.X * width / 100 + mParentMenu.Size.X;
            size.Y = mRelativePosition.Y * height / 100 + mParentMenu.Size.Y;
            size.Width = mRelativePosition.Width * width / 100;
            size.Height = mRelativePosition.Height * height / 100;

            mSize = size;

            if (width != mWindowWidth || height != mWindowHeight)
            {
                mFont = FontManager.Get(mFontType, mGraphicsDevice.Viewport.Width, mGraphicsDevice.Viewport.Height);
            }

            mWindowWidth = width;
            mWindowHeight = height;

            // Update the text size
            Text = mText;
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
            var isBlocked = InputManager.IsBlocked(MouseButtons.Left);
            var peak = InputManager.MouseLeftButtonPressedPeak();
            if (!mSize.Contains(Mouse.GetState().Position) && (isBlocked || peak))
            {
                InputManager.UnsetInputboxTarget(this);
                return;
            }

            if (InputManager.MouseLeftButtonPressed())
            {
                InputManager.SetInputboxTarget(this);
            }

            // All code modifying the text is in Events/IntputManager.cs
        }
        
        public void AddListener(MouseButtons mb, InputState inputState, Action a)
        {
        }

        public void AddListener(Keys k, InputState inputState, Action a)
        {
        }

        /////////////////////
        // Private Methods //
        /////////////////////

        private void ResizeTextLines()
        {
            if (mTextList.Count > 1)
            {
                for (var i = 0; i < mTextList.Count && i < mTextSizeList.Count; i++)
                {
                    // var y = mSize.Y + mSize.Height / mTextSizeList.Count * (i + 1) - mFont.MeasureString(Text).Y / 2.0f;
                    mTextSizeList[i] = new Vector2(mSize.X + mSize.Width / 2.0f - mFont.MeasureString(mTextList[i]).X / 2.0f, mSize.Y + mSize.Height / mTextSizeList.Count * i);
                }
            }
            else
            {
                for (var i = 0; i < mTextList.Count && i < mTextSizeList.Count; i++)
                {
                    // var y = mSize.Y + mSize.Height / mTextSizeList.Count * (i + 1) - mFont.MeasureString(Text).Y / 2.0f;
                    mTextSizeList[i] = new Vector2(mSize.X + mSize.Width / 2.0f - mFont.MeasureString(mTextList[i]).X / 2.0f, mSize.Y + mSize.Height / 2.0f * (i + 1) - mFont.MeasureString(Text).Y / 2.0f);
                }
            }
            // Update the text size
            mTextSize.X = mSize.X + mSize.Width / 2.0f - mFont.MeasureString(Text).X / 2.0f;
            mTextSize.Y = mSize.Y + mSize.Height / 2.0f - mFont.MeasureString(Text).Y / 2.0f;

        }

    }

}
