using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using LevelEditor.Objects.Ape;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Menu = LevelEditor.Ui.Menu;
using LevelEditor.Sound;
using LevelEditor.UIv2;
using Microsoft.Xna.Framework.Audio;
using LevelEditor.Engine;

namespace LevelEditor.Screen
{
    internal sealed class EditorHudScreen : IScreen
    {

        private readonly string mDefaultDirectoryPath = $"..\\..\\..\\..\\Content";

        private List<UIv2.Menu> mMenuList;
        private EditorScreen mGameScreen;
        private GraphicsDevice mDevice;
        private SpriteFont mFont;
        private SpriteFont mFont2;

        private SpriteBatch mSpriteBatch;

        private int mScreenWidth;
        private int mScreenHeight;

        private SoundEffect mClickSound;

        public UIv2.Components.Label mCameraPositionLabel;

        public ScreenManager ScreenManager { get; set; }
        public SoundManager SoundManager { get; set; }
        public bool IsVisible { get; set; }

        public void LoadContent(GraphicsDeviceManager deviceManager,
            ContentManager contentManager,
            int windowWidth,
            int windowHeight)
        {

            mScreenWidth = windowWidth;
            mScreenHeight = windowHeight;

            //Load SoundEffects
            mClickSound = contentManager.Load<SoundEffect>("Audio/click2");

            mMenuList = new List<UIv2.Menu>();
            mDevice = deviceManager.GraphicsDevice;

            IsVisible = true;

            var buttonWidth = 10;
            var buttonWidthPlusGap = 11;
            var menuButtonCounter = 0;

            // Load font
            mFont = contentManager.Load<SpriteFont>("Font");
            var buttonTexture = contentManager.Load<Texture2D>("button");

            var texture2DMenu = Menu.CreateTexture2D(deviceManager.GraphicsDevice,
                50,
                30,
                pixel => new Color(0.0f, 0.0f, 0.0f, 0.2f));
            var texture2D = Menu.CreateTexture2D(deviceManager.GraphicsDevice,
                50,
                30,
                pixel => Color.Black);
            IsVisible = true;

            // Texture for the health Bar
            mFont2 = contentManager.Load<SpriteFont>("Font2");
            var texture2Dhealthpoints = Menu.CreateTexture2D(deviceManager.GraphicsDevice,
                50,
                30,
                pixel => Color.Red);
            IsVisible = true;

            mSpriteBatch = new SpriteBatch(mDevice);

            mFont2 = contentManager.Load<SpriteFont>("Font2");


            // Instantiate a new menu

            mGameScreen = new EditorScreen();
            ScreenManager.Add(mGameScreen);

            UIv2.Menu effectsMenu = null;
            UIv2.Menu mechanicsMenu = null;

            var menu = new UIv2.Menu(mDevice, 0, 0, 100, 5);
            menu.WithBackground(texture2DMenu, 0, 0, 100, 100);
            mMenuList.Add(menu);

            var menu2 = new UIv2.Menu(mDevice, 80, 5, 20, 95);
            mMenuList.Add(menu2);

            var scrollList =
                new UIv2.Components.ScrollList(deviceManager.GraphicsDevice, 2, 2, 96, 96, texture2DMenu, 8);
            scrollList.AddTo(menu2);
            var scrollListButtonCount = 0;

            // Create a button
            var loadButton = new UIv2.Components.Button(mDevice,
                buttonWidthPlusGap * menuButtonCounter++,
                0,
                buttonWidth,
                100,
                texture2D,
                "Load",
                mFont,
                Color.White);
            loadButton.AddTo(menu);


            // Add an event listener to the button
            loadButton.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    SoundManager.AddSound(mClickSound);

                    deviceManager.ToggleFullScreen();

                    var dialog = new OpenFileDialog
                    {
                        Title = "Load a level",
                        InitialDirectory = mDefaultDirectoryPath,
                        Filter = "Level files (*.lvl)|*.lvl",
                        FilterIndex = 1,
                        Multiselect = false
                    };
                    if (dialog.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    mGameScreen.mLevel.mActorBatches.Clear();
                    mGameScreen.mLevel.Load(dialog.FileName);

                    scrollList.ChildMenu.mElementList.Clear();
                    scrollListButtonCount = 0;

                    foreach (var batch in mGameScreen.mLevel.mActorBatches)
                    {

                        if (batch.mMesh == mGameScreen.mLevel.mTerrain.Actor.mMesh || batch.mMesh == mGameScreen.mLevel.mHutMesh)
                        {
                            continue;
                        }

                        var modelButton = new UIv2.Components.Button(mDevice,
                            0,
                            7 * scrollListButtonCount,
                            100,
                            5,
                            texture2D,
                            Path.GetFileName(batch.mMesh.Path),
                            mFont,
                            Color.White);
                        modelButton.AddTo(scrollList);
                        modelButton.AddListener(MouseButtons.Left,
                            InputState.Pressed,
                            () =>
                            {

                                SoundManager.AddSound(mClickSound);
                                if (mGameScreen.mSelectedActor != null)
                                {
                                    mGameScreen.mLevel.Remove(mGameScreen.mSelectedActor);
                                }

                                mGameScreen.mSelectedActor = null;
                                mGameScreen.mSelectedMesh = batch.mMesh;
                                mGameScreen.mSelectMode = false;
                            });

                        scrollListButtonCount++;
                    }

                    deviceManager.ToggleFullScreen();

                });

