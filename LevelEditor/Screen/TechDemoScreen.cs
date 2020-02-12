using System;
using LevelEditor.Engine;
using LevelEditor.Engine.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LevelEditor.Sound;
using System.Diagnostics;
using LevelEditor.UIv2;
using LevelEditor.Collision;
using System.Collections.Generic;
using LevelEditor.Engine.Loader;
using LevelEditor.Engine.Mesh;
using System.IO;
using System.Windows.Forms;
using LevelEditor.Objects.Ape;
using LevelEditor.Objects;
using LevelEditor.Objects.Ape.SubApe;
using LevelEditor.Events;

namespace LevelEditor.Screen
{

    /// <summary>
    /// The actual Screen for the Levels and not the Level Editor
    /// 
    /// Later on we want to create a game screen per level. This is why we hand over the path to the level file in the constructor
    /// For now 
    /// </summary>
    public sealed class TechDemoScreen : IScreen
    {

        public ScreenManager ScreenManager { get; set; }

        public SoundManager SoundManager { get; set; }
        public bool IsVisible { get; set; }

        private Camera mCamera;
        private CameraHandler mCameraHandler;
        private Viewport mViewport;
        private Scene mScene;

        private RenderTarget mRenderTarget;

        private MasterRenderer mMasterRenderer;

        private ContentManager mContentManager;

        private SpriteBatch mSpriteBatch;
        private UIv2.Menu mMenu;
        private UIv2.Components.Label mLabel;

        private const int ObstacleCount = 100;
        private const float ObstacleRadius = 10.0f;

        private Capuchin mSubApe;

        private Silverback mSilverback;

        public void LoadContent(GraphicsDeviceManager deviceManager,
            ContentManager contentManager,
            int windowWidth,
            int windowHeight)
        {

            var currentDirectoryPath = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory("../../../../Content");

            var graphicsDevice = deviceManager.GraphicsDevice;

            // Create a new content manager so we can unload all its content later
            mContentManager = new ContentManager(contentManager.ServiceProvider, contentManager.RootDirectory);


            mSpriteBatch = new SpriteBatch(graphicsDevice);

            var font = mContentManager.Load<SpriteFont>("Font");
            var modelLoader = new ModelLoader(graphicsDevice);

            var texture2D = UIv2.Menu.CreateTexture2D(graphicsDevice,
               50,
               30,
               pixel => Color.Black);

            mMenu = new UIv2.Menu(graphicsDevice, 0, 0, 100, 4);
            mMenu.NonSolid();
            mLabel = new UIv2.Components.Label(graphicsDevice, 0, 0, 25, 100, "", font, Color.Orange);
            mLabel.AddTo(mMenu);

            var backButton = new UIv2.Components.Button(graphicsDevice, 90, 0, 10, 100, texture2D, "Back", font, Color.White);
            backButton.AddTo(mMenu);

            backButton.AddListener(MouseButtons.Left, InputState.Pressed, () =>
            {
                ScreenManager.Remove(this);
                IsVisible = false;
            });

            mRenderTarget = new RenderTarget(graphicsDevice,
                Options.ResolutionWidth,
                Options.ResolutionHeight,
                1);
            mMasterRenderer = new MasterRenderer(graphicsDevice, mContentManager) {DebugMode = false};

            mCamera = new Camera(farPlane: Options.ViewingDistance,
                nearPlane: 0.5f,
                thirdPerson: true,
                location: new Vector3(0.0f, 0.0f, -110.0f));
            mCameraHandler = new CameraHandler(mCamera, 4.0f, 2.0f, .3f);

            mScene = new Scene();

            var staticObjectsRectangles = new List<CollisionRectangle>();

            mScene.mQuadTree = new QuadTree<Actor>(new Rectangle(-128, -128, 256, 256), 4, 10);

            mScene.mTerrain = new Terrain(mContentManager,
                graphicsDevice,
                "Terrain/level1 heightmap.png",
                "Terrain/techdemoTexture.png");
            mScene.Add(mScene.mTerrain);

            var obstacleMesh = modelLoader.LoadMesh("Mesh/beerbottle2.obj");

            var random = new Random();

            for (var i = 0; i < ObstacleCount; i++)
            {

                var x = (2.0f * (float)random.NextDouble() - 1.0f) * ObstacleRadius;
                var z = (2.0f * (float)random.NextDouble() - 1.0f) * ObstacleRadius;

                var vector = new Vector3(x, 0.0f, z) + new Vector3(20.0f, 0.0f, -100.0f);

                vector.Y = mScene.mTerrain.GetHeight(vector);

                var actor = new Actor(obstacleMesh) { ModelMatrix = Matrix.CreateTranslation(vector) };

                mScene.Add(actor);
                mScene.mQuadTree.Insert(actor, actor.mBoundingRectangle.GetAxisAlignedRectangle(1));
                staticObjectsRectangles.Add(actor.mBoundingRectangle);

            }

            mScene.mVisibilityGraph = new Pathfinding.VisibilityGraph(staticObjectsRectangles, new Rectangle(-128, -128, 256, 256), 0.0f, false);

            var hutMesh = modelLoader.LoadMesh("Mesh/spawningcabin_scaled 0.015.fbx");

            var lumberjackMesh = modelLoader.LoadMesh("Mesh/lumberjack_minimal_noanimation.obj");

            var doubleAxeKillerMesh = modelLoader.LoadMesh("Mesh/lumberjack_distance_idle.fbx");
            modelLoader.LoadAnimation(doubleAxeKillerMesh, "Mesh/lumberjack_distance_walk.fbx");
            modelLoader.LoadAnimation(doubleAxeKillerMesh, "Mesh/lumberjack_distance_hit.fbx");
            modelLoader.LoadAnimation(doubleAxeKillerMesh, "Mesh/lumberjack_distance_run.fbx");

            var axeMesh = modelLoader.LoadMesh("Mesh/axe.fbx");

            var silverbackMesh = modelLoader.LoadMesh("Mesh/gorilla_idle.fbx");
            modelLoader.LoadAnimation(silverbackMesh, "Mesh/gorilla_walking.fbx");
            modelLoader.LoadAnimation(silverbackMesh, "Mesh/gorilla_smash.fbx");

            var capuchinMesh = modelLoader.LoadMesh("Mesh/kapuziner_idle.fbx");
            modelLoader.LoadAnimation(capuchinMesh, "Mesh/kapuziner_walk.fbx");
            modelLoader.LoadAnimation(capuchinMesh, "Mesh/kapuziner_heal.fbx");

            mSilverback = new Silverback(new Vector3(0.0f, 0.0f, -110.0f), new Vector2(0.0f), silverbackMesh);
            mScene.Add(mSilverback);

            var otherSilverback = new Silverback(new Vector3(0.0f, 0.0f, -110.0f), new Vector2(0.0f), silverbackMesh);

            mSubApe = new Capuchin(capuchinMesh, mScene.mTerrain, otherSilverback, mScene, ref random);
            mScene.Add(mSubApe);

            var hut = new Hut(hutMesh, lumberjackMesh, doubleAxeKillerMesh, axeMesh, mSilverback, mScene, 1000, 100.0f, 110.0f, true);

            mScene.Add(hut);

            mScene.mPostProcessing.mBloom.Activated = false;
            mScene.mPostProcessing.mFxaa.Activated = false;
            mScene.mSky.Light.mShadow.mActivated = false;

            mCamera.UpdatePerspective();

            Directory.SetCurrentDirectory(currentDirectoryPath);

        }

