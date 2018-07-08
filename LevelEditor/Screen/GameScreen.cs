using System;
using LevelEditor.Engine;
using LevelEditor.Engine.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LevelEditor.Sound;

namespace LevelEditor.Screen
{

    /// <summary>
    /// The actual Screen for the Levels and not the Level Editor
    /// 
    /// Later on we want to create a game screen per level. This is why we hand over the path to the level file in the constructor
    /// For now 
    /// </summary>
    internal sealed class GameScreen : IScreen
    {

        public ScreenManager ScreenManager { get; set; }
        public SoundManager SoundManager { get; set; }
        public bool IsVisible { get; set; }

        public Level mLevel;

        public Camera mCamera;
        public CameraHandler mCameraHandler;
        public Viewport mViewport;

        private RenderTarget mRenderTarget;

        private MasterRenderer mMasterRenderer;

        private ContentManager mContentManager;

        private SpriteBatch mSpriteBatch;
        private UIv2.Menu mMenu;
        private UIv2.Components.Label mLabel;

        private readonly string mLevelPath;

        public GameScreen (string levelPath)
        {
            mLevelPath = levelPath;
        }

        public void LoadContent(GraphicsDeviceManager deviceManager, ContentManager contentManager, int windowWidth, int windowHeight)
        {

            var graphicsDevice = deviceManager.GraphicsDevice;

            // Create a new content manager so we can unload all its content later
            mContentManager = new ContentManager(contentManager.ServiceProvider, contentManager.RootDirectory);

            mLevel = new Level(mContentManager, graphicsDevice, false);
            mLevel.Load(mLevelPath);

            mSpriteBatch = new SpriteBatch(graphicsDevice);

            var font = mContentManager.Load<SpriteFont>("Font");

            mMenu = new UIv2.Menu(graphicsDevice, 0, 0, 100, 100);
            mLabel = new UIv2.Components.Label(graphicsDevice, 0, 0, 5, 3, "", font, Color.Orange);
            mLabel.AddTo(mMenu);

            mRenderTarget = new RenderTarget(graphicsDevice, Options.ResolutionWidth, Options.ResolutionHeight, Options.ShadowResolution);
            mMasterRenderer = new MasterRenderer(graphicsDevice, mContentManager) { DebugMode = false };

            mCamera = new Camera(farPlane: Options.ViewingDistance, nearPlane: 0.5f, thirdPerson: true);
            mCameraHandler = new CameraHandler(mCamera, 4.0f, 2.0f, .3f);

            mCamera.UpdatePerspective();
            mCamera.mLocation = new Vector3(0.0f, 20.0f, -10.0f);

        }

        public void UnloadContent()
        {

            mRenderTarget.Dispose();
            mMasterRenderer.Dispose();
            mLevel.Dispose();
            mContentManager.Dispose();

        }

        public void Update(GameTime gameTime)
        {

            mCameraHandler.Update(gameTime.ElapsedGameTime.Milliseconds);
            
            mLevel.Update(mCamera, mCameraHandler,gameTime);

            SoundManager.Update(gameTime, mLevel, mCamera);

            mLabel.Text = gameTime.ElapsedGameTime.Milliseconds + "ms";

        }

        public void Render(GameTime gameTime)
        { 

            mMasterRenderer.Render(mViewport, mRenderTarget, mCamera, mLevel);

            mMenu.Render(mSpriteBatch);

        }

        public void ChangeWindowSize(int width, int height)
        {
            mViewport = new Viewport(0, 0, width, height);
        }

        public void ChangeRenderingResolution(int width, int height)
        {
            
            mRenderTarget.Resize(width, height);
        }

        public void Load()
        {
        }
    }
}