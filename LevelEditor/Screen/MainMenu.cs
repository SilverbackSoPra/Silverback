using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using LevelEditor.UIv2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LevelEditor.Sound;
using Microsoft.Xna.Framework.Audio;

namespace LevelEditor.Screen

{

    internal sealed class MainMenuScreen : IScreen

    {
        private List<UIv2.Menu> mMenuList;

        private readonly string mDefaultDirectoryPath = $"..\\..\\..\\..\\Content";
        
        private OptionsMenu mOptionsScreen;
        private StatisticsScreen mStatisticsScreen;
        private EditorHudScreen mEditorHudScreen;
        private AchievementsScreen mAchievementsScreen;
        private CreditsScreen mCreditsScreen;
        private ScreamScreen mScreamScreen;

        private SpriteBatch mSpriteBatch;

        public ScreenManager ScreenManager { get; set; }
        public SoundManager SoundManager { get; set; }

        public bool IsVisible { get; set; }
        public SoundEffect Content { get; private set; }

        private Texture2D mBackgroundImage;

        private int mScreenWidth;
        private int mScreenHeight;

        private GraphicsDevice mGraphicsDevice;

        private SoundEffect mClickSound;
        private SoundEffect mMainMenuMusic;

        public void LoadContent(GraphicsDeviceManager deviceManager, ContentManager contentManager, int windowWidth, int windowHeight)
        {

            mScreenWidth = windowWidth;
            mScreenHeight = windowHeight;

            //Load SoundEffects
            mClickSound = contentManager.Load<SoundEffect>("Audio/click2");
            mMainMenuMusic = contentManager.Load<SoundEffect>("Audio/slowmo");

            SoundManager.AddMusic(mMainMenuMusic);

            // We should remove this for a more stable framerate later
            deviceManager.SynchronizeWithVerticalRetrace = false;
            deviceManager.ApplyChanges();

            mMenuList = new List<UIv2.Menu>();

            IsVisible = true;

            mGraphicsDevice = deviceManager.GraphicsDevice;

            mSpriteBatch = new SpriteBatch(deviceManager.GraphicsDevice);

            // Load font
            var font = contentManager.Load<SpriteFont>("Font");
            var headerFont = contentManager.Load<SpriteFont>("Heading");
            var buttonTexture = contentManager.Load<Texture2D>("button");
            IsVisible = true;

            // Load background image
            mBackgroundImage = contentManager.Load<Texture2D>("Forest");

            IsVisible = true;
            
            mOptionsScreen = new OptionsMenu(false);
            mStatisticsScreen = new StatisticsScreen();
            mCreditsScreen = new CreditsScreen();
            mEditorHudScreen = new EditorHudScreen();
            mAchievementsScreen = new AchievementsScreen();
            mScreamScreen = new ScreamScreen();

            var menu = new UIv2.Menu(mGraphicsDevice, 0, 0, 100, 100);
            mMenuList.Add(menu);

            var texture2D = UIv2.Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.Black);
            
            var heading = new UIv2.Components.Label(mGraphicsDevice, 10, 0, 80, 30, "Silverback", headerFont, Color.DarkSlateGray);
            heading.FontType = FontManager.FontType.Heading;
            heading.AddTo(menu);

            var newGameButton = new UIv2.Components.Button(mGraphicsDevice, 35, 24, 30, 7, texture2D, "New Game", font, Color.White);
            newGameButton.AddTo(menu);
            newGameButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                Statistic.CurrentSaveTime = 0;
                var loadingScreen = new LoadingScreen("..\\..\\..\\..\\Content\\tutorial.lvl");
                ScreenManager.Add(loadingScreen);
                IsVisible = false;
            });

            var loadGameButton = new UIv2.Components.Button(mGraphicsDevice, 35, 32, 30, 7, texture2D, "Load Game", font, Color.White);
            loadGameButton.AddTo(menu);
            // Add an event listener to the button
            loadGameButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);

                if (!File.Exists(PauseScreen.GetSavedGamesPath() + "\\Camera.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\CapuchinPositions.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\ChimpPositions.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\GibbonPositions.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\HudScreen.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\Huts.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\Level.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\OrangPositions.xml") ||
                    !File.Exists(PauseScreen.GetSavedGamesPath() + "\\Silverback.xml"))
                {
                    return;
                }

                // Get the level file

                using (var fs = File.OpenRead(PauseScreen.GetSavedGamesPath() + "\\HudScreen.xml"))
                {
                    var serializer = new XmlSerializer(typeof(HudScreen));
                    HudScreen hudScreen = null;
                    try
                    {
                        hudScreen = (HudScreen) serializer.Deserialize(fs);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.GetBaseException());
                    }

                    if (hudScreen != null)
                    {
                        var loadingScreen = new LoadingScreen(hudScreen.mLevelPath);
                        loadingScreen.LoadSaveGame();
                        ScreenManager.Add(loadingScreen);
                    }

                    IsVisible = false;
                }

            });

            // Create Statistic button
            var statisticsButton = new UIv2.Components.Button(mGraphicsDevice, 35, 40, 30, 7, texture2D, "Statistics", font, Color.White);
            statisticsButton.AddTo(menu);
            // Add an event listener to the button
            statisticsButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {

                SoundManager.AddSound(mClickSound);
                ScreenManager.Add(mStatisticsScreen);
                IsVisible = false;

            });

            // Create Achievements button
            var achievementsButton = new UIv2.Components.Button(mGraphicsDevice, 35, 48, 30, 7, texture2D, "Achievements", font, Color.White);
            achievementsButton.AddTo(menu);
            // Add an event listener to the button
            achievementsButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                ScreenManager.Add(mAchievementsScreen);
                IsVisible = false;

            });

            // Create Credits button
            var creditsButton = new UIv2.Components.Button(mGraphicsDevice, 35, 56, 30, 7, texture2D, "Credits", font, Color.White);
            creditsButton.AddTo(menu);
            // Add an event listener to the button
            creditsButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                // ScreenManager.Add(mScreamScreen);
                ScreenManager.Add(mCreditsScreen);
                IsVisible = false;

            });

            // Create Option button
            var optionsButton = new UIv2.Components.Button(mGraphicsDevice, 35, 64, 30, 7, texture2D, "Options", font, Color.White);
            optionsButton.AddTo(menu);
            // Add an event listener to the button
            optionsButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                ScreenManager.Add(mOptionsScreen);
                IsVisible = false;

            });

            var editorButton = new UIv2.Components.Button(mGraphicsDevice, 35, 72, 30, 7, texture2D, "Editor", font, Color.White);
            editorButton.AddTo(menu);
            editorButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                ScreenManager.Add(mEditorHudScreen);
                IsVisible = false;

            });
            
            var exitButton = new UIv2.Components.Button(mGraphicsDevice, 35, 80, 30, 7, texture2D, "Exit", font, Color.White);
            exitButton.AddTo(menu);
            exitButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                ScreenManager.Remove(this);

            });

            
        }

        public void UnloadContent()
        { }


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
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackgroundImage, new Rectangle(0, 0, mScreenWidth, mScreenHeight), Color.White);
            mSpriteBatch.End();
            foreach (var menu in mMenuList)
            {
                menu.Render(mSpriteBatch);
            }
        }
        
        public void ChangeWindowSize(int width, int height)
        {
            mScreenWidth = width;
            mScreenHeight = height;

            if (mMenuList == null)
            {
                return;
            }

            foreach (var menu in mMenuList)
            {
                menu.UpdateResolution(width, height);
            }
        }
        
        public void ChangeRenderingResolution(int width, int height)
        {
        }

    }

}

