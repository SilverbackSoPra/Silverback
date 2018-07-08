using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using LevelEditor.Engine.Mesh;
using LevelEditor.Engine;
using LevelEditor.Events;
using LevelEditor.Objects;
using LevelEditor.Objects.Ape.SubApe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Menu = LevelEditor.Ui.Menu;
using LevelEditor.Sound;
using LevelEditor.UIv2;
using Microsoft.Xna.Framework.Audio;
using LevelEditor.Objects.Ape;
using LevelEditor.UI.Components;

namespace LevelEditor.Screen
{
    sealed class HudScreen : IScreen
    {
        private List<UIv2.Menu> mMenuList;

        private EditorScreen mHudScreen;
        private GameScreen mGameScreen;
        private PauseScreen mPauseScreen;

        private WinScreen mWinScreen;
        private LoseScreen mLoseScreen;

        private ActorBatch mActorBatch;
        private SpriteFont mFont2;

        private UIv2.Components.Button mHighlitedButton;

        private int mScreenWidth;
        private int mScreenHeight;

        private int mCounter1 = 5;

        private bool mHighlighted;

        private GraphicsDevice mGraphicsDevice;

        private SpriteBatch mSpriteBatch;

        private readonly string mLevelPath;

        private SoundEffect mClickSound;

        private static int sHealthpoints;

        private double mHudStartTime;

        private UIv2.Components.Label mContinueLabel;

        private bool mWasVisible = true;

        private Texture2D mButtonTexture;
        private Texture2D mHighlightedButtonTexture;

        public ScreenManager ScreenManager { get; set; }
        public SoundManager SoundManager { get; set; }
        public bool IsVisible { get; set; }

        private UIv2.Components.Label mTimeLabel;

        public static UIv2.Components.Label mSilverbackPosXLabel;
        public static UIv2.Components.Label mSilverbackPosYLabel;

        public void LoadContent(GraphicsDeviceManager deviceManager,
            ContentManager contentManager,
            int windowWidth,
            int windowHeight)
        {

            mScreenWidth = windowWidth;
            mScreenHeight = windowHeight;

            //Load SoundEffects
            mClickSound = contentManager.Load<SoundEffect>("Audio/click2");

            mGraphicsDevice = deviceManager.GraphicsDevice;

            mPauseScreen = new PauseScreen(this);

            mWinScreen = new WinScreen(this);
            mLoseScreen = new LoseScreen(this);

            mGameScreen = new GameScreen(mLevelPath);

            ScreenManager.Add(mGameScreen);

            mSpriteBatch = new SpriteBatch(mGraphicsDevice);

            // Load font
            var font = contentManager.Load<SpriteFont>("Font");
            var subHeaderFont = contentManager.Load<SpriteFont>("SubHeaderFont");
            IsVisible = true;

            // Instantiate a new menu
            mMenuList = new List<UIv2.Menu>();

            mButtonTexture = Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.Black);
            var textureTransparent = UIv2.Menu.CreateTexture2D(mGraphicsDevice, 10, 10, pixel => Color.Transparent);

            //Highlited Button
            mHighlightedButtonTexture = Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.Orange);

            var menu = new UIv2.Menu(mGraphicsDevice, 0, 95, 100, 5);
            menu.WithBackground(Menu.CreateTexture2D(deviceManager.GraphicsDevice,
                windowWidth,
                200,
                pixel => new Color(0.0f, 0.0f, 0.0f, 0.15f)),
                0,
                0,
                100,
                100);
            mMenuList.Add(menu);

            var counterMenu = new UIv2.Menu(mGraphicsDevice, 40, 15, 20, 4);
            counterMenu.WithBackground(Menu.CreateTexture2D(deviceManager.GraphicsDevice,
                windowWidth,
                200,
                pixel => new Color(0.0f, 0.0f, 0.0f, 0.15f)),
                0,
                0,
                100,
                100);
            counterMenu.NonSolid();
            mMenuList.Add(counterMenu);

            var counterLabel = new UIv2.Components.Label(mGraphicsDevice, 0, 0, 100, 100, "Available apes to spawn: " + mCounter1, font, Color.White);
            counterLabel.AddTo(counterMenu);

