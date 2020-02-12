using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using LevelEditor.Engine.Mesh;
using LevelEditor.Events;
using LevelEditor.Objects;
using LevelEditor.Objects.Ape.SubApe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LevelEditor.Sound;
using LevelEditor.UIv2;
using Microsoft.Xna.Framework.Audio;
using Newtonsoft.Json;

namespace LevelEditor.Screen
{
    [Serializable()]
    public sealed class HudScreen : IScreen, ISaver, ILoader
    {
        private List<UIv2.Menu> mMenuList;

        [XmlIgnore]
        public GameScreen mGameScreen;
        private PauseScreen mPauseScreen;

        private WinScreen mWinScreen;
        private LoseScreen mLoseScreen;

        private ActorBatch mActorBatch;

        private UIv2.Components.Button mHighlitedButton;

        private int mScreenWidth;
        private int mScreenHeight;
        
        public int mTotalCunter;

        private bool mHighlighted;
        
        private GraphicsDevice mGraphicsDevice;

        private SpriteBatch mSpriteBatch;

        public string mLevelPath;

        private SoundEffect mClickSound;

        private double mHudStartTime;

        private UIv2.Components.Label mContinueLabel;

        private bool mWasVisible = true;

        private Texture2D mButtonTexture;
        private Texture2D mHighlightedButtonTexture;

        [XmlIgnore]
        public ScreenManager ScreenManager { get; set; }
        [XmlIgnore]
        public SoundManager SoundManager { get; set; }
        public bool IsVisible { get; set; }

        private UIv2.Components.Label mTimeLabel;
        private UIv2.Components.Label mHPLabel;
        
        [XmlIgnore]
        public static UIv2.Components.Label mSilverbackPosXLabel;
        [XmlIgnore]
        public static UIv2.Components.Label mSilverbackPosYLabel;
        
        public int mOrangCounter = 0;
        public int mCapuchinCounter = 0;
        public int mChimpCounter = 0;
        public int mGibbonCounter = 0;

        [XmlIgnore]
        public UIv2.Menu mCounterMenu;

        public Random mRandom = new Random();


        private UIv2.Components.Button mApe1Button;
        private UIv2.Components.Button mApe2Button;
        private UIv2.Components.Button mApe3Button;
        private UIv2.Components.Button mApe4Button;


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
            mTotalCunter = mGameScreen.mLevel.mSpawnablePrimatesCount;

            mSpriteBatch = new SpriteBatch(mGraphicsDevice);

            // Load font
            var font = contentManager.Load<SpriteFont>("Font");
            var subHeaderFont = contentManager.Load<SpriteFont>("SubHeaderFont");
            IsVisible = true;

            // Instantiate a new menu
            mMenuList = new List<UIv2.Menu>();

            mButtonTexture = UIv2.Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.Black);
            var textureTransparent = UIv2.Menu.CreateTexture2D(mGraphicsDevice, 10, 10, pixel => Color.Transparent);

