using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LevelEditor.UIv2;
using Menu = LevelEditor.Ui.Menu;
using LevelEditor.Sound;

namespace LevelEditor.Screen
{
    sealed class PauseScreen : IScreen
    {
        private OptionsMenu mOptionsScreen;


        private List<UIv2.Menu> mMenuList;
        private SpriteBatch mSpriteBatch;
        private Texture2D mBackgroundImageOptions;

        private SoundEffect mClickSound;

        private GraphicsDevice Device { get; set; }

        private HudScreen mHudScreen;

        private GraphicsDevice mGraphicsDevice;

        private int mScreenWidth;
        private int mScreenHeight;

        private Texture2D texture;

        // private SpriteFont heading1;

        public ScreenManager ScreenManager { get; set; }
        public SoundManager SoundManager { get; set; }
        public bool IsVisible { get; set; }

        public PauseScreen(HudScreen screen)
        {
            mHudScreen = screen;
        }

        public void LoadContent(GraphicsDeviceManager deviceManager, ContentManager contentManager, int windowWidth, int windowHeight)
        {

            mScreenWidth = windowWidth;
            mScreenHeight = windowHeight;

            //Load SoundEffects
            mClickSound = contentManager.Load<SoundEffect>("Audio/click2");

            Device = deviceManager.GraphicsDevice;
            mSpriteBatch = new SpriteBatch(deviceManager.GraphicsDevice);

            mOptionsScreen = new OptionsMenu(true);

            // Unlock the framerate. Later we want to remove this piece of code
            mGraphicsDevice = deviceManager.GraphicsDevice;
            mSpriteBatch = new SpriteBatch(deviceManager.GraphicsDevice);

            // Load font
            var font = contentManager.Load<SpriteFont>("Font");
            var subHeaderFont = contentManager.Load<SpriteFont>("SubHeaderFont");
            IsVisible = true;

            // Load background image
            mBackgroundImageOptions = contentManager.Load<Texture2D>("forest");

            mMenuList = new List<UIv2.Menu>();

            var menu = new UIv2.Menu(mGraphicsDevice, 5, 5, 90, 90);
            menu.WithBackground(Menu.CreateTexture2D(deviceManager.GraphicsDevice, 50, 30, pixel => new Color(0.0f, 0.0f, 0.0f, 0.2f)), 5, 5, 90, 90);
            mMenuList.Add(menu);

            Texture2D texture2D = Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.Black);
            Texture2D texture2DSliderPoint = Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.White);

            var heading = new UIv2.Components.Label(mGraphicsDevice, 35, 5, 30, 15, "Pause", subHeaderFont, Color.White);
            heading.AddTo(menu);

            // Create new Resume Button

            var resumeButton = new UIv2.Components.Button(mGraphicsDevice, 40, 20, 20, 7, texture2D, "Resume", font, Color.White);
            resumeButton.AddTo(menu);
            resumeButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                mHudScreen.IsVisible = true;
                ScreenManager.Remove(this);
                IsVisible = false;
            });

            // Create new Save Button
            var saveButton = new UIv2.Components.Button(mGraphicsDevice, 40, 30, 20, 7, texture2D, "Save Game", font, Color.White);
            saveButton.AddTo(menu);
            saveButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
            });

            // Create new Load Game Button
            var loadButton = new UIv2.Components.Button(mGraphicsDevice, 40, 40, 20, 7, texture2D, "Load Checkpoint", font, Color.White);
            loadButton.AddTo(menu);
            loadButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                // ScreenManager.Remove(this);
                // var hudScreen = new HudScreen("..\\..\\..\\..\\Content\\level.lvl");
                // ScreenManager.Add(hudScreen);          
                // IsVisible = false;

            });
            
            // Create new Options Button
            var optionsButton = new UIv2.Components.Button(mGraphicsDevice, 40, 50, 20, 7, texture2D, "Options", font, Color.White);
            optionsButton.AddTo(menu);
            optionsButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                ScreenManager.Add(mOptionsScreen);
                IsVisible = false;

            });

            // Create new Back Button
            var backButton = new UIv2.Components.Button(mGraphicsDevice, 40, 60, 20, 7, texture2D, "Main Menu", font, Color.White);
            backButton.AddTo(menu);
            backButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                ScreenManager.Remove(mHudScreen);
                ScreenManager.Remove(this);
                IsVisible = false;

            });

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

        public void Render(GameTime time)
        {
            if (!IsVisible)
            {
                return;
            }
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackgroundImageOptions, new Rectangle(0, 0, mScreenWidth, mScreenHeight), Color.White);
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
    }
}