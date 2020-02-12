using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LevelEditor.Sound;
using LevelEditor.UIv2;
using Microsoft.Xna.Framework.Audio;



namespace LevelEditor.Screen
{
    class ScreamScreen : IScreen
    {
        private List<UIv2.Menu> mMenuList;
        public ScreenManager ScreenManager { get; set; }
        public SoundManager SoundManager { get; set; }
        public bool IsVisible { get; set; }

        private int mScreenWidth;
        private int mScreenHeight;

        private SpriteBatch mSpriteBatch;
        private Texture2D mBackgroundImageStatistics;
        private GraphicsDevice mGraphicsDevice;
        private Vector2 mTotalWindowSize;
        private MainMenuScreen mMainMenuScreen;

        private SoundEffect mClickSound;
        private SoundEffect mScream;

        public void LoadContent(GraphicsDeviceManager deviceManager, ContentManager contentManager, int windowWidth, int windowHeight)
        {


            mScreenWidth = windowWidth;
            mScreenHeight = windowHeight;

            //Load SoundEffects
            mClickSound = contentManager.Load<SoundEffect>("Audio/click2");
            mScream = contentManager.Load<SoundEffect>("Audio/Gibbons_singen");


            mGraphicsDevice = deviceManager.GraphicsDevice;
            mSpriteBatch = new SpriteBatch(deviceManager.GraphicsDevice);
            mTotalWindowSize = new Vector2(mGraphicsDevice.Viewport.Width, mGraphicsDevice.Viewport.Height);
            
            // Load font
            var font = contentManager.Load<SpriteFont>("Font");
            var subHeaderFont = contentManager.Load<SpriteFont>("SubHeaderFont");
            IsVisible = true;

            // Load background image
            mBackgroundImageStatistics = contentManager.Load<Texture2D>("scare");
            IsVisible = true;

            // Play Scream
            SoundManager.AddSound(mScream);

            mMenuList = new List<UIv2.Menu>();

            Texture2D texture2D = UIv2.Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.Black);

            // Instantiate a new menu
            var menu = new UIv2.Menu(deviceManager.GraphicsDevice, 5, 5, 90, 90);

            // Create the Back Button
            var backButton = new UIv2.Components.Button(mGraphicsDevice, 40, 80, 20, 7, texture2D, "Back", font, Color.White);
            backButton.AddTo(menu);
            backButton.AddListener(System.Windows.Forms.MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                ScreenManager.Remove(this);
                mScream.Dispose();
                IsVisible = false;

            });

            mMenuList.Add(menu);

            mMainMenuScreen = new MainMenuScreen();

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
            mSpriteBatch.Draw(mBackgroundImageStatistics, new Rectangle(0, 0, mScreenWidth, mScreenHeight), Color.White);
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