            // Create a button
            var saveButton = new UIv2.Components.Button(mDevice,
                buttonWidthPlusGap * menuButtonCounter++,
                0,
                buttonWidth,
                100,
                texture2D,
                "Save",
                mFont,
                Color.White);
            saveButton.AddTo(menu);
            saveButton.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    SoundManager.AddSound(mClickSound);

                    deviceManager.ToggleFullScreen();

                    var dialog = new SaveFileDialog
                    {
                        Title = "Save the level",
                        InitialDirectory = mDefaultDirectoryPath,
                        Filter = "Level files (*.lvl)|*.lvl",
                        FilterIndex = 1,
                        DefaultExt = "lvl",
                        AddExtension = true
                    };
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        mGameScreen.mLevel.Save(dialog.FileName);
                    }

                    deviceManager.ToggleFullScreen();

                });



            // Create a button
            var loadTerrain = new UIv2.Components.Button(mDevice,
                buttonWidthPlusGap * menuButtonCounter++,
                0,
                buttonWidth,
                100,
                texture2D,
                "New Terrain",
                mFont,
                Color.White);
            loadTerrain.AddTo(menu);
            loadTerrain.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    SoundManager.AddSound(mClickSound);
                    deviceManager.ToggleFullScreen();

                    var dialog = new OpenFileDialog
                    {
                        Title = "Load the heightmap",
                        Filter = "PNG files (*.png)|*.png",
                        InitialDirectory = mDefaultDirectoryPath,
                        Multiselect = false
                    };
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {

                        var heightmapPath = dialog.FileName;

                        dialog = new OpenFileDialog
                        {
                            Title = "Load the heightmap texture",
                            InitialDirectory = mDefaultDirectoryPath,
                            Multiselect = false
                        };

                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            var texturePath = dialog.FileName;

                            if (mGameScreen.mLevel.mTerrain != null)
                            {
                                mGameScreen.mLevel.Remove(mGameScreen.mLevel.mTerrain);
                            }

                            mGameScreen.mLevel.mTerrain = new Terrain(contentManager,
                                deviceManager.GraphicsDevice,
                                heightmapPath,
                                texturePath);

                            mGameScreen.mLevel.Add(mGameScreen.mLevel.mTerrain);

                            mGameScreen.IsVisible = true;

                        }

                    }

                    deviceManager.ToggleFullScreen();

                });

            // Create a button
            var reloadTerrain = new UIv2.Components.Button(mDevice,
                buttonWidthPlusGap * menuButtonCounter++,
                0,
                buttonWidth,
                100,
                texture2D,
                "Reload Terrain",
                mFont,
                Color.White);
            reloadTerrain.AddTo(menu);
            reloadTerrain.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    SoundManager.AddSound(mClickSound);

                    try
                    {
                        mGameScreen.mLevel?.ReloadTerrain();
                    }
                    catch (NullReferenceException)
                    {
                    }
                });

            // Create a button
            var newMesh = new UIv2.Components.Button(mDevice,
                buttonWidthPlusGap * menuButtonCounter++,
                0,
                buttonWidth,
                100,
                texture2D,
                "New Mesh",
                mFont,
                Color.White);
            newMesh.AddTo(menu);
            newMesh.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    SoundManager.AddSound(mClickSound);
                    deviceManager.ToggleFullScreen();

                    var dialog = new OpenFileDialog
                    {
                        Title = "Load a mesh",
                        InitialDirectory = mDefaultDirectoryPath + "\\Mesh",
                        Multiselect = false
                    };
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        var mesh = mGameScreen.mLevel.mModelLoader.LoadMesh(dialog.FileName);
                        mGameScreen.mSelectedMesh = mesh;
                        mGameScreen.mSelectedMesh.Path = dialog.FileName;
                        mGameScreen.mSelectedActor = null;
                        mGameScreen.mSelectMode = false;

                        // Create a button
                        var modelButton = new UIv2.Components.Button(mDevice,
                            36,
                            7 * scrollListButtonCount,
                            7,
                            5,
                            texture2D,
                            Path.GetFileName(mesh.Path),
                            mFont,
                            Color.White);
                        modelButton.AddTo(menu);
                        modelButton.AddListener(MouseButtons.Left,
                            InputState.Pressed,
                            () =>
                            {

                                SoundManager.AddSound(mClickSound);

                                mGameScreen.mLevel.Remove(mGameScreen.mSelectedActor);
                                mGameScreen.mSelectedActor = null;
                                mGameScreen.mSelectedMesh = mesh;
                                mGameScreen.mSelectMode = false;
                            });

                        modelButton.AddTo(scrollList);
                        scrollListButtonCount++;
                    }

                    deviceManager.ToggleFullScreen();

                });

            // Create a button
            var levelEffectsButton = new UIv2.Components.Button(mDevice,
                buttonWidthPlusGap * menuButtonCounter++,
                0,
                buttonWidth,
                100,
                texture2D,
                "Effects",
                mFont,
                Color.White);
            levelEffectsButton.AddTo(menu);
            levelEffectsButton.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {

                    SoundManager.AddSound(mClickSound);
                    if (effectsMenu == null)
                    {
                        if (mechanicsMenu != null)
                        {
                            mMenuList.Remove(mechanicsMenu);
                            mechanicsMenu = null;
                        }
                        effectsMenu = CreateSettingsMenu(buttonTexture, texture2DMenu);
                        mMenuList.Add(effectsMenu);
                        effectsMenu.CheckRegisteredEvents();
                    }
                    else
                    {
                        mMenuList.Remove(effectsMenu);
                        effectsMenu = null;
                    }
                });

            // Create a button
            var levelMechanicsButton = new UIv2.Components.Button(mDevice,
                buttonWidthPlusGap * menuButtonCounter++,
                0,
                buttonWidth,
                100,
                texture2D,
                "Mechanics",
                mFont,
                Color.White);
            levelMechanicsButton.AddTo(menu);
            levelMechanicsButton.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {

                    SoundManager.AddSound(mClickSound);
                    if (mechanicsMenu == null)
                    {
                        if (effectsMenu != null)
                        {
                            mMenuList.Remove(effectsMenu);
                            effectsMenu = null;
                        }
                        mechanicsMenu = CreateMechanicsMenu(buttonTexture, texture2DMenu);
                        mMenuList.Add(mechanicsMenu);
                        mechanicsMenu.CheckRegisteredEvents();
                    }
                    else
                    {
                        mMenuList.Remove(mechanicsMenu);
                        mechanicsMenu = null;
                    }
                });

            // Create a button
            var hideMeshMenuButton = new UIv2.Components.Button(mDevice,
                buttonWidthPlusGap * menuButtonCounter++,
                0,
                buttonWidth,
                100,
                texture2D,
                "Toggle Meshes",
                mFont,
                Color.White);
            hideMeshMenuButton.AddTo(menu);
            hideMeshMenuButton.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {

                    SoundManager.AddSound(mClickSound);
                    menu2.SetVisibility(!menu2.mIsVisible);
                    
                    var visibility = mGameScreen.mLevel.mVisibilityGraph;
                    mGameScreen.mLevel.mVisibilityGraph = mGameScreen.mLevel.mBruteVisibilityGraph;
                    mGameScreen.mLevel.mBruteVisibilityGraph = visibility;
                    
                });
            // Create a button
            var backButton = new UIv2.Components.Button(mDevice,
                buttonWidthPlusGap * menuButtonCounter++,
                0,
                buttonWidth,
                100,
                texture2D,
                "Back",
                mFont,
                Color.White);
            backButton.AddTo(menu);
            backButton.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    SoundManager.AddSound(mClickSound);

                    ScreenManager.Remove(this);
                    ScreenManager.Remove(mGameScreen);
                });

            mCameraPositionLabel = new UIv2.Components.Label(mDevice, 0, 0, 100, 100, "", mFont, Color.Orange);

            var cameraMenu = new UIv2.Menu(mDevice, 0, 5, 10, 10);
            mCameraPositionLabel.AddTo(cameraMenu);

            mMenuList.Add(cameraMenu);

            mMenuList.Add(menu);

            mMenuList.Add(menu2);

        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime time)
        {
            var menus = mMenuList.ToArray();

            mCameraPositionLabel.Text = mGameScreen.mCamera.mLocation.ToString();
            // Iterate over all menus
            foreach (var menu in menus)
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

        private UIv2.Menu CreateSettingsMenu(Texture2D buttonTexture, Texture2D texture2DMenu)
        {

            var settingsMenu = new UIv2.Menu(mDevice, 2, 12, 68, 86);
            settingsMenu.WithBackground(texture2DMenu, 0, 0, 100, 100);

            // Just in case we need more space
            // var scrollListSettings = new UIv2.Components.ScrollList(mDevice, 0, 0, 100, 100, texture2DMenu, 8);
            // scrollListSettings.AddTo(settingsMenu);

            var scrollListRowCount = 1;

            var rowHeight = 4;
            var rowSpace = 5;
            var buttonHeight = 5;

            var labelWidth = 35;
            var sliderWidth = 30;
            var valueWidth = 15;
            var buttonWidth = 20;

            var labelX = 5;
            var sliderX = 45;
            var valueX = 80;
            var buttonX = 40;


            Texture2D texture2D = Menu.CreateTexture2D(mDevice, 200, 30, pixel => Color.Black);
            Texture2D texture2DSliderPoint = Menu.CreateTexture2D(mDevice, 200, 30, pixel => Color.White);

            var bloomIntensityLabel = new UIv2.Components.Label(mDevice,
                labelX,
                scrollListRowCount * rowSpace,
                labelWidth,
                rowHeight,
                "Bloom Intensity",
                mFont,
                Color.White);
            bloomIntensityLabel.AddTo(settingsMenu);
            var valueBox1 = new UIv2.Components.Label(mDevice,
                valueX,
                scrollListRowCount * rowSpace,
                valueWidth,
                rowHeight,
                "",
                mFont,
                Color.White);
            valueBox1.AddTo(settingsMenu);
            var bloomIntensitySlider = new UIv2.Components.Slider(mDevice,
                settingsMenu,
                sliderX,
                scrollListRowCount * rowSpace,
                sliderWidth,
                rowHeight,
                texture2D,
                texture2DSliderPoint,
                0,
                5,
                mGameScreen.mLevel.mPostProcessing.mBloom.mIntensity);
            bloomIntensitySlider.AddOnChangeListeners((val) =>
            {
                valueBox1.Text = val.ToString(CultureInfo.InvariantCulture);
                mGameScreen.mLevel.mPostProcessing.mBloom.mIntensity = val;
            });
            scrollListRowCount++;

            var bloomPowerLabel = new UIv2.Components.Label(mDevice,
                labelX,
                scrollListRowCount * rowSpace,
                labelWidth,
                rowHeight,
                "Bloom Power",
                mFont,
                Color.White);
            bloomPowerLabel.AddTo(settingsMenu);
            var valueBox2 = new UIv2.Components.Label(mDevice,
                valueX,
                scrollListRowCount * rowSpace,
                valueWidth,
                rowHeight,
                "",
                mFont,
                Color.White);
            valueBox2.AddTo(settingsMenu);
            var bloomPowerSlider = new UIv2.Components.Slider(mDevice,
                settingsMenu,
                sliderX,
                scrollListRowCount++ * rowSpace,
                sliderWidth,
                rowHeight,
                texture2D,
                texture2DSliderPoint,
                0,
                5,
                mGameScreen.mLevel.mPostProcessing.mBloom.mPower);
            bloomPowerSlider.AddOnChangeListeners((val) =>
            {
                valueBox2.Text = val.ToString(CultureInfo.InvariantCulture);
                mGameScreen.mLevel.mPostProcessing.mBloom.mPower = val;
            });

            var bloomThresholdLabel = new UIv2.Components.Label(mDevice,
                labelX,
                scrollListRowCount * rowSpace,
                labelWidth,
                rowHeight,
                "Bloom Threshold",
                mFont,
                Color.White);
            bloomThresholdLabel.AddTo(settingsMenu);
            var valueBox3 = new UIv2.Components.Label(mDevice,
                valueX,
                scrollListRowCount * rowSpace,
                valueWidth,
                rowHeight,
                "",
                mFont,
                Color.White);
            valueBox3.AddTo(settingsMenu);
            var bloomThresholdSlider = new UIv2.Components.Slider(mDevice,
                settingsMenu,
                sliderX,
                scrollListRowCount++ * rowSpace,
                sliderWidth,
                rowHeight,
                texture2D,
                texture2DSliderPoint,
                -1,
                1,
                mGameScreen.mLevel.mPostProcessing.mBloom.mThreshold);
            bloomThresholdSlider.AddOnChangeListeners((val) =>
            {
                valueBox3.Text = val.ToString(CultureInfo.InvariantCulture);
                mGameScreen.mLevel.mPostProcessing.mBloom.mThreshold = val;
            });

            var timeOfDayLabel = new UIv2.Components.Label(mDevice,
                labelX,
                scrollListRowCount * rowSpace,
                labelWidth,
                rowHeight,
                "Time of Day",
                mFont,
                Color.White);
            timeOfDayLabel.AddTo(settingsMenu);
            var valueBox4 = new UIv2.Components.Label(mDevice,
                valueX,
                scrollListRowCount * rowSpace,
                valueWidth,
                rowHeight,
                "",
                mFont,
                Color.White);
            valueBox4.AddTo(settingsMenu);
            var timeOfDaySlider = new UIv2.Components.Slider(mDevice,
                settingsMenu,
                sliderX,
                scrollListRowCount++ * rowSpace,
                sliderWidth,
                rowHeight,
                texture2D,
                texture2DSliderPoint,
                0,
                23,
                mGameScreen.mLevel.mSky.mTime);
            timeOfDaySlider.AddOnChangeListeners((val) =>
            {
                valueBox4.Text = val.ToString(CultureInfo.InvariantCulture);
                mGameScreen.mLevel.mSky.mTime = val;
            });

            var lumaThresholdLabel = new UIv2.Components.Label(mDevice,
                labelX,
                scrollListRowCount * rowSpace,
                labelWidth,
                rowHeight,
                "LumaThreshold",
                mFont,
                Color.White);
            lumaThresholdLabel.AddTo(settingsMenu);
            var valueBox5 = new UIv2.Components.Label(mDevice,
                valueX,
                scrollListRowCount * rowSpace,
                valueWidth,
                rowHeight,
                "",
                mFont,
                Color.White);
            valueBox5.AddTo(settingsMenu);
            var lumaTresholdSlider = new UIv2.Components.Slider(mDevice,
                settingsMenu,
                sliderX,
                scrollListRowCount++ * rowSpace,
                sliderWidth,
                rowHeight,
                texture2D,
                texture2DSliderPoint,
                0,
                1,
                mGameScreen.mLevel.mPostProcessing.mFxaa.mLumaThreshold);
            lumaTresholdSlider.AddOnChangeListeners((val) =>
            {
                valueBox5.Text = val.ToString(CultureInfo.InvariantCulture);
                mGameScreen.mLevel.mPostProcessing.mFxaa.mLumaThreshold = val;
            });

            var lumaThresholdMinLabel = new UIv2.Components.Label(mDevice,
                labelX,
                scrollListRowCount * rowSpace,
                labelWidth,
                rowHeight,
                "LumaThresholdMin",
                mFont,
                Color.White);
            lumaThresholdMinLabel.AddTo(settingsMenu);
            var valueBox6 = new UIv2.Components.Label(mDevice,
                valueX,
                scrollListRowCount * rowSpace,
                valueWidth,
                rowHeight,
                "",
                mFont,
                Color.White);
            valueBox6.AddTo(settingsMenu);
            var lumaTresholdMinSlider = new UIv2.Components.Slider(mDevice,
                settingsMenu,
                sliderX,
                scrollListRowCount++ * rowSpace,
                sliderWidth,
                rowHeight,
                texture2D,
                texture2DSliderPoint,
                0,
                1,
                mGameScreen.mLevel.mPostProcessing.mFxaa.mLumaThresholdMin);
            lumaTresholdMinSlider.AddOnChangeListeners((val) =>
            {
                valueBox6.Text = val.ToString(CultureInfo.InvariantCulture);
                mGameScreen.mLevel.mPostProcessing.mFxaa.mLumaThresholdMin = val;
            });

            var lightRLabel = new UIv2.Components.Label(mDevice,
                labelX,
                scrollListRowCount * rowSpace,
                labelWidth,
                rowHeight,
                "Light R value",
                mFont,
                Color.White);
            lightRLabel.AddTo(settingsMenu);
            var valueBox7 = new UIv2.Components.Label(mDevice,
                valueX,
                scrollListRowCount * rowSpace,
                valueWidth,
                rowHeight,
                "",
                mFont,
                Color.White);
            valueBox7.AddTo(settingsMenu);
            var lightRSlider = new UIv2.Components.Slider(mDevice,
                settingsMenu,
                sliderX,
                scrollListRowCount++ * rowSpace,
                sliderWidth,
                rowHeight,
                texture2D,
                texture2DSliderPoint,
                0,
                255,
                mGameScreen.mLevel.mSky.Light.mColor.X * 255.0f);
            lightRSlider.AddOnChangeListeners((val) =>
            {
                valueBox7.Text = val.ToString(CultureInfo.InvariantCulture);
                mGameScreen.mLevel.mSky.Light.mColor.X = val / 255.0f;
            });

            var lightGLabel = new UIv2.Components.Label(mDevice,
                labelX,
                scrollListRowCount * rowSpace,
                labelWidth,
                rowHeight,
                "Light G value",
                mFont,
                Color.White);
            lightGLabel.AddTo(settingsMenu);
            var valueBox8 = new UIv2.Components.Label(mDevice,
                valueX,
                scrollListRowCount * rowSpace,
                valueWidth,
                rowHeight,
                "",
                mFont,
                Color.White);
            valueBox8.AddTo(settingsMenu);
            var lightGSlider = new UIv2.Components.Slider(mDevice,
                settingsMenu,
                sliderX,
                scrollListRowCount++ * rowSpace,
                sliderWidth,
                rowHeight,
                texture2D,
                texture2DSliderPoint,
                0,
                255,
                mGameScreen.mLevel.mSky.Light.mColor.Y * 255.0f);
            lightGSlider.AddOnChangeListeners((val) =>
            {
                valueBox8.Text = val.ToString(CultureInfo.InvariantCulture);
                mGameScreen.mLevel.mSky.Light.mColor.Y = val / 255.0f;
            });

            var lightBLabel = new UIv2.Components.Label(mDevice,
                labelX,
                scrollListRowCount * rowSpace,
                labelWidth,
                rowHeight,
                "Light B value",
                mFont,
                Color.White);
            lightBLabel.AddTo(settingsMenu);
            var valueBox9 = new UIv2.Components.Label(mDevice,
                valueX,
                scrollListRowCount * rowSpace,
                valueWidth,
                rowHeight,
                "",
                mFont,
                Color.White);
            valueBox9.AddTo(settingsMenu);
            var lightBSlider = new UIv2.Components.Slider(mDevice,
                settingsMenu,
                sliderX,
                scrollListRowCount++ * rowSpace,
                sliderWidth,
                rowHeight,
                texture2D,
                texture2DSliderPoint,
                0,
                255,
                mGameScreen.mLevel.mSky.Light.mColor.Z * 255.0f);
            lightBSlider.AddOnChangeListeners((val) =>
            {
                valueBox9.Text = val.ToString(CultureInfo.InvariantCulture);
                mGameScreen.mLevel.mSky.Light.mColor.Z = val / 255.0f;
            });

            var ambienteLabel = new UIv2.Components.Label(mDevice,
                labelX,
                scrollListRowCount * rowSpace,
                labelWidth,
                rowHeight,
                "Ambient",
                mFont,
                Color.White);
            ambienteLabel.AddTo(settingsMenu);
            var valueBox10 = new UIv2.Components.Label(mDevice,
                valueX,
                scrollListRowCount * rowSpace,
                valueWidth,
                rowHeight,
                "",
                mFont,
                Color.White);
            valueBox10.AddTo(settingsMenu);
            var ambienteSlider = new UIv2.Components.Slider(mDevice,
                settingsMenu,
                sliderX,
                scrollListRowCount++ * rowSpace,
                sliderWidth,
                rowHeight,
                texture2D,
                texture2DSliderPoint,
                0,
                1,
                mGameScreen.mLevel.mSky.Light.mAmbient);
            ambienteSlider.AddOnChangeListeners((val) =>
            {
                valueBox10.Text = val.ToString(CultureInfo.InvariantCulture);
                mGameScreen.mLevel.mSky.Light.mAmbient = val;
            });

            var fogRLabel = new UIv2.Components.Label(mDevice,
                labelX,
                scrollListRowCount * rowSpace,
                labelWidth,
                rowHeight,
                "Fog R value",
                mFont,
                Color.White);
            fogRLabel.AddTo(settingsMenu);
            var valueBox12 = new UIv2.Components.Label(mDevice,
                valueX,
                scrollListRowCount * rowSpace,
                valueWidth,
                rowHeight,
                "",
                mFont,
                Color.White);
            valueBox12.AddTo(settingsMenu);
            var fogRSlider = new UIv2.Components.Slider(mDevice,
                settingsMenu,
                sliderX,
                scrollListRowCount++ * rowSpace,
                sliderWidth,
                rowHeight,
                texture2D,
                texture2DSliderPoint,
                0,
                255,
                mGameScreen.mLevel.mFog.mColor.X * 255.0f);
            fogRSlider.AddOnChangeListeners((val) =>
            {
                valueBox12.Text = val.ToString(CultureInfo.InvariantCulture);
                mGameScreen.mLevel.mFog.mColor.X = val / 255.0f;
            });

            var fogGLabel = new UIv2.Components.Label(mDevice,
                labelX,
                scrollListRowCount * rowSpace,
                labelWidth,
                rowHeight,
                "Fog G value",
                mFont,
                Color.White);
            fogGLabel.AddTo(settingsMenu);
            var valueBox13 = new UIv2.Components.Label(mDevice,
                valueX,
                scrollListRowCount * rowSpace,
                valueWidth,
                rowHeight,
                "",
                mFont,
                Color.White);
            valueBox13.AddTo(settingsMenu);
            var fogGSlider = new UIv2.Components.Slider(mDevice,
                settingsMenu,
                sliderX,
                scrollListRowCount++ * rowSpace,
                sliderWidth,
                rowHeight,
                texture2D,
                texture2DSliderPoint,
                0,
                255,
                mGameScreen.mLevel.mFog.mColor.Y * 255.0f);
            fogGSlider.AddOnChangeListeners((val) =>
            {
                valueBox13.Text = val.ToString(CultureInfo.InvariantCulture);
                mGameScreen.mLevel.mFog.mColor.Y = val / 255.0f;
            });

            var fogBLabel = new UIv2.Components.Label(mDevice,
                labelX,
                scrollListRowCount * rowSpace,
                labelWidth,
                rowHeight,
                "Fog B value",
                mFont,
                Color.White);
            fogBLabel.AddTo(settingsMenu);
            var valueBox14 = new UIv2.Components.Label(mDevice,
                valueX,
                scrollListRowCount * rowSpace,
                valueWidth,
                rowHeight,
                "",
                mFont,
                Color.White);
            valueBox14.AddTo(settingsMenu);
            var fogBSlider = new UIv2.Components.Slider(mDevice,
                settingsMenu,
                sliderX,
                scrollListRowCount++ * rowSpace,
                sliderWidth,
                rowHeight,
                texture2D,
                texture2DSliderPoint,
                0,
                255,
                mGameScreen.mLevel.mFog.mColor.Z * 255.0f);
            fogBSlider.AddOnChangeListeners((val) =>
            {
                valueBox14.Text = val.ToString(CultureInfo.InvariantCulture);
                mGameScreen.mLevel.mFog.mColor.Z = val / 255.0f;
            });


            scrollListRowCount += 2;

            var debugModeButton = new UIv2.Components.Button(mDevice,
                20,
                scrollListRowCount * rowSpace,
                20,
                rowHeight,
                texture2D,
                "Debug Mode (off)",
                mFont,
                Color.White);
            debugModeButton.AddTo(settingsMenu);
            debugModeButton.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    var debug = mGameScreen.mMasterRenderer.DebugMode;
                    debug = !debug;
                    debugModeButton.Text = debug ? "Debug mode (on)" : "Debug mode (off)";
                    mGameScreen.mMasterRenderer.DebugMode = debug;

                    SoundManager.AddSound(mClickSound);
                });

            // Create a button
            var fxaaButton = new UIv2.Components.Button(mDevice,
                50,
                scrollListRowCount * rowSpace,
                20,
                rowHeight,
                texture2D,
                "FXAA Debug (off)",
                mFont,
                Color.White);
            fxaaButton.AddTo(settingsMenu);
            // Add an event listener to the button
            fxaaButton.AddListener(MouseButtons.Left,
                InputState.Pressed,
                () =>
                {
                    var fxaa = mGameScreen.mLevel.mPostProcessing.mFxaa;
                    fxaa.mDebugMode = !fxaa.mDebugMode;
                    fxaaButton.Text = fxaa.mDebugMode ? "FXAA Debug (on)" : "FXAA Debug (off)";

                    SoundManager.AddSound(mClickSound);
                });

            return settingsMenu;
        }

        private UIv2.Menu CreateMechanicsMenu(Texture2D buttonTexture, Texture2D texture2DMenu)
        {

            var mechanicsMenu = new UIv2.Menu(mDevice, 2, 12, 68, 86);
            mechanicsMenu.WithBackground(texture2DMenu, 0, 0, 100, 100);

            // Just in case we need more space
            // var scrollListSettings = new UIv2.Components.ScrollList(mDevice, 0, 0, 100, 100, texture2DMenu, 8);
            // scrollListSettings.AddTo(settingsMenu);

            var scrollListRowCount = 1;

            var rowHeight = 4;
            var rowSpace = 5;
            var buttonHeight = 5;

            var labelWidth = 35;
            var sliderWidth = 30;
            var valueWidth = 15;
            var buttonWidth = 20;

            var labelX = 5;
            var sliderX = 45;
            var valueX = 80;
            var buttonX = 40;

            return mechanicsMenu;

        }

    }

}
