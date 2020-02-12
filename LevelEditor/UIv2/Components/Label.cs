using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace LevelEditor.UIv2.Components
{
    [Serializable]
    public sealed class Label: IDrawable2D, IMenuComponent, ISerializable
    {
        private bool mIsVisible;
        private string mText;

        private List<string> mTextList;
        private List<Vector2> mTextSizeList;

        public bool mAutoBreak;

        private FontManager.FontType mFontType;

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

        public string Text
        {
            get
            {
                return string.Join("\n", mTextList);
            }
            set
            {
                mText = value;

                mTextList.Clear();
                mTextList = new List<string>();
                mTextSizeList.Clear();
                mTextSizeList = new List<Vector2>();

                if (!mAutoBreak)
                {
                    mTextList.Add(value);
                    mTextSizeList.Add(new Vector2());
                    ResizeTextLines();
                    return;
                }

                var textArr = mText.Split(' ');
                mTextList.Add("");

                foreach (var t in textArr)
                {
                    var tmpText = mTextList[mTextList.Count - 1] + " " + t;
                    var fontMessures = mFont.MeasureString(tmpText);
                    if (fontMessures.X >= mSize.Width)
                    {
                        mTextList[mTextList.Count - 1] = mTextList[mTextList.Count - 1].Trim();
                        mTextSizeList.Add(new Vector2());
                        mTextList.Add("");
                    }
                    mTextList[mTextList.Count - 1] += " " + t;
                }
                mTextList[mTextList.Count - 1] = mTextList[mTextList.Count - 1].Trim();
                mTextSizeList.Add(new Vector2());
                ResizeTextLines();
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


        private Texture2D mDebugTexture2D;

        public Label(GraphicsDevice device, int xPositionInPercent, int yPositionInPercent, int widthInPercent, int heightInPercent, string text, SpriteFont font, Color textColor)
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

            mIsVisible = true;
            mAutoBreak = true;

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

            for (var i = 0; i < mTextList.Count && i < mTextSizeList.Count; i++)
            {
                // mTextSizeList[i] = new Vector2(mSize.X + mSize.Width / 2.0f - mFont.MeasureString(mTextList[i]).X / 2.0f, mSize.Y + mSize.Height / 2.0f - mFont.MeasureString(mTextList[i]).Y / 2.0f);
                batch.DrawString(mFont, mTextList[i], mTextSizeList[i], mTextColor);
            }

            // For debug purposes only, please do not remove this and the next line
            // batch.Draw(mDebugTexture2D, mSize, Color.White*.5f);
            // batch.DrawString(mFont, Text, mTextSize, mTextColor);
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

        public void DisableAutobreak()
        {
            mAutoBreak = false;
            Text = mText;
        }

        public void EnableAutobreak()
        {
            mAutoBreak = true;
            Text = mText;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("mIsVisible", mIsVisible);
            info.AddValue("mText", mText);
            info.AddValue("mTextList", mTextList);
            info.AddValue("mTextSizeList", mTextSizeList);
            info.AddValue("mAutoBreak", mAutoBreak);
            info.AddValue("mSize", mSize);
            info.AddValue("mTextSize", mTextSize);
            info.AddValue("mTextColor", mTextColor);
            info.AddValue("mFont", mFont);
            info.AddValue("mGraphicsDevice", mGraphicsDevice);
            info.AddValue("mRelativePosition", mRelativePosition);
            info.AddValue("mParentMenu", mParentMenu);
            info.AddValue("mDebugTexture2D", mDebugTexture2D);
            /*
                private bool mIsVisible;
                private string mText;

                private List<string> mTextList;
                private List<Vector2> mTextSizeList;

                public bool mAutoBreak;

                private Rectangle mSize;

                // Text
                private Vector2 mTextSize;
                public Color mTextColor;
                private readonly SpriteFont mFont;

                private GraphicsDevice mGraphicsDevice;
                private readonly Rectangle mRelativePosition;
                private Menu mParentMenu;


                private Texture2D mDebugTexture2D;
             */
        }

        public Label(SerializationInfo info, StreamingContext context)
        {
            mIsVisible = (bool)info.GetValue("mIsVisible", typeof(bool));
            mText = (string)info.GetValue("mText", typeof(string));
            mTextList = (List<string>)info.GetValue("mTextList", typeof(List<string>));
            mTextSizeList = (List<Vector2>)info.GetValue("mTextSizeList", typeof(Vector2));
            mAutoBreak = (bool)info.GetValue("mAutoBreak", typeof(bool));
            mSize = (Rectangle)info.GetValue("mSize", typeof(Rectangle));
            mTextSize = (Vector2)info.GetValue("mTextSize", typeof(Vector2));
            mTextColor = (Color)info.GetValue("mTextColor", typeof(Color));
            mFont = (SpriteFont)info.GetValue("mFont", typeof(SpriteFont));
            mGraphicsDevice = (GraphicsDevice)info.GetValue("mGraphicsDevice", typeof(GraphicsDevice));
            mRelativePosition = (Rectangle)info.GetValue("mRelativePosition", typeof(Rectangle));
            mParentMenu = (Menu)info.GetValue("mParentMenu", typeof(Menu));
            mDebugTexture2D = (Texture2D)info.GetValue("mDebugTexture2D", typeof(Texture2D));
            /*
                private bool mIsVisible;
                private string mText;

                private List<string> mTextList;
                private List<Vector2> mTextSizeList;

                public bool mAutoBreak;

                private Rectangle mSize;

                // Text
                private Vector2 mTextSize;
                public Color mTextColor;
                private readonly SpriteFont mFont;

                private GraphicsDevice mGraphicsDevice;
                private readonly Rectangle mRelativePosition;
                private Menu mParentMenu;


                private Texture2D mDebugTexture2D;
             */
        }

        public Label()
        { }

    }
}
