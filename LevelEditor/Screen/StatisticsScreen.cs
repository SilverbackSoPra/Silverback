using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LevelEditor.Sound;
using LevelEditor.UIv2;
using Microsoft.Xna.Framework.Audio;

namespace LevelEditor.Screen
{
    internal sealed class StatisticsScreen : IScreen
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
            IsVisible = true;

            // Load background image
            mBackgroundImageStatistics = contentManager.Load<Texture2D>("forest");
            IsVisible = true;

            mMenuList = new List<UIv2.Menu>();

            Texture2D texture2D = UIv2.Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.Black);

            // Instantiate a new menu
            var menu = new UIv2.Menu(deviceManager.GraphicsDevice, 5, 5, 90, 90);
            menu.WithBackground(UIv2.Menu.CreateTexture2D(mGraphicsDevice, (int)mTotalWindowSize.X, (int)mTotalWindowSize.Y, pixel => new Color(0.0f, 0.0f, 0.0f, 0.2f)), 5, 5, 90, 90);

            var heading = new UIv2.Components.Label(mGraphicsDevice, 35, 5, 30, 15, "Statistics", subHeaderFont, Color.White);
            heading.FontType = FontManager.FontType.Subheading;
            heading.AddTo(menu);

            var time = TimeSpan.FromMilliseconds(Statistic.Time);

            // Gesamtspielzeit
            var timeString = "Total playing time: " + time.Days + " days " + time.Hours + " hours " + time.Minutes + " minutes " + time.Seconds + " seconds";
            var totalPlayingTime = new UIv2.Components.Label(mGraphicsDevice, 30, 20, 40, 7, timeString, font, Color.White);
            totalPlayingTime.AddTo(menu);

            
            // Escaped Mit-Primaten beim Durchspielen
            var fledPrimatesString = "Total amount of primates fled: " + Statistic.EscapedApes;
            var fledPrimatesLabel = new UIv2.Components.Label(mGraphicsDevice, 30, 30, 40, 7, fledPrimatesString, font, Color.White);
            fledPrimatesLabel.AddTo(menu);

            
            // Escaped Holzfäller
            var lumberjacksFledString = "Total number of lumberjacks defeated: " + Statistic.EscapedLumberjacks;
            var lumberjacksFledLabel = new UIv2.Components.Label(mGraphicsDevice, 30, 40, 40, 7, lumberjacksFledString, font, Color.White);
            lumberjacksFledLabel.AddTo(menu);

            
            // Wie oft hat man gewonnen / verloren
            var lostWonString = "Games won: " + Statistic.Win + "    " + "Games lost: " + Statistic.Lost;
            var lostWonLabel = new UIv2.Components.Label(mGraphicsDevice, 30, 50, 40, 7, lostWonString, font, Color.White);
            lostWonLabel.AddTo(menu);


            // Minimale Zeit zum Durchspielen
            time = TimeSpan.FromMilliseconds(Statistic.Time);

            // Gesamtspielzeit
            timeString = "Total playing time: " + time.Days + " days " + time.Hours + " hours " + time.Minutes + " minutes " + time.Seconds + " seconds";
            var minTimeString = Statistic.MinimalTime == 0 ? "Shortest time to finish the whole game: Not finished" : "Shortest time to finish the whole game: " + timeString;
            var minTimeLabel = new UIv2.Components.Label(mGraphicsDevice, 30, 60, 40, 7, minTimeString, font, Color.White);
            minTimeLabel.AddTo(menu);


            // Create the Back Button
            var backButton = new UIv2.Components.Button(mGraphicsDevice, 40, 80, 20, 7, texture2D, "Back", font, Color.White);
            backButton.AddTo(menu);
            backButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                ScreenManager.Remove(this);
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
