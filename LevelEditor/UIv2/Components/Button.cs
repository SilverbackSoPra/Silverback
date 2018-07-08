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
    sealed class Button: IDrawable2D, IMenuComponent
    {
        private bool mIsVisible;
        private string mText;

        private List<string> mTextList;
        private List<Vector2> mTextSizeList;

        private bool mAutoBreak;

        public string Text
        {
            get
            {
                return string.Join("\n", mTextList);
            }
            set
            {
                mText = value;
                var textArr = mText.Split(' ');

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

        // Apperance
        private Rectangle mSize;
        public Texture2D mTexture;

        // Text
        private Vector2 mTextSize;
        private readonly Color mTextColor;
        private readonly SpriteFont mFont;
        private readonly List<Event> mEvents;

        private GraphicsDevice mGraphicsDevice;
        private readonly Rectangle mRelativePosition;
        private Menu mParentMenu;

        public Button(GraphicsDevice device, int xPositionInPercent, int yPositionInPercent, int widthInPercent, int heightInPercent, Texture2D texture, string text, SpriteFont font, Color textColor)
        {
            mGraphicsDevice = device;
            mRelativePosition.X = xPositionInPercent;
            mRelativePosition.Y = yPositionInPercent;
            mRelativePosition.Width = widthInPercent;
            mRelativePosition.Height = heightInPercent;

            mTextList = new List<string>();
            mTextSizeList = new List<Vector2>();
            mTexture = texture;

            mFont = font;
            mTextColor = textColor;
            Text = text;

            mAutoBreak = false;

            mIsVisible = true;
            mEvents = new List<Event>();
        }

        ////////////////////
        // Public methods //
        ////////////////////

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
            for (var i = 0; i < mTextList.Count && i < mTextSizeList.Count; i++)
            {
                // mTextSizeList[i] = new Vector2(mSize.X + mSize.Width / 2.0f - mFont.MeasureString(mTextList[i]).X / 2.0f, mSize.Y + mSize.Height / 2.0f - mFont.MeasureString(mTextList[i]).Y / 2.0f);
                batch.DrawString(mFont, mTextList[i], mTextSizeList[i], mTextColor);
            }

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
            // Update the the buttons position
            var size = mSize;

            size.X = mRelativePosition.X * width / 100 + mParentMenu.Size.X;
            size.Y = mRelativePosition.Y * height / 100 + mParentMenu.Size.Y;
            size.Width = mRelativePosition.Width * width / 100;
            size.Height = mRelativePosition.Height * height / 100;

            mSize = size;

            Text = mText;
        }

        public Rectangle GetSize()
        {
            return mSize;
        }

        public void SetSize(Rectangle r)
        {
            mTextSize.X += r.X - mSize.X;
            mTextSize.Y += r.Y - mSize.Y;
            mSize = r;
        }

        public void SetVisibility(bool v)
        {
            mIsVisible = v;
        }

        //////////////////////////
        // All the Event things //
        //////////////////////////

        public void CheckRegisteredEvents()
        {
            // Check keyboard events
            var keyboardEvents = mEvents.FindAll(e => e.EventType == EventType.Keyboard);
            foreach (var e in keyboardEvents)
            {
                if (e.InputState == InputState.Down && InputManager.AllKeysDown(e.mKey))
                {
                    e.Action();
                    continue;
                }
                if (e.InputState == InputState.Pressed && InputManager.AllKeysPressed(e.mKey))
                {
                    e.Action();
                    continue;
                }
                if (e.InputState == InputState.Up && InputManager.AllKeysUp(e.mKey))
                {
                    e.Action();
                    continue;
                }
                if (e.InputState == InputState.Released && InputManager.AllKeysReleased(e.mKey))
                {
                    e.Action();
                }
            }

            if (!mSize.Contains(Mouse.GetState().Position))
            {
                return;
            }

            // Check all mouse events
            var mouseEvents = mEvents.FindAll(e => e.EventType == EventType.Mouse);
            foreach (var e in mouseEvents)
            {
                if (e.InputState == InputState.Down && InputManager.AllMouseButtonsDown(e.mMouseButton))
                {
                    e.Action();
                    continue;
                }
                if (e.InputState == InputState.Pressed && InputManager.AllMouseButtonsPressed(e.mMouseButton))
                {
                    e.Action();
                    continue;
                }
                if (e.InputState == InputState.Up && InputManager.AllMouseButtonsUp(e.mMouseButton))
                {
                    e.Action();
                    continue;
                }
                if (e.InputState == InputState.Released && InputManager.AllMouseButtonsReleased(e.mMouseButton))
                {
                    e.Action();
                }
            }
        }

        public void AddListener(MouseButtons mb, InputState inputState, Action a)
        {
            var e = new Event(mb, inputState, a);
            if (mEvents.Contains(e))
            {
                return;
            }

            mEvents.Add(e);
        }

        public void AddListener(Keys k, InputState inputState, Action a)
        {
            var e = new Event(k, inputState, a);
            if (mEvents.Contains(e))
            {
                return;
            }

            mEvents.Add(e);
        }

        public void AddListener(Event e)
        {
            if (mEvents.Contains(e))
            {
                return;
            }

            mEvents.Add(e);
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
                    mTextSizeList[i] = new Vector2(mSize.X + mSize.Width / 2.0f - mFont.MeasureString(mTextList[i]).X / 2.0f, mSize.Y + mSize.Height / mTextSizeList.Count * (i));
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
    }
}
