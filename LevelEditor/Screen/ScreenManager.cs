using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using LevelEditor.Events;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using LevelEditor.Sound;

namespace LevelEditor.Screen
{
    public sealed class ScreenManager
    {

        private readonly GraphicsDeviceManager mGraphicsManager;
        public static GraphicsDeviceManager GraphicsManager;
        private readonly ContentManager mContentManager;
        public static ContentManager ContentManager;
        private readonly Game mGame;
        private readonly SoundManager mSoundManager;
        public static SoundManager SoundManager;
        private List<IScreen> Screens { get; }

        private int RenderingWidth { get; set; }

        private int mRenderingHeight;

        private int mScreenWidth;
        private int mScreenHeight;

        private List<InputEvent> mEvents;

        public ScreenManager(Game game, int renderingWidth, int renderingHeight, bool fullScreen)
        {

            mSoundManager = new SoundManager();
            SoundManager = mSoundManager;

            mScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            mScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            mGraphicsManager = new GraphicsDeviceManager(game) { GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferWidth = mScreenWidth,
                PreferredBackBufferHeight = mScreenHeight,
                IsFullScreen = fullScreen};
            GraphicsManager = mGraphicsManager;

            Options.ScreenResolution(mScreenWidth, mScreenHeight);

            mContentManager = game.Content;
            ContentManager = game.Content;
            mGame = game;

            Screens = new List<IScreen>();

            RenderingWidth = renderingWidth;
            mRenderingHeight = renderingHeight;


            mEvents = new List<InputEvent> {new InputEvent(Mouse.GetState())};
        }
        
        public void Add(IScreen screen)
        {

            screen.ScreenManager = this;
            screen.SoundManager = mSoundManager;
            Screens.Add(screen);
            screen.IsVisible = true;
            
            try
            {
                screen.LoadContent(mGraphicsManager, mContentManager, mScreenWidth, mScreenHeight);
            }
            catch (NotImplementedException) { }
            try
            {
                screen.ChangeWindowSize(mGraphicsManager.PreferredBackBufferWidth, mGraphicsManager.PreferredBackBufferHeight);
            }
            catch (NotImplementedException) { }

            try
            {
                screen.ChangeRenderingResolution(RenderingWidth, mRenderingHeight);
            }
            catch (NotImplementedException) { }

        }

        public void Remove(IScreen screen)
        {

            try
            {
                screen.UnloadContent();
            }
            catch (NotImplementedException) { }

            Screens.Remove(screen);

        }


        public void ChangeWindowSize(int width, int height)
        {

            mGraphicsManager.PreferredBackBufferWidth = width;
            mGraphicsManager.PreferredBackBufferHeight = height;

            foreach (var screen in Screens)
            {
                try
                {
                    screen.ChangeWindowSize(width, height);
                }
                catch (NotImplementedException) { }
                
            }

            mGraphicsManager.ApplyChanges();

        }

        public void ChangeRenderingResolution(int width, int height)
        {
            foreach (var screen in Screens)
            {
                try
                {
                    screen.ChangeRenderingResolution(width, height);
                }
                catch (NotImplementedException) { }

            }

            RenderingWidth = width;
            mRenderingHeight = height;

        }

        public void Update(GameTime gameTime)
        {
            var screens = Screens.ToArray();

            InputManager.Update();

            foreach (var screen in screens)
            {
                try
                {
                    if (screen.IsVisible)
                    {
                        screen.Update(gameTime);
                    }
                }
                catch (NotImplementedException) { }
            }

            if (Screens.Count == 0)
            {
                mGame.Exit();
                return;
            }

            // We want to make sure that at least the lowest screen in the stack is visible
            // Lowest screen has to be visible
            if (!Screens[Screens.Count - 1].IsVisible)
            {
                Screens[Screens.Count - 1].IsVisible = true;
            }

            mSoundManager.Update(gameTime);

        }

        public void Render(GameTime gameTime)
        {

            mGraphicsManager.GraphicsDevice.Clear(Color.Black);
            var visibleScreens = new List<IScreen>();

            // We check the whole list of screens
            foreach (var screen in Screens)
            {

                if (screen.IsVisible)
                {
                    visibleScreens.Add(screen);
                }

            }

            visibleScreens.Reverse();

            foreach (var screen in visibleScreens)
            {
                try
                {
                    screen.Render(gameTime);
                }
                catch (NotImplementedException) { }
            }

        }

    }

}