            var pauseButton =
                new UIv2.Components.Button(mGraphicsDevice, 90, 0, 7, 100, mButtonTexture, "Pause", font, Color.White);
            pauseButton.AddTo(menu);
            pauseButton.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    SoundManager.AddSound(mClickSound);
                    ScreenManager.Add(mPauseScreen);
                    mGameScreen.mCameraHandler.Lock();
                    IsVisible = false;
                    mWasVisible = false;
                    mGameScreen.IsVisible = false;
                });

            mTimeLabel = new UIv2.Components.Label(mGraphicsDevice,
                0,
                0,
                10,
                100,
                "",
                font,
                Color.White);
            mTimeLabel.AddTo(menu);

            var levelLabel = new UIv2.Components.Label(mGraphicsDevice,
                10,
                0,
                7,
                100,
                "Level: 1",
                font,
                Color.White);
            levelLabel.AddTo(menu);

            // Create selection button for Baboon
            var ape1Button =
                new UIv2.Components.Button(mGraphicsDevice, 30, 0, 7, 100, mButtonTexture, "Baboon", font, Color.White);
            ape1Button.AddTo(menu);
            ape1Button.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    SoundManager.AddSound(mClickSound);

                    if (mCounter1 == 0)
                    {
                        UnHiglightSelectedActorBatch();
                        mActorBatch = mGameScreen.mLevel.mBaboonBatch;
                        mHighlitedButton = ape1Button;
                        HighlightSelectedActorBatch();
                    }
                    else if (mCounter1 > 0)
                    {
                        mCounter1--;
                        counterLabel.Text = "Available apes to spawn: " + mCounter1;
                        var rand = new Random();
                        var mbaboon = new Baboon(mGameScreen.mLevel.mBaboonMesh,
                            mGameScreen.mLevel.mTerrain,
                            mGameScreen.mLevel.mSilverback,
                            mGameScreen.mLevel.mQuadTree,
                            ref rand);
                        mbaboon.mScene = mGameScreen.mLevel;
                        mGameScreen.mLevel.Add(mbaboon);
                    }

                    if (mCounter1 == 0)
                    {
                        counterMenu.mIsVisible = false;
                    }

                });

            // Create selection button for Capuchin
            var ape2Button = new UIv2.Components.Button(mGraphicsDevice,
                41,
                0,
                7,
                100,
                mButtonTexture,
                "Capuchin",
                font,
                Color.White);
            ape2Button.AddTo(menu);
            ape2Button.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    SoundManager.AddSound(mClickSound);

                    if (mCounter1 == 0)
                    {
                        UnHiglightSelectedActorBatch();
                        mActorBatch = mGameScreen.mLevel.mCapuchinBatch;
                        mHighlitedButton = ape2Button;
                        HighlightSelectedActorBatch();                        
                    }
                    else if (mCounter1 > 0)
                    {
                        mCounter1--;
                        counterLabel.Text = "Available apes to spawn: " + mCounter1;
                        var rand = new Random();
                        var mcapuchin = new Capuchin(mGameScreen.mLevel.mCapuchinMesh,
                            mGameScreen.mLevel.mTerrain,
                            mGameScreen.mLevel.mSilverback,
                            mGameScreen.mLevel,
                            ref rand);
                        mGameScreen.mLevel.Add(mcapuchin);
                    }

                    if (mCounter1 == 0)
                    {
                        counterMenu.mIsVisible = false;
                    }

                });

            // Create selection button for Chimpaneeze
            var ape3Button =
                new UIv2.Components.Button(mGraphicsDevice, 52, 0, 7, 100, mButtonTexture, "Chimp", font, Color.White);
            ape3Button.AddTo(menu);

            ape3Button.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    SoundManager.AddSound(mClickSound);

                    if (mCounter1 == 0)
                    {
                        UnHiglightSelectedActorBatch();
                        mActorBatch = mGameScreen.mLevel.mChimpanezzeBatch;
                        mHighlitedButton = ape3Button;
                        HighlightSelectedActorBatch();
                    }
                    else if (mCounter1 > 0)
                    {
                        mCounter1--;
                        counterLabel.Text = "Available apes to spawn: " + mCounter1;
                        var rand = new Random();
                        var mchimpanezee = new Chimpanezee(mGameScreen.mLevel.mChimpanezzeMesh,
                            mGameScreen.mLevel.mTerrain,
                            mGameScreen.mLevel.mSilverback,
                            mGameScreen.mLevel,
                            ref rand);
                        mGameScreen.mLevel.Add(mchimpanezee);
                    }

                    if (mCounter1 == 0)
                    {
                        counterMenu.mIsVisible = false;
                    }

                }
                );

            // Create selection button for Gibbon
            var ape4Button =
                new UIv2.Components.Button(mGraphicsDevice, 63, 0, 7, 100, mButtonTexture, "Gibbon", font, Color.White);
            ape4Button.AddTo(menu);
            ape4Button.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    SoundManager.AddSound(mClickSound);

                    if (mCounter1 == 0)
                    {
                        UnHiglightSelectedActorBatch();
                        mActorBatch = mGameScreen.mLevel.mGibbonBatch;
                        mHighlitedButton = ape4Button;
                        HighlightSelectedActorBatch();
                    }
                    else if (mCounter1 > 0)
                    {
                        mCounter1--;
                        counterLabel.Text = "Available apes to spawn: " + mCounter1;
                        var rand = new Random();
                        var mgibbon = new Gibbon(mGameScreen.mLevel.mGibbonMesh,
                            mGameScreen.mLevel.mTerrain,
                            mGameScreen.mLevel.mSilverback,
                            mGameScreen.mLevel,
                            ref rand);
                        mGameScreen.mLevel.Add(mgibbon);
                    }

                    if (mCounter1 == 0)
                    {
                        counterMenu.mIsVisible = false;
                    }

                });

            /*
            // Create button for attack
            var attack =
                new UIv2.Components.Button(mGraphicsDevice, 62, 0, 8, 100, mButtonTexture, "Attack", font, Color.White);
            attack.AddTo(menu);
            attack.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {

                    SoundManager.AddSound(mClickSound);
                    foreach (var actorbatch in mGameScreen.mLevel.mActorBatches)
                    {

                        foreach (var actor in actorbatch.mActors)
                        {
                            if (actor.IActor is IAttacker)
                            {
                                var attacker = (IAttacker) actor.IActor;
                                attacker.Attack();
                            }

                        }

                    }

                    mGameScreen.mLevel.mSilverback.Attack();
                });


            // Following 2 buttons will be deleted at the end
            // Create button for win simulation
            var win =
                new UIv2.Components.Button(mGraphicsDevice, 71, 50, 4, 50, mButtonTexture, "Win", font, Color.White);
            win.AddTo(menu);
            win.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    ScreenManager.Add(mWinScreen);
                    Statistic.Win++;
                    IsVisible = false;
                    mGameScreen.IsVisible = false;
                    SoundManager.AddSound(mClickSound);
                });

            // Create button for lose simulation
            var lose =
                new UIv2.Components.Button(mGraphicsDevice, 76, 50, 4, 50, mButtonTexture, "Lose", font, Color.White);
            lose.AddTo(menu);
            lose.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    ScreenManager.Add(mLoseScreen);
                    Statistic.Lost++;
                    IsVisible = false;
                    mGameScreen.IsVisible = false;
                    SoundManager.AddSound(mClickSound);
                });
            */
                        
            // Display the level text before the level starts
            if (mGameScreen.mLevel.mLevelStory != null)
            {
                mGameScreen.mCameraHandler.Lock();
                var textMenu = new UIv2.Menu(mGraphicsDevice, 5, 5, 90, 90);
                textMenu.WithBackground(Menu.CreateTexture2D(deviceManager.GraphicsDevice, 50, 30, pixel => new Color(0.0f, 0.0f, 0.0f, 0.15f)), 5, 5, 90, 90);
                var heading = new UIv2.Components.Label(mGraphicsDevice, 5, 5, 90, 15, mGameScreen.mLevel.mLevelTitle, subHeaderFont, Color.DarkSlateGray);
                heading.AddTo(textMenu);
                var textLabel = new UIv2.Components.Label(mGraphicsDevice, 25, 28, 50, 35, mGameScreen.mLevel.mLevelStory, font, Color.White);
                textLabel.mAutoBreak = true;
                textLabel.AddTo(textMenu);
                mContinueLabel = new UIv2.Components.Label(mGraphicsDevice, 25, 80, 50, 10, "Press the left mouse button to continue", font, Color.White);
                mContinueLabel.AddTo(textMenu);
                var textButton = new UIv2.Components.Button(mGraphicsDevice, 0, 0, 100, 100, textureTransparent, "", font, Color.White);
                textButton.AddTo(textMenu);
                textButton.AddListener(MouseButtons.Left,
                    InputState.Pressed,
                    () =>
                    {
                        menu.mIsVisible = true;
                        counterMenu.mIsVisible = true;
                        textMenu.mIsVisible = false;
                        mGameScreen.mCameraHandler.Unlock();
                    });
                mMenuList.Add(textMenu);
                menu.mIsVisible = false;
                counterMenu.mIsVisible = false;
            }

            mMenuList.Add(menu);

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


        public HudScreen(string levelPath)
        {
            mLevelPath = levelPath;
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

        public void UnloadContent()
        {
            ScreenManager.Remove(mGameScreen);
        }

        public void Update(GameTime time)
        {

            if (IsVisible && !mWasVisible)
            {
                mWasVisible = true;
                mGameScreen.mCameraHandler.Unlock();
            }

            if (mGameScreen.IsVisible)
            {
                IsVisible = true;
            }            

            if (mHudStartTime == 0)
            {
                mHudStartTime = time.TotalGameTime.TotalMilliseconds;
            }

            var hudTime = time.TotalGameTime.TotalMilliseconds - mHudStartTime;
            var timeSpan = TimeSpan.FromMilliseconds(hudTime);
            mTimeLabel.Text = "Time: " + timeSpan.Minutes + ":" + timeSpan.Seconds;

            if (mGameScreen.mLevel.mLevelStory != null)
            {
                mContinueLabel.mTextColor = Color.White *
                                            (float) (Math.Sin(time.TotalGameTime.TotalMilliseconds / 200.0f) * 0.5f +
                                                     0.5f);
            }

            // Iterate over all Menu elements
            foreach (var menu in mMenuList)
            {
                if (!menu.mIsVisible)
                {
                    continue;
                }
                menu.CheckRegisteredEvents();
            }

            if (mActorBatch != null && InputManager.MouseRightButtonPressed())
            {
                                       
            
                var raycast = RayCasting.CalculateMouseRayTerrainIntersection(mGameScreen.mViewport, mGameScreen.mCamera, mGameScreen.mLevel.mTerrain);
            
                if (!raycast.mIntersected)
                {
                    return;
                }

                foreach (var actor in mActorBatch.mActors)
                {
                    var movable = (IMoveable)actor.IActor;
                    movable.SetTarget(raycast.mLocation);
                }

            }

            // Health Points Range
            sHealthpoints = MathHelper.Clamp(sHealthpoints, 0, 5000);

            // Getting HP of Silverback
            var currentHP = mGameScreen.mLevel.mSilverback.HP;

            // Triggering lose screen when Silverbacks HP == 0
            if (currentHP == 0)
            {
            ScreenManager.Add(mLoseScreen);
            Statistic.Lost++;
            IsVisible = false;
            mGameScreen.IsVisible = false;
            }

            // TODO: Triggering win screen...
        }
        public void HighlightSelectedActorBatch()
        {

            if (mActorBatch == null)
            {
                return;
            }

            foreach(var actor in mActorBatch.mActors)
            {
                actor.Color = new Vector3(1.0f, 0.65f, 0.0f);
            }
            if (mHighlitedButton == null)
            {
                return;
            }
            mHighlitedButton.mTexture = mHighlightedButtonTexture;
        }

        public void UnHiglightSelectedActorBatch()
        {

            if (mActorBatch == null)
            {
                return;
            }

            foreach (var actor in mActorBatch.mActors)
            {
                actor.Color = new Vector3(1.0f, 1.0f, 1.0f);
            }
            if (mHighlitedButton == null)
            {
                return;
            }
            mHighlitedButton.mTexture = mButtonTexture;
        }

    }
}