            //Highlighted Button
            mHighlightedButtonTexture = UIv2.Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.Orange);

            var menu = new UIv2.Menu(mGraphicsDevice, 0, 95, 100, 5);
            menu.WithBackground(UIv2.Menu.CreateTexture2D(deviceManager.GraphicsDevice,
                windowWidth,
                200,
                pixel => new Color(0.0f, 0.0f, 0.0f, 0.15f)),
                0,
                0,
                100,
                100);
            mMenuList.Add(menu);

            mCounterMenu = new UIv2.Menu(mGraphicsDevice, 40, 15, 20, 4);
            mCounterMenu.WithBackground(UIv2.Menu.CreateTexture2D(deviceManager.GraphicsDevice,
                windowWidth,
                200,
                pixel => new Color(0.0f, 0.0f, 0.0f, 0.15f)),
                0,
                0,
                100,
                100);
            mCounterMenu.NonSolid();
            mMenuList.Add(mCounterMenu);

            var counterLabel = new UIv2.Components.Label(mGraphicsDevice, 0, 0, 100, 100, "Available apes to spawn: " + mGameScreen.mLevel.mSpawnablePrimatesCount, font, Color.White);
            counterLabel.AddTo(mCounterMenu);

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

            mHPLabel = new UIv2.Components.Label(mGraphicsDevice,
                0,
                0,
                45,
                100,
                "HP: ",
                font,
                Color.White);
            mHPLabel.AddTo(menu);

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
                "Level: Tutorial",
                font,
                Color.White);
            levelLabel.AddTo(menu);
            for (var i = 0; i < 0; i++)
            {
                
                var morang = new OrangUtan(mGameScreen.mLevel.mOrangUtanMesh,
                    mGameScreen.mLevel.mTerrain,
                    mGameScreen.mLevel.mSilverback,
                    mGameScreen.mLevel,
                    ref mRandom);
                //morang.mScene = mGameScreen.mLevel;
                mGameScreen.mLevel.Add(morang);
            }

            // Create selection button for Baboon
            mApe1Button =
                new UIv2.Components.Button(mGraphicsDevice, 30, 0, 7, 100, mButtonTexture, "Orang", font, Color.White);
            mApe1Button.AddTo(menu);
            mApe1Button.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    SoundManager.AddSound(mClickSound);

                    if (mGameScreen.mLevel.mSpawnablePrimatesCount == 0)
                    {
                        if (mGameScreen.mLevel.mOrangutanBatch.mActors.Count > 0)
                        {
                            UnHiglightSelectedActorBatch();
                            mActorBatch = mGameScreen.mLevel.mOrangutanBatch;
                            mHighlitedButton = mApe1Button;
                            HighlightSelectedActorBatch();
                        }
                    }
                    else if (mGameScreen.mLevel.mSpawnablePrimatesCount > 0)
                    {
                        mGameScreen.mLevel.mSpawnablePrimatesCount--;
                        mOrangCounter++;
                        counterLabel.Text = "Available apes to spawn: " + mGameScreen.mLevel.mSpawnablePrimatesCount;

                        var morang = new OrangUtan(mGameScreen.mLevel.mOrangUtanMesh,
                            mGameScreen.mLevel.mTerrain,
                            mGameScreen.mLevel.mSilverback,
                            mGameScreen.mLevel,
                            ref mRandom);
                        //morang.mScene = mGameScreen.mLevel;
                        mGameScreen.mLevel.Add(morang);
                        LumberChoice.AddApe(2);
                    }

                    if (mGameScreen.mLevel.mSpawnablePrimatesCount == 0)
                    {
                        mCounterMenu.mIsVisible = false;
                        DisableUnusedApeSelectorButtons();
                    }

                });
            
            // Create selection button for Capuchin
            mApe2Button = new UIv2.Components.Button(mGraphicsDevice,
                41,
                0,
                7,
                100,
                mButtonTexture,
                "Capuchin",
                font,
                Color.White);
            mApe2Button.AddTo(menu);
            mApe2Button.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    SoundManager.AddSound(mClickSound);

                    if (mGameScreen.mLevel.mSpawnablePrimatesCount == 0)
                    {
                        if (mGameScreen.mLevel.mCapuchinBatch.mActors.Count > 0)
                        {
                            UnHiglightSelectedActorBatch();
                            mActorBatch = mGameScreen.mLevel.mCapuchinBatch;
                            mHighlitedButton = mApe2Button;
                            HighlightSelectedActorBatch();                        
                        }
                    }
                    else if (mGameScreen.mLevel.mSpawnablePrimatesCount > 0)
                    {
                        mCapuchinCounter++;
                        counterLabel.Text = "Available apes to spawn: " + mGameScreen.mLevel.mSpawnablePrimatesCount;
                        mGameScreen.mLevel.mSpawnablePrimatesCount--;
                        var mcapuchin = new Capuchin(mGameScreen.mLevel.mCapuchinMesh,
                            mGameScreen.mLevel.mTerrain,
                            mGameScreen.mLevel.mSilverback,
                            mGameScreen.mLevel,
                            ref mRandom);
                        mGameScreen.mLevel.Add(mcapuchin);
                        LumberChoice.AddApe(3);
                    }

                    if (mGameScreen.mLevel.mSpawnablePrimatesCount == 0)
                    {
                        mCounterMenu.mIsVisible = false;
                        DisableUnusedApeSelectorButtons();
                    }

                });

            // Create selection button for Chimpaneeze
            mApe3Button =
                new UIv2.Components.Button(mGraphicsDevice, 52, 0, 7, 100, mButtonTexture, "Chimp", font, Color.White);
            mApe3Button.AddTo(menu);

            mApe3Button.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    SoundManager.AddSound(mClickSound);

                    if (mGameScreen.mLevel.mSpawnablePrimatesCount == 0)
                    {
                        if (mGameScreen.mLevel.mChimpanezzeBatch.mActors.Count > 0)
                        {
                            UnHiglightSelectedActorBatch();
                            mActorBatch = mGameScreen.mLevel.mChimpanezzeBatch;
                            mHighlitedButton = mApe3Button;
                            HighlightSelectedActorBatch();
                        }
                    }
                    else if (mGameScreen.mLevel.mSpawnablePrimatesCount > 0)
                    {
                        mChimpCounter++;
                        mGameScreen.mLevel.mSpawnablePrimatesCount--;
                        counterLabel.Text = "Available apes to spawn: " + mGameScreen.mLevel.mSpawnablePrimatesCount;
                        var mchimpanezee = new Chimpanezee(mGameScreen.mLevel.mChimpanezzeMesh,
                            mGameScreen.mLevel.mTerrain,
                            mGameScreen.mLevel.mSilverback,
                            mGameScreen.mLevel,
                            ref mRandom);
                        mGameScreen.mLevel.Add(mchimpanezee);
                        LumberChoice.AddApe(1);
                    }

                    if (mGameScreen.mLevel.mSpawnablePrimatesCount == 0)
                    {
                        mCounterMenu.mIsVisible = false;
                        DisableUnusedApeSelectorButtons();
                    }

                }
                );

            // Create selection button for Gibbon
            mApe4Button =
                new UIv2.Components.Button(mGraphicsDevice, 63, 0, 7, 100, mButtonTexture, "Gibbon", font, Color.White);
            mApe4Button.AddTo(menu);
            mApe4Button.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    SoundManager.AddSound(mClickSound);

                    if (mGameScreen.mLevel.mSpawnablePrimatesCount == 0)
                    {
                        if (mGameScreen.mLevel.mGibbonBatch.mActors.Count > 0)
                        {
                            UnHiglightSelectedActorBatch();
                            mActorBatch = mGameScreen.mLevel.mGibbonBatch;
                            mHighlitedButton = mApe4Button;
                            HighlightSelectedActorBatch();
                        }
                    }
                    else if (mGameScreen.mLevel.mSpawnablePrimatesCount > 0)
                    {
                        mGibbonCounter++;
                        mGameScreen.mLevel.mSpawnablePrimatesCount--;
                        counterLabel.Text = "Available apes to spawn: " + mGameScreen.mLevel.mSpawnablePrimatesCount;

                        var mgibbon = new Gibbon(mGameScreen.mLevel.mGibbonMesh,
                            mGameScreen.mLevel.mTerrain,
                            mGameScreen.mLevel.mSilverback,
                            mGameScreen.mLevel,
                            ref mRandom);
                        mGameScreen.mLevel.Add(mgibbon);
                        LumberChoice.AddApe(4);
                    }

                    if (mGameScreen.mLevel.mSpawnablePrimatesCount == 0)
                    {
                        mCounterMenu.mIsVisible = false;
                        DisableUnusedApeSelectorButtons();
                    }

                });

            // Display the level text before the level starts
            if (mGameScreen.mLevel.mLevelStory != null)
            {
                mGameScreen.mCameraHandler.Lock();
                var textMenu = new UIv2.Menu(mGraphicsDevice, 5, 5, 90, 90);
                textMenu.WithBackground(UIv2.Menu.CreateTexture2D(deviceManager.GraphicsDevice, 50, 30, pixel => new Color(0.0f, 0.0f, 0.0f, 0.15f)), 5, 5, 90, 90);
                var heading = new UIv2.Components.Label(mGraphicsDevice, 5, 5, 90, 15, mGameScreen.mLevel.mLevelTitle, subHeaderFont, Color.DarkSlateGray);
                heading.FontType = FontManager.FontType.Subheading;
                heading.AddTo(textMenu);
                var textLabel = new UIv2.Components.Label(mGraphicsDevice, 25, 28, 50, 35, mGameScreen.mLevel.mLevelStory, font, Color.White);
                textLabel.DisableAutobreak();
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
                        mCounterMenu.mIsVisible = true;
                        textMenu.mIsVisible = false;
                        mGameScreen.mCameraHandler.Unlock();
                    });
                mMenuList.Add(textMenu);
                menu.mIsVisible = false;
                mCounterMenu.mIsVisible = false;
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

        private HudScreen()
        { }

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

            if (mGameScreen.mLevel.mSilverback != null)
            {
                 mHPLabel.Text = "HP: " + mGameScreen.mLevel.mSilverback.HP.ToString() + " / " + mGameScreen.mLevel.mSilverback.mMaxHP.ToString();
            }

            DisableUnusedApeSelectorButtons();

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

            if (mGameScreen.mLevel.mSilverback.HP <= 0)
            {
                ScreenManager.Remove(this);
                ScreenManager.Add(mLoseScreen);
                Statistic.Lost++;
            }

            if (mGameScreen.mLevel.mHuts.Count == 0)
            {
                ScreenManager.Remove(this);
                ScreenManager.Add(mWinScreen);
                Statistic.Win++;
            }

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

        public string Save()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(mGameScreen, Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                });
        }

        public bool Load(string str)
        {
            mGameScreen = Newtonsoft.Json.JsonConvert.DeserializeObject<GameScreen>(str);
            return true;
        }

        private void DisableUnusedApeSelectorButtons()
        {

            if (mGameScreen.mLevel.mSpawnablePrimatesCount == 0 && mGameScreen.mLevel.mOrangutanBatch.mActors.Count == 0)
            {
                mApe1Button.mDisabled = true;
            }

            if (mGameScreen.mLevel.mSpawnablePrimatesCount == 0 && mGameScreen.mLevel.mCapuchinBatch.mActors.Count == 0)
            {
                mApe2Button.mDisabled = true;
            }

            if (mGameScreen.mLevel.mSpawnablePrimatesCount == 0 && mGameScreen.mLevel.mChimpanezzeBatch.mActors.Count == 0)
            {
                mApe3Button.mDisabled = true;
            }

            if (mGameScreen.mLevel.mSpawnablePrimatesCount == 0 && mGameScreen.mLevel.mGibbonBatch.mActors.Count == 0)
            {
                mApe4Button.mDisabled = true;
            }

        }
    }
}
