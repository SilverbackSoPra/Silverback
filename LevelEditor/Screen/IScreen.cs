using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using LevelEditor.Sound;

namespace LevelEditor.Screen
{

    /// <summary>
    /// The interface is used to implement a screen which is then organized by the screen manager.
    /// When a screen isn't needed anymore it should remove itself from the screen manager.
    /// </summary>
    public interface IScreen
    {

        /// <summary>
        /// The screen manager in which the screen will be organized.
        /// </summary>
        ScreenManager ScreenManager { get; set; }

        SoundManager SoundManager { get; set; }

        /// <summary>
        /// Determines if the screen gets rendered
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// A method where all the content of the screen should be loaded.
        /// Will be called when the screen is added to the screen manager.
        /// </summary>
        /// <param name="deviceManager">The <see cref="GraphicsDeviceManager"/> which is currently in use</param>
        /// <param name="contentManager">The <see cref="ContentManager"/> which is used by the <see cref="ScreenManager"/></param>
        /// <param name="windowWidth">The width of the window</param>
        /// <param name="windowHeight">The height of the window</param>
        void LoadContent(GraphicsDeviceManager deviceManager, ContentManager contentManager, int windowWidth, int windowHeight);

        /// <summary>
        /// A method where all previosly loaded content should be deleted.
        /// Will be called when screen is removed from the screen manager.
        /// </summary>
        void UnloadContent();

        /// <summary>
        /// A method where the screen should update all its contents.
        /// Is called every frame by the screen manager.
        /// </summary>
        /// <param name="time">The game time</param>
        void Update(GameTime time);

        /// <summary>
        /// A method where the screen should render all its stuff.
        /// Is called every frame by the screen manager.
        /// </summary>
        /// <param name="time">The game time</param>
        void Render(GameTime time);

        /// <summary>
        /// This method will be called by the screen manager if there is a window resize.
        /// Will also be called when the screen is added to the screen manager.
        /// </summary>
        /// <param name="width">The width of the window</param>
        /// <param name="height">The height of the window</param>
        void ChangeWindowSize(int width, int height);

        /// <summary>
        /// This method will be called by the screen manager if the rendering resolution has changed.
        /// Will also be called when the screen is added to the screen manager.
        /// </summary>
        /// <param name="width">The width of the rendering resolution</param>
        /// <param name="height">The height of the rendering resolution</param>
        void ChangeRenderingResolution(int width, int height);

    }
}
