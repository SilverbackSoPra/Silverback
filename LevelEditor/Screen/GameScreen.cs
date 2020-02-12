using System;
using LevelEditor.Engine;
using LevelEditor.Engine.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LevelEditor.Sound;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using LevelEditor.UIv2;
using LevelEditor.UIv2.Components;
using Newtonsoft.Json;

namespace LevelEditor.Screen
{

    /// <summary>
    /// The actual Screen for the Levels and not the Level Editor
    /// 
    /// Later on we want to create a game screen per level. This is why we hand over the path to the level file in the constructor
    /// For now 
    /// </summary>
    [Serializable()]
    public sealed class GameScreen : IScreen, ISerializable
    {

        [XmlIgnore]
        [JsonIgnore]
        public ScreenManager ScreenManager { get; set; }

        [XmlIgnore]
        public SoundManager SoundManager { get; set; }
        public bool IsVisible { get; set; }
        
        [XmlElement("mLevel")]
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

        private string mLevelPath;

        public GameScreen(string levelPath)
        {
            mLevelPath = levelPath;
        }

        public void LoadContent(GraphicsDeviceManager deviceManager,
            ContentManager contentManager,
            int windowWidth,
            int windowHeight)
        {

            var graphicsDevice = deviceManager.GraphicsDevice;

            // Create a new content manager so we can unload all its content later
            mContentManager = new ContentManager(contentManager.ServiceProvider, contentManager.RootDirectory);

            mLevel = new Level(mContentManager, SoundManager, graphicsDevice, false);
            mLevel.Load(mLevelPath);

            mSpriteBatch = new SpriteBatch(graphicsDevice);

            var font = mContentManager.Load<SpriteFont>("Font");

            mMenu = new Menu(graphicsDevice, 0, 0, 100, 100);
            mLabel = new Label(graphicsDevice, 0, 0, 25, 3, "", font, Color.Orange);
            mLabel.FontType = FontManager.FontType.Default;
            mLabel.AddTo(mMenu);

            mRenderTarget = new RenderTarget(graphicsDevice,
                Options.ResolutionWidth,
                Options.ResolutionHeight,
                Options.ShadowResolution);
            mMasterRenderer = new MasterRenderer(graphicsDevice, mContentManager) {DebugMode = false};

            mCamera = new Camera(farPlane: Options.ViewingDistance,
                nearPlane: 0.5f,
                thirdPerson: true,
                location: mLevel.mInitialSilverbackLocation);
            mCameraHandler = new CameraHandler(mCamera, 4.0f, 2.0f, .3f);

            mCamera.UpdatePerspective();

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

            Statistic.Time += gameTime.ElapsedGameTime.Milliseconds;
            Statistic.CurrentSaveTime += gameTime.ElapsedGameTime.Milliseconds;

            var oldLocation = mCameraHandler.mLocation;

            mCameraHandler.Update(gameTime.ElapsedGameTime.Milliseconds);

            if (mLevel.mTerrain.GetSlope(mCameraHandler.mLocation) > Math.PI / 6.0f)
            {
                mCameraHandler.mLocation = oldLocation;
            }

            mLevel.Update(mCamera, mCameraHandler, gameTime);

            SoundManager.Update(gameTime, mLevel, mCamera);

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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsVisible", IsVisible);
            info.AddValue("mLevel", mLevel);
            info.AddValue("mCamera", mCamera);
            info.AddValue("mCameraHandler", mCameraHandler);
            // info.AddValue("mViewport", mViewport);
            // info.AddValue("mRenderTarget", mRenderTarget);
            // info.AddValue("mMasterRenderer", mMasterRenderer);
            // info.AddValue("mContentManager", mContentManager);
            // info.AddValue("mSpriteBatch", mSpriteBatch);
            info.AddValue("mMenu", mMenu);
            info.AddValue("mLabel", mLabel);
            info.AddValue("mLevelPath", mLevelPath);
        }


        public GameScreen(SerializationInfo info, StreamingContext context)
        {
            IsVisible = (bool)info.GetValue("IsVisible", typeof(bool));
            mLevel = (Level)info.GetValue("mLevel", typeof(Level));
            mCameraHandler = (CameraHandler)info.GetValue("mCameraHandler", typeof(CameraHandler));
            mCamera = mCameraHandler.mCamera;// (Camera)info.GetValue("mCamera", typeof(Camera));
            // mViewport = (Viewport)info.GetValue("mViewport", typeof(Viewport));
            // mRenderTarget = (RenderTarget)info.GetValue("mRenderTarget", typeof(RenderTarget));
            // mMasterRenderer = (MasterRenderer)info.GetValue("mMasterRenderer", typeof(MasterRenderer));
            // mContentManager= (ContentManager)info.GetValue("mContentManager", typeof(ContentManager));
            // mContentManager = (ContentManager)info.GetValue("mContentManager", typeof(ContentManager));
            mSpriteBatch = (SpriteBatch)info.GetValue("mSpriteBatch", typeof(SpriteBatch));
            mMenu = (Menu)info.GetValue("mMenu", typeof(Menu));
            mLabel = (Label)info.GetValue("mLabel", typeof(Label));
            mLevelPath = (string)info.GetValue("mLevelPath", typeof(string));
        }

        public GameScreen()
        {
            // mCameraHandler.mCamera = mCamera;
        }
    }
}