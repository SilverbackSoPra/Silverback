using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using LevelEditor.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LevelEditor.UIv2
{
    [Serializable()]
    public sealed class Menu : IDrawable2D, ISerializable
    {
        public readonly List<IDrawable2D> mElementList;
        public bool mIsVisible;

        public Rectangle Size { get; private set; }

        private readonly Rectangle mRelativePosition;
        private int mScreenWidth;
        private int mScreenHeight;

        private bool mSolid;

        // Background
        private bool mHasBackground;
        private Texture2D mBackgorundTexture2D;
        private Rectangle mBackgroundSizeRelative;
        private Rectangle mBackgroundSize;

        public Menu(GraphicsDevice device, int xPositionInPercent, int yPositionInPercent, int widthInPercent, int heightInPercent)
        {
            var graphicsDevice = device;
            mRelativePosition.X = xPositionInPercent;
            mRelativePosition.Y = yPositionInPercent;
            mRelativePosition.Width = widthInPercent;
            mRelativePosition.Height = heightInPercent;
            
            mScreenWidth = graphicsDevice.Viewport.Width;
            mScreenHeight = graphicsDevice.Viewport.Height;
            
            mIsVisible = true;
            mHasBackground = false;
            mSolid = true;

            var size = new Rectangle
            {
                X = mRelativePosition.X * mScreenWidth / 100,
                Y = mRelativePosition.Y * mScreenHeight / 100,
                Width = mRelativePosition.Width * mScreenWidth / 100,
                Height = mRelativePosition.Height * mScreenHeight / 100
            };
            
            Size = size;

            mElementList = new List<IDrawable2D>();

        }

        public void Render(SpriteBatch batch)
        {
            if (!mIsVisible)
            {
                return;
            }

            batch.Begin(depthStencilState: DepthStencilState.Default);

            if (mHasBackground)
            {
                batch.Draw(mBackgorundTexture2D, mBackgroundSize, Color.White);
            }

            foreach (var elem in mElementList)
            {
                elem.Render(batch);

            }
            batch.End();
        }

        public void CheckRegisteredEvents()
        {
            var elems = mElementList.ToArray();
            foreach (var elem in elems)
            {
                elem.CheckRegisteredEvents();
            }

            if (!Size.Contains(Mouse.GetState().Position))
            {
                return;
            }

            if (!mSolid)
            {
                return;
            }
            // Make the menu solid
            InputManager.MouseLeftButtonDown();
            InputManager.MouseRightButtonDown();
        }

        public void UpdateResolution(int width, int height)
        {
            // Update the menu

            var size = Size;

            size.X = mRelativePosition.X * width / 100;
            size.Y = mRelativePosition.Y * height / 100;
            size.Width = mRelativePosition.Width * width / 100;
            size.Height = mRelativePosition.Height * height / 100;

            Size = size;
            mScreenWidth = width;
            mScreenHeight = height;

            // Update the background
            if (mHasBackground)
            {
                mBackgroundSize.X = Size.Width * mBackgroundSizeRelative.X / 100 + Size.X;
                mBackgroundSize.Y = Size.Height * mBackgroundSizeRelative.Y / 100 + Size.Y;
                mBackgroundSize.Width = Size.Width * mBackgroundSizeRelative.Width / 100;
                mBackgroundSize.Height = Size.Height * mBackgroundSizeRelative.Height / 100;
            }

            // Update the subcomponents
            foreach (var elem in mElementList)
            {
                elem.UpdateResolution(size.Width, size.Height);
            }
        }

        public Rectangle GetSize()
        {
            return Size;
        }

        public void SetSize(Rectangle r)
        {
            Size = r;
        }

        public void SetVisibility(bool v)
        {
            mIsVisible = v;
        }

        public void WithBackground(Texture2D texture, int xPositionInPercent, int yPositionInPercent, int widthInPercent, int heightInPercent)
        {
            mHasBackground = true;
            mBackgorundTexture2D = texture;
            // Adjust position
            mBackgroundSizeRelative.X = xPositionInPercent;
            mBackgroundSizeRelative.Y = yPositionInPercent;
            mBackgroundSizeRelative.Width = widthInPercent;
            mBackgroundSizeRelative.Height = heightInPercent;


            mBackgroundSize.X = Size.Width * mBackgroundSizeRelative.X / 100 + Size.X;
            mBackgroundSize.Y = Size.Height * mBackgroundSizeRelative.Y / 100 + Size.Y;
            mBackgroundSize.Width = Size.Width * mBackgroundSizeRelative.Width / 100;
            mBackgroundSize.Height = Size.Height * mBackgroundSizeRelative.Height / 100;

        }

        public void Solid()
        {
            mSolid = true;
        }

        public void NonSolid()
        {
            mSolid = false;
        }

        public static Texture2D CreateTexture2D(GraphicsDevice device, int width, int height, Func<int, Color> paint)
        {
            var texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            var data = new Color[width * height];
            for (var pixel = 0; pixel < data.Length; pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = paint(pixel);
            }

            //set the color
            texture.SetData(data);

            return texture;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // info.AddValue("mElementList", mElementList);
            info.AddValue("mIsVisible", mIsVisible);
            info.AddValue("Size", Size);
            info.AddValue("mRelativePosition", mRelativePosition);
            info.AddValue("mScreenWidth", mScreenWidth);
            info.AddValue("mScreenHeight", mScreenHeight);
            info.AddValue("mSolid", mSolid);
            info.AddValue("mHasBackground", mHasBackground);
            info.AddValue("mBackgorundTexture2D", mBackgorundTexture2D);
            info.AddValue("mBackgroundSizeRelative", mBackgroundSizeRelative);
            info.AddValue("mBackgroundSize", mBackgroundSize);
            /*
                public readonly List<IDrawable2D> mElementList;
                public bool mIsVisible;

                public Rectangle Size { get; private set; }

                private readonly Rectangle mRelativePosition;
                private int mScreenWidth;
                private int mScreenHeight;

                private bool mSolid;

                // Background
                private bool mHasBackground;
                private Texture2D mBackgorundTexture2D;
                private Rectangle mBackgroundSizeRelative;
                private Rectangle mBackgroundSize;
             */
        }

        public Menu(SerializationInfo info, StreamingContext context)
        {
            // mElementList = (List<IDrawable2D>)info.GetValue("mElementList", typeof(List<IDrawable2D>));
            mIsVisible = (bool)info.GetValue("mIsVisible", typeof(bool));
            Size = (Rectangle)info.GetValue("Size", typeof(Rectangle));
            mRelativePosition = (Rectangle)info.GetValue("mRelativePosition", typeof(Rectangle));
            mScreenWidth = (int)info.GetValue("mScreenWidth", typeof(int));
            mScreenHeight = (int)info.GetValue("mScreenHeight", typeof(int));
            mSolid = (bool)info.GetValue("mSolid", typeof(bool));
            mHasBackground = (bool)info.GetValue("mHasBackground", typeof(bool));
            mBackgorundTexture2D = (Texture2D)info.GetValue("mBackgorundTexture2D", typeof(Texture2D));
            mBackgroundSizeRelative = (Rectangle)info.GetValue("mBackgroundSizeRelative", typeof(Rectangle));
            mBackgroundSize = (Rectangle)info.GetValue("mBackgroundSize", typeof(Rectangle));
            /*
                public readonly List<IDrawable2D> mElementList;
                public bool mIsVisible;

                public Rectangle Size { get; private set; }

                private readonly Rectangle mRelativePosition;
                private int mScreenWidth;
                private int mScreenHeight;

                private bool mSolid;

                // Background
                private bool mHasBackground;
                private Texture2D mBackgorundTexture2D;
                private Rectangle mBackgroundSizeRelative;
                private Rectangle mBackgroundSize;
             */
        }
        public Menu() { }
    }
}
