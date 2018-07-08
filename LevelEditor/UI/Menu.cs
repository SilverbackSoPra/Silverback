using System;
using System.Collections.Generic;
using LevelEditor.Events;
using LevelEditor.UI.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LevelEditor.Ui
{
    internal sealed class Menu
    {
        private readonly List<IDrawable2D> mElementList = new List<IDrawable2D>();
        private readonly SpriteBatch mBatch;
        private GraphicsDevice mGraphicsDevice;

        private int ScreenWidth { get; set; }
        private int ScreenHeight { get; set; }

        public Menu(GraphicsDevice device)
        {
            mGraphicsDevice = device;
            mBatch = new SpriteBatch(device);
            ScreenWidth = 1920;
            ScreenHeight = 1080;
        }

        public void SetRelativePosition(float windowWidth, float windowHeight, float xperc, float yperc, float elemWidth, float elemHeigth)
        {
            Size = RelativeToAbsolutePosition(windowWidth, windowHeight, xperc, yperc, elemWidth, elemHeigth);
        }

        private Rectangle Size { get; set; }

        public void AddElement(IDrawable2D elem)
        {
            try
            {
                elem.SetWindowWidth(ScreenWidth);
                elem.SetWindowHeight(ScreenHeight);
            }
            catch (NotImplementedException)
            { }
            var size = elem.Size;
            size.Offset(Size.X, Size.Y);
            elem.Size = size;
            mElementList.Add(elem);
        }

        public void RemoveElement(IDrawable2D elem)
        {
            mElementList.Remove(elem);
        }

        public void Render()
        {

            if (!Visible)
            {
                return;
            }

            mBatch.Begin(depthStencilState: DepthStencilState.Default);
            // Iterate over all elements from ElementList and call elem.Draw(batch);
            foreach (var elem in mElementList)
            {
                elem.Render(mBatch);
            }

            mBatch.End();

        }

        private bool mVisible;

        private bool Visible
        {
            get { return mVisible; }
            set
            {
                mVisible = value;
                // UpdateResolution(mGraphicsDevice.Viewport.Width, mGraphicsDevice.Viewport.Height);
            }
        }

        public void UpdateResolution(int width, int height)
        {
            // Console.WriteLine("\nmElementList.Count" + mElementList.Count + " Menu.UpdateResolution. width is " + width + " height is " + height + "\n");
            var size = Size;
            size.X = size.X * width / ScreenWidth;
            size.Y = size.Y * height / ScreenHeight;
            size.Width *= width / ScreenWidth;
            size.Height *= height / ScreenHeight;
            Size = size;

            ScreenWidth = width;
            ScreenHeight = height;
            foreach (var elem in mElementList)
            {
                // size = elem.Size;
                // size.Offset(Size.X, Size.Y);
                // elem.Size = size;
                elem.UpdateResolution(width, height);
            }
        }

        public void Clicked()
        {
            foreach (var elem in mElementList)
            {
                elem.Clicked();
            }

            if (Size.Contains(Mouse.GetState().Position))
            {
                InputManager.MouseLeftButtonDown();
                InputManager.MouseRightButtonDown();
            }
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
        
        public static Texture2D CreateBorder(Texture2D texture, int borderWidth, Color borderColor)
        {
            var colors = new Color[texture.Width * texture.Height];

            for (var x = 0; x < texture.Width; x++)
            {
                for (var y = 0; y < texture.Height; y++)
                {
                    var colored = false;
                    for (var i = 0; i <= borderWidth; i++)
                    {
                        if (x != i && y != i && x != texture.Width - 1 - i && y != texture.Height - 1 - i)
                        {
                            continue;
                        }

                        colors[x + y * texture.Width] = borderColor;
                        colored = true;
                        break;
                    }

                    if (colored == false)
                    {
                        colors[x + y * texture.Width] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colors);
            return texture;
        }

        public static Rectangle RelativeToAbsolutePosition(float windowWidth, float windowHeight, float xperc, float yperc, float elemWidthPerc, float elemHeightPerc)
        {
            var xAbs = (int)Math.Round(windowWidth * xperc / 100);
            var yAbs = (int)Math.Round(windowHeight * yperc / 100);
            var widthAbs = (int)Math.Round(windowWidth * elemWidthPerc / 100);
            var heightAbs = (int)Math.Round(windowHeight * elemHeightPerc / 100);
            return new Rectangle(xAbs, yAbs, widthAbs, heightAbs);
        }
    }
}
