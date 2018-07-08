using System;
using System.Collections.Generic;
using LevelEditor.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Menu = LevelEditor.Ui.Menu;

namespace LevelEditor.UI.Components
{
    internal sealed class Slider: IDrawable2D
    {
        private int mScrollSpeed = 1;
        private int mCurrentYPos;

        private bool mIsScrolling;
        private bool mIsHovering;

        private List<Action<float>> ChangeActions;
        private int ScreenWidth { get; set; }
        private int ScreenHeight { get; set; }

        private float mValue;
        // {
        //     set { mValue = (value - Range.X) * (Size.Width - SizePoint.Width) / (Range.Y - Range.X); }
        // };

        private float Value
        {
            get
            {
                var val = 0.0f;
                // Get the amount of possible values
                // E.g. 0 to 100 --> 101 possible values (100 - 0 + 1)
                val = Range.Y - Range.X;
                // Get the x value per rangepoint
                val = (Size.Width - SizePoint.Width) / val;
                // 
                val = ((mValue - Size.X) / val) + Range.X;
                return val;
            }
            set
            {
                // if (value < Range.X)
                // {
                //     value = Range.X;
                // }
                // 
                // if (value > Range.Y)
                // {
                //     value = Range.Y;
                // }

                mValue = value;
                var size = SizePoint;
                var tmp = Mouse.GetState().Position;
                size.X = (int)value;// (int) (((value - Range.X) * (Size.Width - SizePoint.Width) / (Range.Y - Range.X)) + Size.X);
                SizePoint = size;
                foreach (var fn in ChangeActions)
                {
                    fn(Value);
                }
            }
        }

        public Slider(GraphicsDevice graphicsDevice, int width, int height, int startRange, int endRange, int scrollSpeed)
        {
            mScrollSpeed = scrollSpeed;
            ChangeActions = new List<Action<float>>();
            Value = 0;
            Visible = true;
            SizePoint = new Rectangle(0, 0, 0, 0);
            TextureSlider = Menu.CreateTexture2D(graphicsDevice, width, height, pixel => Color.Black);
            Range = new Vector2(startRange, endRange);
            TextureRange = Menu.CreateTexture2D(graphicsDevice, height / 2, height / 2, pixel => Color.White);
            TextureRangeBig = Menu.CreateTexture2D(graphicsDevice, height, height / 4, pixel => Color.White);
        }

        private Vector2 Range { get; set; }

        private Texture2D TextureSlider { get; set; }
        private Texture2D TextureRange { get; set; }
        private Texture2D TextureRangeBig { get; set; }
        public void SetRelativePosition(float windowWidth, float windowHeight, float xperc, float yperc, float elemWidth, float elemHeigth)
        {
            Size = Menu.RelativeToAbsolutePosition(windowWidth, windowHeight, xperc, yperc, elemWidth, elemHeigth);
        }

        public void UpdateAllPositions()
        {
            // Offset SizePoint
            var size = SizePoint;
            size.Offset(Size.X, Size.Y);
            size.Width = Size.Height / 2;
            size.Height = Size.Height / 2;
            size.Y = size.Y + (Size.Y - size.Y) + size.Height/2;
            SizePoint = size;

            size.Height = Size.Height;
            SizePointBig = size;

            TextureSlider = Menu.CreateTexture2D(TextureSlider.GraphicsDevice, Size.Width, Size.Height, pixel => Color.Black);
        }

        public void Render(SpriteBatch batch)
        {
            if (!Visible)
            {
                return;
            }

            if (mIsHovering)
            {
                batch.Draw(TextureSlider, Size, Color.White);
            }
            else
            {
                batch.Draw(TextureSlider, Size, Color.White * .5f);
            }
            
            // if (mIsScrolling)
            // {
            //     batch.Draw(TextureRangeBig, SizePointBig, Color.White);
            // }
            // else
            // {
                batch.Draw(TextureRange, SizePoint, Color.White);
            // }
        }

        public bool Clicked()
        {
            // Check if the mouse is inside
            if ((Size.X + Size.Width) < (SizePoint.X + SizePoint.Width))
            {
                mIsScrolling = false;
                Value = Size.X + Size.Width - SizePoint.Width; // Range.Y;
                return false;
            }
            if (Size.X > SizePoint.X)
            {
                mIsScrolling = false;
                Value = Size.X; // Range.X;
                return false;
            }

            if (!Size.Contains(Mouse.GetState().Position))
            {
                mIsScrolling = false;
                mIsHovering = false;
                return false;
            }
            mIsHovering = true;

            if (InputManager.MouseLeftButtonPressed() || InputManager.MouseLeftButtonDown())
            {
                mIsScrolling = true;
            }
            else if (InputManager.MouseLeftButtonReleased())
            {
                mIsScrolling = false;
            }
            else
            {
                mIsScrolling = false;
                return false;
            }

            var tmp = ((Mouse.GetState().X - Size.X) / ((Size.Width - SizePoint.Width) / (Range.Y - Range.X)));

            Value = Mouse.GetState().Position.X; // tmp;
            return false;
        }
        
        public Rectangle Size { get; set; }
        private Rectangle SizePoint { get; set; }
        private Rectangle SizePointBig { get; set; }
        public bool Visible { get; set; }
        public void SetWindowWidth(int width)
        {
            ScreenWidth = width;
        }

        public void SetWindowHeight(int height)
        {
            ScreenHeight = height;
        }

        public void UpdateResolution(int width, int height)
        {
            if (ScreenWidth != 0 && ScreenHeight != 0)
            {
                var size = Size;
                size.X = size.X * width / ScreenWidth;
                size.Y = size.Y * height / ScreenHeight;
                size.Width = size.Width * width / ScreenWidth;
                size.Height = size.Height * height / ScreenHeight;
                Size = size;

                size = SizePoint;
                size.X = size.X * width / ScreenWidth;
                size.Y = size.Y * height / ScreenHeight;
                size.Width = size.Width * width / ScreenWidth;
                size.Height = size.Height * height / ScreenHeight;
                SizePoint = size;
            }
            ScreenWidth = width;
            ScreenHeight = height;
        }

        public void AddListener(ButtonState s, SbButtonType t, Func<bool> on)
        {
            throw new NotImplementedException();
        }

        public void AddOnChangeListeners(params Action<float>[] fn)
        {
            ChangeActions.AddRange(fn);
        }
        public void RemoveOnChangeListener(Action<float> fn)
        {
            ChangeActions.Remove(fn);
        }
    }
}
