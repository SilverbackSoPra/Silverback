using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LevelEditor.UIv2;
using Menu = LevelEditor.Ui.Menu;
using LevelEditor.Sound;
using Label = LevelEditor.UIv2.Components.Label;
using Microsoft.Xna.Framework.Audio;

namespace LevelEditor.Screen
{
    internal sealed class OptionsMenu: IScreen
    {
        private List<UIv2.Menu> mMenuList;
        private SpriteBatch mSpriteBatch;
        private Texture2D mBackgroundImageOptions;

        private GraphicsDevice mGraphicsDevice;

        private int mScreenWidth;
        private int mScreenHeight;
        private readonly bool mPauseScreen;

        private SoundEffect mClickSound;

        public ScreenManager ScreenManager { get; set; }

        public SoundManager SoundManager { get; set; }
        public bool IsVisible { get; set; }
        
        public OptionsMenu(bool pauseScreen)
        {
            mPauseScreen = pauseScreen;
        }

        public void LoadContent(GraphicsDeviceManager deviceManager, ContentManager contentManager, int windowWidth, int windowHeight)
        {

            mScreenWidth = windowWidth;
            mScreenHeight = windowHeight;

            //Load SoundEffects
            mClickSound = contentManager.Load<SoundEffect>("Audio/click2");

            // Unlock the framerate. Later we want to remove this piece of code
            mGraphicsDevice = deviceManager.GraphicsDevice;
            mSpriteBatch = new SpriteBatch(deviceManager.GraphicsDevice);

            // Load font
            var font = contentManager.Load<SpriteFont>("Font");
            var subHeaderFont = contentManager.Load<SpriteFont>("SubHeaderFont");
            IsVisible = true;

            // Load backround Image
            mBackgroundImageOptions = contentManager.Load<Texture2D>("forest");

            mMenuList = new List<UIv2.Menu>();

            var menu = new UIv2.Menu(mGraphicsDevice, 5, 5, 90, 90);
            menu.WithBackground(Menu.CreateTexture2D(deviceManager.GraphicsDevice, 50, 30, pixel => new Color(0.0f, 0.0f, 0.0f, 0.2f)), 5, 5, 90, 90);
            mMenuList.Add(menu);

            Texture2D Checked = Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.DarkOliveGreen);
            Texture2D texture2D = Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.Black);
            Texture2D texture2DSliderPoint = Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.White);

            var heading = new Label(mGraphicsDevice, 35, 5, 30, 15, "Options", subHeaderFont, Color.White);
            heading.AddTo(menu);


            var resolutionLabel = new Label(mGraphicsDevice, 10, 20, 30, 3, "Resolution", font, Color.White);
            resolutionLabel.AddTo(menu);
            var resolutionValueBox = new Label(mGraphicsDevice, 60, 20, 20, 3, "", font, Color.White);
            resolutionValueBox.AddTo(menu);
            var resolutionSlider = new UIv2.Components.Slider(mGraphicsDevice, menu, 40, 20, 20, 3, texture2D, texture2DSliderPoint, 25, 200, Options.Resolution);


            // We want to allow the user to set the resolution to 25%.
            // Lets assume the user has a 4K monitor then we would allow
            // a resolution range of [1024x540-8192x4320]. The higher resolution
            // is for PCs which have a lot of hardware power and will be used for super sampling
            // which gives us really clean edges (no FXAA needed anymore)
            
            // You can manually set the slider value
            // The ui will be updated accordingly
            
            // Add one or more callbacks as arguments
            // The function(s) will be called, after the value changed
            resolutionSlider.AddOnChangeListeners((val) => {

                var value = (int)Math.Round(val);
                var width = (int)(mScreenWidth * (value / 100.0f));
                var height = (int)(mScreenHeight * (value / 100.0f));
                resolutionValueBox.Text = width + "x" + height + " pixel";
                ScreenManager.ChangeRenderingResolution(width, height);
                Options.Resolution = value;

            });

            var musicLabel = new Label(mGraphicsDevice, 10, 28, 30, 3, "Music Volume", font, Color.White);
            musicLabel.AddTo(menu);
            var musicValueBox = new Label(mGraphicsDevice, 60, 28, 20, 3, "", font, Color.White);
            musicValueBox.AddTo(menu);

            var musicSlider = new UIv2.Components.Slider(mGraphicsDevice, menu, 40, 28, 20, 3, texture2D, texture2DSliderPoint, 0, 100, Options.MusicVolume);

            musicSlider.AddOnChangeListeners((val) => {
                var value = (int)Math.Round(val);
                Options.MusicVolume = value;
                musicValueBox.Text = value + "%";
                //Soundmanager has to be set
                //SoundManager.ChangeVolume("Music", value / 100.0f);
            });


            var effectLabel = new Label(mGraphicsDevice, 10, 36, 30, 3, "Sound Effects Volume", font, Color.White);
            effectLabel.AddTo(menu);
            var effectValueBox = new Label(mGraphicsDevice, 60, 36, 20, 3, "", font, Color.White);
            effectValueBox.AddTo(menu);

            var effectSlider = new UIv2.Components.Slider(mGraphicsDevice, menu, 40, 36, 20, 3, texture2D, texture2DSliderPoint, 0, 100, Options.SoundEffectVolume);

            effectSlider.AddOnChangeListeners((val) => {
                var value = (int)Math.Round(val);
                Options.SoundEffectVolume = value;
                effectValueBox.Text = value + "%";
            });
            // TODO adding function of brightness 
            var brightnessLabel = new Label(mGraphicsDevice, 10, 44, 30, 3, "Brightness", font, Color.White);
            brightnessLabel.AddTo(menu);
            var brightnessValueBox = new Label(mGraphicsDevice, 60, 44, 20, 3, "", font, Color.White);
            brightnessValueBox.AddTo(menu);

            var brightnessSlider = new UIv2.Components.Slider(mGraphicsDevice, menu, 40, 44, 20, 3, texture2D, texture2DSliderPoint, 0, 1, Options.Brightness);

            brightnessSlider.AddOnChangeListeners((val) => {
                var value = (int)Math.Round(val * 100); // here I'm unsure          
                Options.Brightness = val;
                brightnessValueBox.Text = value + "%";
            });


            var fxaaLabel = new Label(mGraphicsDevice, 10, 52, 30, 3, "Antialiasing", font, Color.White);
            fxaaLabel.AddTo(menu);
            var fxaaButtonOn = new UIv2.Components.CheckedButton(mGraphicsDevice, 40, 52, 5, 5, texture2D, Checked, "On", font, Color.White, Options.Fxaa);
            fxaaButtonOn.AddTo(menu);
            var fxaaButtonOff = new UIv2.Components.CheckedButton(mGraphicsDevice, 46, 52, 5, 5, texture2D, Checked, "Off", font, Color.White, !Options.Fxaa);
            fxaaButtonOff.AddTo(menu);

            if (!mPauseScreen)
            {
                fxaaButtonOn.AddListener(MouseButtons.Left,
                    InputState.Pressed,
                    () =>
                    {
                        SoundManager.AddSound(mClickSound);
                        fxaaButtonOn.Check();
                        fxaaButtonOff.Uncheck();
                        Options.Fxaa = true;

                    });
                fxaaButtonOff.AddListener(MouseButtons.Left,
                    InputState.Pressed,
                    () =>
                    {

                        SoundManager.AddSound(mClickSound);
                        fxaaButtonOff.Check();
                        fxaaButtonOn.Uncheck();
                        Options.Fxaa = false;

                    });
            }

            var graphicsLabel = new Label(mGraphicsDevice, 10, 60, 30, 3, "Graphics Quality", font, Color.White);
            graphicsLabel.AddTo(menu);

           
            var graphicsVeryLowButton = new UIv2.Components.CheckedButton(mGraphicsDevice, 40, 60, 8, 5, texture2D, Checked, "Very low", font, Color.White, Options.GraphicsQuality == 0);
            graphicsVeryLowButton.AddTo(menu);
            var graphicsLowButton = new UIv2.Components.CheckedButton(mGraphicsDevice, 49, 60, 8, 5, texture2D, Checked, "Low", font, Color.White, Options.GraphicsQuality == 1);
            graphicsLowButton.AddTo(menu);
            var graphicsMediumButton = new UIv2.Components.CheckedButton(mGraphicsDevice, 58, 60, 8, 5, texture2D, Checked, "Medium", font, Color.White, Options.GraphicsQuality == 2);
            graphicsMediumButton.AddTo(menu);
            var graphicsHighButton = new UIv2.Components.CheckedButton(mGraphicsDevice, 67, 60, 8, 5, texture2D, Checked, "High", font, Color.White, Options.GraphicsQuality == 3);
            graphicsHighButton.AddTo(menu);
            var graphicsVeryHighButton = new UIv2.Components.CheckedButton(mGraphicsDevice, 76, 60, 8, 5, texture2D, Checked, "Very high", font, Color.White, Options.GraphicsQuality == 4);
            graphicsVeryHighButton.AddTo(menu);

            // We just want to use the handlers if this screen isn't initialized by the PauseScreen
            if (!mPauseScreen)
            {
                graphicsVeryLowButton.AddListener(MouseButtons.Left,
                    InputState.Pressed,
                    () =>
                    {
                        SoundManager.AddSound(mClickSound);
                        graphicsVeryLowButton.Check();
                        graphicsLowButton.Uncheck();
                        graphicsMediumButton.Uncheck();
                        graphicsHighButton.Uncheck();
                        graphicsVeryHighButton.Uncheck();
                        Options.GraphicsQuality = 0;
                    });

                graphicsLowButton.AddListener(MouseButtons.Left,
                    InputState.Pressed,
                    () =>
                    {
                        SoundManager.AddSound(mClickSound);
                        graphicsVeryLowButton.Uncheck();
                        graphicsLowButton.Check();
                        graphicsMediumButton.Uncheck();
                        graphicsHighButton.Uncheck();
                        graphicsVeryHighButton.Uncheck();
                        Options.GraphicsQuality = 1;

                    });

                graphicsMediumButton.AddListener(MouseButtons.Left,
                    InputState.Pressed,
                    () =>
                    {
                        SoundManager.AddSound(mClickSound);
                        graphicsVeryLowButton.Uncheck();
                        graphicsLowButton.Uncheck();
                        graphicsMediumButton.Check();
                        graphicsHighButton.Uncheck();
                        graphicsVeryHighButton.Uncheck();
                        Options.GraphicsQuality = 2;

                    });

                graphicsHighButton.AddListener(MouseButtons.Left,
                    InputState.Pressed,
                    () =>
                    {
                        SoundManager.AddSound(mClickSound);
                        graphicsVeryLowButton.Uncheck();
                        graphicsLowButton.Uncheck();
                        graphicsMediumButton.Uncheck();
                        graphicsHighButton.Check();
                        graphicsVeryHighButton.Uncheck();
                        Options.GraphicsQuality = 3;

                    });

                graphicsVeryHighButton.AddListener(MouseButtons.Left,
                    InputState.Pressed,
                    () =>
                    {
                        SoundManager.AddSound(mClickSound);
                        graphicsVeryLowButton.Uncheck();
                        graphicsLowButton.Uncheck();
                        graphicsMediumButton.Uncheck();
                        graphicsHighButton.Uncheck();
                        graphicsVeryHighButton.Check();
                        Options.GraphicsQuality = 4;
                    });
            }
            else
            {
                var graphicsInformationLabel = new Label(mGraphicsDevice,
                    20,
                    65,
                    60,
                    10,
                    "Graphics quality and Antialiasing can only be changed from the main menu",
                    font,
                    Color.Orange);
                graphicsInformationLabel.AddTo(menu);
            }


            var backButton = new UIv2.Components.Button(mGraphicsDevice, 40, 80, 20, 7, texture2D, "Back", font, Color.White);
            backButton.AddTo(menu);
            backButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                SoundManager.AddSound(mClickSound);
                ScreenManager.Remove(this);
                IsVisible = false;
            });
        }

        public void UnloadContent()
        {
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
        }
    }
}
