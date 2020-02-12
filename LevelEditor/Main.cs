using LevelEditor.Pathfinding;
using LevelEditor.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace LevelEditor
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    internal sealed class Main : Game
    {

        private readonly ScreenManager mScreenManager;
        private readonly MainMenuScreen mMainMenuScreen;

        public Main()
        {

            // This should also be removed if we don't want the variable framerate anymore
            IsFixedTimeStep = false;

            mScreenManager = new ScreenManager(this, 1920, 1080, true);
            mMainMenuScreen = new MainMenuScreen();

            IsMouseVisible = true;

            Content.RootDirectory = "Content";

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // TODO: use this.Content to load your game content here
            mScreenManager.Add(mMainMenuScreen);

            Statistic.Load();
            Options.Load();
            Achievements.Load();

            mScreenManager.ChangeRenderingResolution(Options.ResolutionWidth, Options.ResolutionHeight);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            
            // TODO: Unload any non ContentManager content here
            mScreenManager.Remove(mMainMenuScreen);

            Statistic.Save();
            Options.Save();
            Achievements.Save();

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            if (IsActive)
            {

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    Exit();
                }

                mScreenManager.Update(gameTime);

            }

            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            mScreenManager.Render(gameTime);

            base.Draw(gameTime);

        }
    }
}
