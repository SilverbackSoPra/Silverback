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
    sealed class CheckedButton : IDrawable2D, IMenuComponent
    {
        private bool mIsVisible;
        private readonly string mText;

        // Apperance
        private Rectangle mSize;
        private readonly Texture2D mTexture;
        private readonly Texture2D mTextureChecked;

        // Text
        private Vector2 mTextSize;
        private readonly Color mTextColor;
        private SpriteFont mFont;
        private readonly List<Event> mEvents;

        private GraphicsDevice mGraphicsDevice;
        private readonly Rectangle mRelativePosition;
        private Menu mParentMenu;

        private bool mChecked;

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
        
        public CheckedButton(GraphicsDevice device, int xPositionInPercent, int yPositionInPercent, int widthInPercent, int heightInPercent, Texture2D texture, Texture2D textureChecked, string text, SpriteFont font, Color textColor, bool isChecked)
        {
            mGraphicsDevice = device;
            mRelativePosition.X = xPositionInPercent;
            mRelativePosition.Y = yPositionInPercent;
            mRelativePosition.Width = widthInPercent;
            mRelativePosition.Height = heightInPercent;

            mTexture = texture;
            mTextureChecked = textureChecked;

            mFont = font;
            mTextColor = textColor;
            mText = text;

            mIsVisible = true;
            mEvents = new List<Event>();

            mChecked = isChecked;
            FontType = FontManager.FontType.Default;
            mWindowWidth = mGraphicsDevice.Viewport.Width;
            mWindowHeight = mGraphicsDevice.Viewport.Height;
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

            if (!mChecked)
            {
                if (mSize.Contains(Mouse.GetState().Position) && mEvents.Count > 0)
                {
                    batch.Draw(mTexture, mSize, Color.White);
                }
                else
                {
                    batch.Draw(mTexture, mSize, Color.White * .5f);
                }
            }
            else
            {
                batch.Draw(mTextureChecked, mSize, Color.White);
            }
            batch.DrawString(mFont, mText, mTextSize, mTextColor);

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
            mTextSize.X = mSize.X + mSize.Width / 2.0f - mFont.MeasureString(mText).X / 2.0f;
            mTextSize.Y = mSize.Y + mSize.Height / 2.0f - mFont.MeasureString(mText).Y / 2.0f;

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

            if (width != mWindowWidth || height != mWindowHeight)
            {
                mFont = FontManager.Get(mFontType, mGraphicsDevice.Viewport.Width, mGraphicsDevice.Viewport.Height);
            }

            mWindowWidth = width;
            mWindowHeight = height;


            // Update the text size
            mTextSize.X = mSize.X + mSize.Width / 2.0f - mFont.MeasureString(mText).X / 2.0f;
            mTextSize.Y = mSize.Y + mSize.Height / 2.0f - mFont.MeasureString(mText).Y / 2.0f;
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

        public void Check()
        {
            mChecked = true;
        }

        public void Uncheck()
        {
            mChecked = false;
        }

        /////////////////////
        // Private Methods //
        /////////////////////

    }

}
