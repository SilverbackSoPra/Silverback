using System;
using System.Collections.Generic;
using LevelEditor.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using LevelEditor.UIv2;
using System.Windows.Forms;
using Menu = LevelEditor.Ui.Menu;

namespace LevelEditor.Screen
{
    class LoseScreen : IScreen
    {
        private List<UIv2.Menu> mMenuList;
        private SpriteBatch mSpriteBatch;
        private Texture2D mBackgroundImageOptions;
        private SoundEffect mClickSound;

        private HudScreen mHudScreen;

        private GraphicsDevice mGraphicsDevice;

        private int mScreenWidth;
        private int mScreenHeight;

        public bool IsVisible { get; set; }

        public ScreenManager ScreenManager { get; set; }

        public SoundManager SoundManager { get; set; }

        private GraphicsDevice Device { get; set; }

        public LoseScreen(HudScreen screen)
        {
            mHudScreen = screen;
        }

        public void ChangeRenderingResolution(int width, int height)
        {
            throw new NotImplementedException();
        }

        public void ChangeWindowSize(int width, int height)
        {
            if (mMenuList == null)
            {
                return;
            }

            mScreenWidth = width;
            mScreenHeight = height;

            foreach (var menu in mMenuList)
            {
                menu.UpdateResolution(width, height);
            }
        }

        public void LoadContent(GraphicsDeviceManager deviceManager, ContentManager contentManager, int windowWidth, int windowHeight)
        {
            mScreenWidth = windowWidth;
            mScreenHeight = windowHeight;

            //Load SoundEffects
            mClickSound = contentManager.Load<SoundEffect>("Audio/click2");

            Device = deviceManager.GraphicsDevice;
            mSpriteBatch = new SpriteBatch(deviceManager.GraphicsDevice);

            // Unlock the framerate. Later we want to remove this piece of code
            mGraphicsDevice = deviceManager.GraphicsDevice;
            mSpriteBatch = new SpriteBatch(deviceManager.GraphicsDevice);

            // Load font
            var font = contentManager.Load<SpriteFont>("Font");
            var headerFont = contentManager.Load<SpriteFont>("Heading");
            IsVisible = true;

            // Load background image
            // mBackgroundImageOptions = contentManager.Load<Texture2D>("forest");

            mMenuList = new List<UIv2.Menu>();
            var menu = new UIv2.Menu(mGraphicsDevice, 5, 5, 90, 90);
            menu.WithBackground(Menu.CreateTexture2D(deviceManager.GraphicsDevice, 50, 30, pixel => new Color(0.0f, 0.0f, 0.0f, 0.2f)), 5, 5, 90, 90);
            mMenuList.Add(menu);

            Texture2D texture2D = Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.Black);
            Texture2D texture2DSliderPoint = Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.White);

            // GAME OVER Text
            var heading = new UIv2.Components.Label(mGraphicsDevice, 10, 20, 80, 30, "GAME OVER", headerFont, Color.DarkSlateGray);
            heading.AddTo(menu);

            var loseString = "The lumberjacks won... Now go bury yourself!";
            var loseLabel = new UIv2.Components.Label(mGraphicsDevice, 20, 60, 60, 10, loseString, font, Color.White);
            loseLabel.AddTo(menu);

            // Create Main Menu Button (Back Button)
            var mainMenuButton = new UIv2.Components.Button(mGraphicsDevice, 40, 70, 20, 8, texture2D, "Main Menu", font, Color.White);
            mainMenuButton.AddTo(menu);
            mainMenuButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                ScreenManager.Remove(mHudScreen);
                ScreenManager.Remove(this);
                IsVisible = false;
            });
        }

        public void Render(GameTime time)
        {
            if (!IsVisible)
            {
                return;
            }

            foreach (var menu in mMenuList)
            {
                menu.Render(mSpriteBatch);
            }
        }

        public void UnloadContent()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime time)
        {
            // Iterate over all Menu elements
            foreach (var menu in mMenuList)
            {
                if (!menu.mIsVisible)
                {
                    continue;
                }
                menu.CheckRegisteredEvents();
            }
        }
    }
}