        public void UnloadContent()
        {

            mRenderTarget.Dispose();
            mMasterRenderer.Dispose();
            mContentManager.Dispose();

        }

        public void Update(GameTime gameTime)
        {

            mMenu.CheckRegisteredEvents();

            mCameraHandler.Update(gameTime.ElapsedGameTime.Milliseconds);

            mCamera.mLocation.Y = mScene.mTerrain.GetHeight(mCamera.mLocation) + 1.5f;

            // We need to calculate the collision of the camera with the terrain
            var location = mCamera.mLocation - mCamera.mThirdPersonDistance * mCamera.Direction;

            var height = mScene.mTerrain.GetHeight(location);
            var relativeHeight = height - mCamera.mLocation.Y + 1.0f;
            var terrainAngle = Math.Sinh(relativeHeight / mCamera.mThirdPersonDistance);

            if (mCamera.mRotation.Y < terrainAngle)
            {
                mCamera.mRotation.Y = (float)terrainAngle;
                mCameraHandler.mRotation.Y = (float)terrainAngle;
            }

            if (mCamera.mRotation.Y > Math.PI / 3.5f)
            {
                mCamera.mRotation.Y = (float)Math.PI / 3.5f;
                mCameraHandler.mRotation.Y = (float)Math.PI / 3.5f;
            }

            mScene.Update(mCamera, gameTime);

            mSilverback.Update(mScene.mTerrain, mCamera, mCameraHandler, mScene, gameTime);

            mLabel.Text = gameTime.ElapsedGameTime.Milliseconds + "ms 1002 movable actors";
            
            if (InputManager.MouseRightButtonPressed())
            {
                var raycast = RayCasting.CalculateMouseRayTerrainIntersection(mViewport, mCamera, mScene.mTerrain);

                if (!raycast.mIntersected)
                {
                    return;
                }

                mSubApe.SetTarget(raycast.mLocation);

            }

        }

        public void Render(GameTime gameTime)
        {

            mMasterRenderer.Render(mViewport, mRenderTarget, mCamera, mScene);

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

    }

}