using LevelEditor.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using LevelEditor.Engine;
using LevelEditor.Objects.Ape;
using LevelEditor.Objects.Ape.SubApe;
using LevelEditor.UIv2;

namespace LevelEditor.Screen
{
    class LoadingScreen : IScreen
    {

        private List<UIv2.Menu> mMenuList;
        public ScreenManager ScreenManager { get; set; }

        public SoundManager SoundManager { get; set; }
        public bool IsVisible { get; set; }

        private int mScreenWidth;
        private int mScreenHeight;

        private SpriteBatch mSpriteBatch;
        private Texture2D mBackgroundImageCredits;
        private GraphicsDevice mGraphicsDevice;
        private Vector2 mTotalWindowSize;
        private MainMenuScreen mMainMenuScreen;

        private SoundEffect mClickSound;
        private string mLevelPath;

        private bool mDeleteScreen = false;
        private bool mLoadSaveGame = false;

        public LoadingScreen(string levelPath)
        {
            mLevelPath = levelPath;
        }

        public void LoadContent(GraphicsDeviceManager deviceManager, ContentManager contentManager, int windowWidth, int windowHeight)
        {

            mScreenWidth = windowWidth;
            mScreenHeight = windowHeight;

            //Load SoundEffects
            mClickSound = contentManager.Load<SoundEffect>("Audio/click2");

            mGraphicsDevice = deviceManager.GraphicsDevice;
            mSpriteBatch = new SpriteBatch(deviceManager.GraphicsDevice);
            mTotalWindowSize = new Vector2(mGraphicsDevice.Viewport.Width, mGraphicsDevice.Viewport.Height);

            // Load font
            var font = contentManager.Load<SpriteFont>("Font");
            var subHeaderFont = contentManager.Load<SpriteFont>("SubHeaderFont");
            var headerFont = contentManager.Load<SpriteFont>("Heading");
            IsVisible = true;

            // Load backround Image
            mBackgroundImageCredits = contentManager.Load<Texture2D>("forest");
            IsVisible = true;

            Texture2D texture2D = UIv2.Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.Black);

            mMenuList = new List<UIv2.Menu>();

            // Instantiate a new menu
            var menu = new UIv2.Menu(mGraphicsDevice, 0, 0, 100, 100);
            menu.WithBackground(UIv2.Menu.CreateTexture2D(mGraphicsDevice, (int)mTotalWindowSize.X, (int)mTotalWindowSize.Y, pixel => new Color(0.0f, 0.0f, 0.0f, 0.0f)), 5, 5, 90, 90);

            mMenuList.Add(menu);

            var heading = new UIv2.Components.Label(mGraphicsDevice, 10, 0, 80, 30, "Silverback", headerFont, Color.DarkSlateGray);
            heading.FontType = FontManager.FontType.Heading;
            heading.AddTo(menu);

            var subheading = new UIv2.Components.Label(mGraphicsDevice, 35, 65, 30, 15, "Loading...", subHeaderFont, Color.White);
            subheading.FontType = FontManager.FontType.Subheading;
            subheading.AddTo(menu);

            var text = new UIv2.Components.Label(mGraphicsDevice, 20, 80, 60, 15, "Hurry! The apes are waiting for your help", font, Color.White);
            text.AddTo(menu);

        }

        public void UnloadContent()
        {
            mMenuList.Clear();
        }

        public void Update(GameTime time)
        {

            // We just need to render once
            if (mDeleteScreen)
            {
                var hudScreen = new HudScreen(mLevelPath);
                ScreenManager.Add(hudScreen);
                ScreenManager.Remove(this);

                if (mLoadSaveGame)
                {
                    LoadSaveGame(ref hudScreen);
                }
            }
            else {

                var menus = mMenuList.ToArray();

                // Iterate over all Menu elements
                foreach (var menu in menus)
                {
                    if (!menu.mIsVisible)
                    {
                        continue;
                    }
                    menu.CheckRegisteredEvents();
                }

                mDeleteScreen = true;

            }

        }

        public void Render(GameTime time)
        {
            if (!IsVisible)
            {
                return;
            }
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackgroundImageCredits, new Rectangle(0, 0, mScreenWidth, mScreenHeight), Color.White);
            mSpriteBatch.End();
            foreach (var menu in mMenuList)
            {
                menu.Render(mSpriteBatch);
            }
            
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

        public void ChangeRenderingResolution(int width, int height)
        {
            throw new NotImplementedException();
        }

        public void LoadSaveGame()
        {
            mLoadSaveGame = true;
        }

        private void LoadSaveGame(ref HudScreen hudScreen)
        {
            var err = PauseScreen.ReinitialseSavegame(ref hudScreen);
            if (err != "")
            {
                mLoadSaveGame = false;
                ScreenManager.Remove(this);
                return;
            }
        }

    }
}
