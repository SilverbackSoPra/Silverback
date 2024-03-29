﻿using System;
using System.Collections.Generic;
using LevelEditor.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using LevelEditor.UIv2;
using System.Windows.Forms;
using System.IO;

namespace LevelEditor.Screen
{
    class WinScreen : IScreen
    {
        private List<UIv2.Menu> mMenuList;
        private SpriteBatch mSpriteBatch;
        private Texture2D mBackgroundImage;
        private SoundEffect mClickSound;

        private HudScreen mHudScreen;

        private GraphicsDevice mGraphicsDevice;

        private int mScreenWidth;
        private int mScreenHeight;

        public bool IsVisible { get; set; }

        public ScreenManager ScreenManager { get; set; }

        public SoundManager SoundManager { get; set; }

        private GraphicsDevice Device { get; set; }

        public WinScreen(HudScreen screen)
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
            mBackgroundImage = contentManager.Load<Texture2D>("Forest");

            mMenuList = new List<UIv2.Menu>();
            var menu = new UIv2.Menu(mGraphicsDevice, 5, 5, 90, 90);
            menu.WithBackground(UIv2.Menu.CreateTexture2D(deviceManager.GraphicsDevice, 50, 30, pixel => new Color(0.0f, 0.0f, 0.0f, 0.2f)), 5, 5, 90, 90);
            mMenuList.Add(menu);

            Texture2D texture2D = UIv2.Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.Black);
            Texture2D texture2DSliderPoint = UIv2.Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.White);

            // Congratulations!
            var congratz = new UIv2.Components.Label(mGraphicsDevice, 10, 0, 80, 30, "Congratulations!", headerFont, Color.DarkSlateGray);
            congratz.FontType = FontManager.FontType.Heading;
            congratz.AddTo(menu);

            // You've won
            var win = new UIv2.Components.Label(mGraphicsDevice, 10, 30, 80, 30, "You've won", headerFont, Color.DarkSlateGray);
            win.FontType = FontManager.FontType.Heading;
            win.AddTo(menu);

            var winString = "Nice work dude!";
            var winLabel = new UIv2.Components.Label(mGraphicsDevice, 20, 60, 60, 10, winString, font, Color.White);
            winLabel.AddTo(menu);

            if (mHudScreen.mGameScreen.mLevel.mNextLevelFilename != "null")
            {
                var nextLevelButton = new UIv2.Components.Button(mGraphicsDevice, 40, 70, 20, 7, texture2D, "Next level", font, Color.White);
                nextLevelButton.AddTo(menu);
                nextLevelButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
                {
                    var nextLevelFilename = mHudScreen.mGameScreen.mLevel.mNextLevelFilename;
                // Check if path is relative
                if (!Path.IsPathRooted(nextLevelFilename))
                    {
                        var levelFilename = mHudScreen.mGameScreen.mLevel.mLevelFilename;
                        var directoryPath = Path.GetDirectoryName(levelFilename);
                        nextLevelFilename = directoryPath + "/" + nextLevelFilename;
                    }
                    SoundManager.AddSound(mClickSound);
                    ScreenManager.Remove(this);
                    IsVisible = false;
                    var loadingScreen = new LoadingScreen(nextLevelFilename);
                    ScreenManager.Add(loadingScreen);
                });
            }
            else
            {
                winLabel.Text += " You've finished the game.";
                if (Statistic.CurrentSaveTime < Statistic.MinimalTime || Statistic.MinimalTime == 0)
                {
                    Statistic.MinimalTime = Statistic.CurrentSaveTime;
                    Achievements.Gamemaster = true;
                    if (Statistic.MinimalTime < 3600000)
                    {
                        Achievements.Speedrunner = true;
                    }
                }
            }

            // Create Main Menu Button (Back Button)
            var backButton = new UIv2.Components.Button(mGraphicsDevice, 40, 80, 20, 7, texture2D, "Main menu", font, Color.White);
            backButton.AddTo(menu);
            backButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
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
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackgroundImage, new Rectangle(0, 0, mScreenWidth, mScreenHeight), Color.White);
            mSpriteBatch.End();
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
