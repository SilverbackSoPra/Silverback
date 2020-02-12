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
    internal sealed class AchievementsScreen : IScreen
    {
        private List<UIv2.Menu> mMenuList;
       //  private EditorScreen mEditorScreen;
        public ScreenManager ScreenManager { get; set; }

        private GraphicsDevice mGraphicsDevice;

        private SpriteBatch mSpriteBatch;
        private Texture2D mBackgroundImageAchievements;
        private Vector2 mTotalWindowSize;
        private MainMenuScreen mMainMenuScreen;

        public SoundManager SoundManager { get; set; }
        private SoundEffect mClickSound;

        private int mScreenWidth;
        private int mScreenHeight;
        public bool IsVisible { get; set; }

        public void LoadContent(GraphicsDeviceManager deviceManager, ContentManager contentManager, int windowWidth, int windowHeight)
        {

            mScreenWidth = windowWidth;
            mScreenHeight = windowHeight;

            //Load SoundEffects
            mClickSound = contentManager.Load<SoundEffect>("Audio/click2");

            // Unlock the framerate. Later we want to remove this piece of code
            // deviceManager.SynchronizeWithVerticalRetrace = false;
            // deviceManager.ApplyChanges();

            mMenuList = new List<UIv2.Menu>();

            // Load font
            var font = contentManager.Load<SpriteFont>("Font");
            var subHeaderFont = contentManager.Load<SpriteFont>("SubHeaderFont");
            IsVisible = true;

            mGraphicsDevice = deviceManager.GraphicsDevice;
            mTotalWindowSize = new Vector2(mGraphicsDevice.Viewport.Width, mGraphicsDevice.Viewport.Height);

            mSpriteBatch = new SpriteBatch(mGraphicsDevice);

            // Load background image
            mBackgroundImageAchievements = contentManager.Load<Texture2D>("forest");
            IsVisible = true;

            // Instantiate a new menu
            // var windowWidth = deviceManager.GraphicsDevice.Viewport.Width;
            // var windowHeight = deviceManager.GraphicsDevice.Viewport.Height;

            // mEditorScreen = new EditorScreen();
            // ScreenManager.Add(mEditorScreen);

            Texture2D texture2D = UIv2.Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.Black);
            Texture2D Checked = UIv2.Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.DarkOliveGreen);

            // Instantiate a new menu
            var menu = new UIv2.Menu(deviceManager.GraphicsDevice, 5, 5, 90, 90);
            menu.WithBackground(UIv2.Menu.CreateTexture2D(mGraphicsDevice, (int)mTotalWindowSize.X, (int)mTotalWindowSize.Y, pixel => new Color(0.0f, 0.0f, 0.0f, 0.2f)), 5, 5, 90, 90);

            var screenName = new UIv2.Components.Label(deviceManager.GraphicsDevice, 35, 5, 30, 15, "Achievements", subHeaderFont, Color.White);
            screenName.FontType = FontManager.FontType.Subheading;
            screenName.AddTo(menu);


            // “Gamemaster”: Beende das Spiel erfolgreich.
            var gamemasterString = "Gamemaster: You've played through the whole game. Nice work!";
            var gamemasterButton = new UIv2.Components.CheckedButton(mGraphicsDevice, 15, 25, 70, 7, texture2D, Checked, gamemasterString, font, Color.White, Achievements.Gamemaster);
            gamemasterButton.AddTo(menu);


            // “Lumberjack’s nightmare”: Vertreibe 42 Holzfäller.
            var lumberjackString = "Lumberjack's nightmare: You've made 42 lumberjacks run away. Awesome!";
            var lumberjackLabel = new UIv2.Components.CheckedButton(mGraphicsDevice, 15, 35, 70, 7, texture2D, Checked, lumberjackString, font, Color.White, Achievements.LumberjacksNightmare);
            lumberjackLabel.AddTo(menu);


            // “Speed runner”: Beende das Spiel innerhalb von 60 Minuten.
            var speedyString = "Speed runner: You've finished the whole game within 1 hour. Speedy Gonzales huh?!";
            var speedyLabel = new UIv2.Components.CheckedButton(mGraphicsDevice, 15, 45, 70, 7, texture2D, Checked, speedyString, font, Color.White, Achievements.Speedrunner);
            speedyLabel.AddTo(menu);


            // “Redundancy”: Erhalte ein Achievement.
            var redundancyString = "Redundancy: An achievement is officially yours. Hopefully you're happy with it!";
            var redundancyLabel = new UIv2.Components.CheckedButton(mGraphicsDevice, 15, 55, 70, 7, texture2D, Checked, redundancyString, font, Color.White, Achievements.Redundancy);
            redundancyLabel.AddTo(menu);


            // “Tribute to the creators”: Credits angeschaut.
            var creditsString = "Tribute to the creators: You've had a look at the credits. Thanks man!";
            var creditsLabel = new UIv2.Components.CheckedButton(mGraphicsDevice, 15, 65, 70, 7, texture2D, Checked, creditsString, font, Color.White, Achievements.TributeToTheCreators);
            creditsLabel.AddTo(menu);

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
            mSpriteBatch.Draw(mBackgroundImageAchievements, new Rectangle(0, 0, mScreenWidth, mScreenHeight), Color.White);
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
