using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LevelEditor.Sound;
using LevelEditor.UIv2;
using Menu = LevelEditor.Ui.Menu;
using Microsoft.Xna.Framework.Audio;

namespace LevelEditor.Screen
{
    internal sealed class CreditsScreen : IScreen
    {
        private List<UIv2.Menu> mMenuList;
        public ScreenManager ScreenManager { get; set; }

        public SoundManager SoundManager { get; set; }
        public bool IsVisible { get; set; }

        private int mScreenWidth;
        private int mScreenHeight;

        private SpriteBatch mSpriteBatch;
        private Texture2D mBackgroundImageCredits;
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

            // Load backround Image
            mBackgroundImageCredits = contentManager.Load<Texture2D>("forest");
            IsVisible = true;

            Texture2D texture2D = Menu.CreateTexture2D(mGraphicsDevice, 200, 30, pixel => Color.Black);

            mMenuList = new List<UIv2.Menu>();

            // Instantiate a new menu
            var menu = new UIv2.Menu(mGraphicsDevice, 5, 5, 90, 90);
            menu.WithBackground(Menu.CreateTexture2D(mGraphicsDevice, (int)mTotalWindowSize.X, (int)mTotalWindowSize.Y, pixel => new Color(0.0f, 0.0f, 0.0f, 0.2f)), 5, 5, 90, 90);

            mMenuList.Add(menu);

            var heading = new UIv2.Components.Label(mGraphicsDevice, 35, 5, 30, 15, "Credits", subHeaderFont, Color.White);
            heading.AddTo(menu);

            var scrollListRowCount = 3;

            var rowHeight = 3;
            var rowSpace = 4;

            var labelWidth = 40;

            var labelX = 5;
            /*
            var tt = new UIv2.Components.Label(mGraphicsDevice, labelX, scrollListRowCount++ * rowSpace, labelWidth, rowHeight, "The Team", font, Color.White);
            tt.AddTo(menu);

            var jb = new UIv2.Components.Label(mGraphicsDevice, labelX, scrollListRowCount++ * rowSpace, labelWidth, rowHeight, "Josephine Bergmeier", font, Color.White);
            jb.AddTo(menu);

            var df = new UIv2.Components.Label(mGraphicsDevice, labelX, scrollListRowCount++ * rowSpace, labelWidth, rowHeight, "Domenico Frei", font, Color.White);
            df.AddTo(menu);

            var bf = new UIv2.Components.Label(mGraphicsDevice, labelX, scrollListRowCount++ * rowSpace, labelWidth, rowHeight, "Benjamin Fuchs", font, Color.White);
            bf.AddTo(menu);

            var sg = new UIv2.Components.Label(mGraphicsDevice, labelX, scrollListRowCount++ * rowSpace, labelWidth, rowHeight, "Steven Gilgin", font, Color.White);
            sg.AddTo(menu);

            var mk = new UIv2.Components.Label(mGraphicsDevice, labelX, scrollListRowCount++ * rowSpace, labelWidth, rowHeight, "Mario Kantz", font, Color.White);
            mk.AddTo(menu);

            var dl = new UIv2.Components.Label(mGraphicsDevice, labelX, scrollListRowCount++ * rowSpace, labelWidth, rowHeight, "David Leimroth", font, Color.White);
            dl.AddTo(menu);

            var mr = new UIv2.Components.Label(mGraphicsDevice, labelX, scrollListRowCount++ * rowSpace, labelWidth, rowHeight, "Maximilian Roth", font, Color.White);
            mr.AddTo(menu);

            var st = new UIv2.Components.Label(mGraphicsDevice, labelX, scrollListRowCount++ * rowSpace, labelWidth, rowHeight, "Simon Tippe", font, Color.White);
            st.AddTo(menu);

            var sr = new UIv2.Components.Label(mGraphicsDevice, labelX, scrollListRowCount++ * rowSpace, labelWidth, rowHeight, "Tutor: Samuel Roth", font, Color.White);
            sr.AddTo(menu);

            var noMonkeys = new UIv2.Components.Label(mGraphicsDevice, labelX, scrollListRowCount++ * rowSpace, labelWidth, rowHeight, "No monkeys were harmed during the creation of the game...", font, Color.White);
            noMonkeys.AddTo(menu);

            var wellSome = new UIv2.Components.Label(mGraphicsDevice, labelX, scrollListRowCount++ * rowSpace, labelWidth, rowHeight, "well, some students suffered but that's probably okay. :-)", font, Color.White);
            wellSome.AddTo(menu);

            var nowGo = new UIv2.Components.Label(mGraphicsDevice, labelX, scrollListRowCount++ * rowSpace, labelWidth, rowHeight, "Now go out and hug a tree!", font, Color.White);
            nowGo.AddTo(menu);
            */

            // Credits: Team + Tutor
            var teamString = "The Team:\nJosephine Bergmeier\nDomenico Frei\nBenjamin Fuchs\nSteven Gilgin\nMario Kantz" +
                                "\nDavid Leimroth\nMaximilian Roth\nSimon Tippe\nTutor: Samuel Roth\n\n" +
                                "No monkeys were harmed during the creation of the game...\n" +
                                "well, some students suffered but that's probably okay.\n\n" +
                                "Now go out and hug a tree!";

            var teamLabel = new UIv2.Components.Label(mGraphicsDevice, 35, 40, 30, 15, teamString, font, Color.White);
            teamLabel.DisableAutobreak();
            teamLabel.AddTo(menu);

            // Create the Back Button
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
            mMenuList.Clear();
        }

        public void Update(GameTime time)
        {

            var menus = mMenuList.ToArray();

            // Iterate over all Menu elements
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
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackgroundImageCredits, new Rectangle(0, 0, mScreenWidth, mScreenHeight), Color.White);
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
