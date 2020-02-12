using System;
using LevelEditor.Engine;
using LevelEditor.Engine.Mesh;
using LevelEditor.Engine.Renderer;
using LevelEditor.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LevelEditor.Sound;

namespace LevelEditor.Screen
{
    
    /// <summary>
    /// Later on we want to create a game screen per level. This is why we hand over the path to the level file in the constructor
    /// For now 
    /// </summary>
    internal sealed class EditorScreen : IScreen
    {

        public ScreenManager ScreenManager { get; set; }
        public SoundManager SoundManager { get; set; }
        public bool IsVisible { get; set; }

        public Level mLevel;
        public Mesh mSelectedMesh;
        public Actor mSelectedActor;
        public bool mSelectMode;
        public bool mSetOnce;
        private bool mShiftDown;
        private bool mRightMouseButtonDown;
        private bool mDeleteDown;
        private float mActorYRotation;
        private float mActorYOffset;

        public Camera mCamera;
        private CameraHandler mCameraHandler;
        private Viewport mViewport;

        private RenderTarget mRenderTarget;

        public MasterRenderer mMasterRenderer;

        private ContentManager mContentManager;

        public void LoadContent(GraphicsDeviceManager deviceManager, ContentManager contentManager, int windowWidth, int windowHeight)
        {

            var graphicsDevice = deviceManager.GraphicsDevice;

            // Create a new content manager so we can unload all its content afterwards
            mContentManager = new ContentManager(contentManager.ServiceProvider, contentManager.RootDirectory);

            mLevel = new Level(mContentManager, SoundManager, graphicsDevice, true);

            mRenderTarget = new RenderTarget(graphicsDevice, Options.ResolutionWidth, Options.ResolutionHeight, Options.ShadowResolution);
            mMasterRenderer = new MasterRenderer(graphicsDevice, mContentManager) {PrePass = false, DebugMode = false};

            mCamera = new Camera()
            {
                mFarPlane = Options.ViewingDistance,
                mLocation = {Y = 20.0f}
            };
            mCameraHandler = new CameraHandler(mCamera, 40.0f, 2.0f, .3f);

            mCamera.UpdatePerspective();
            mCamera.mLocation = new Vector3(0.0f, 20.0f, -10.0f);

            mSelectMode = true;

        }

        public void UnloadContent()
        {

            mRenderTarget.Dispose();
            mLevel.Dispose();
            mContentManager.Dispose();

        }

        public void Update(GameTime gameTime)
        {

            if (mSelectedMesh != null && mSelectedActor == null && !mSelectMode)
            {
                mSelectedActor = new Actor(mSelectedMesh);
                mSelectedActor.Color = new Vector3(.3f, 0.3f, 0.3f);
                mLevel.Add(mSelectedActor);
            }

            SoundManager.Update(gameTime, mLevel, mCamera);

            mCameraHandler.Update(gameTime.ElapsedGameTime.Milliseconds);

            var state = Keyboard.GetState();

            // if (state.IsKeyDown(Keys.LeftShift))
            if (InputManager.AllKeysDown(Keys.LeftShift))
            {
                mShiftDown = true;
            }

            // if (state.IsKeyUp(Keys.LeftShift) && mShiftDown)
            if (InputManager.AllKeysUp(Keys.LeftShift) && mShiftDown)
            {
                if (mSelectMode)
                {
                    if (mLevel.mActorBatches.Count > 1)
                    {
                        mSelectedMesh = mLevel.mActorBatches[0].mMesh == mLevel.mTerrain.Actor.mMesh ? mLevel.mActorBatches[1].mMesh : mLevel.mActorBatches[0].mMesh;
                    }
                    mSelectMode = false;
                }
                else
                {
                    mSelectMode = true;
                    mLevel.Remove(mSelectedActor);
                    mSelectedActor = null;
                    mSelectedMesh = null;
                }

                mShiftDown = false;

            }

            // if (state.IsKeyDown(Keys.Left))
            if (InputManager.AllKeysDown(Keys.Left))
            {
                mActorYRotation -= 0.005f * gameTime.ElapsedGameTime.Milliseconds;
            }

            // if (state.IsKeyDown(Keys.Right))
            if (InputManager.AllKeysDown(Keys.Right))
            {
                mActorYRotation += 0.005f * gameTime.ElapsedGameTime.Milliseconds;
            }

            // if (state.IsKeyDown(Keys.Up))
            if (InputManager.AllKeysDown(Keys.Up))
            {
                mActorYOffset += 0.005f * gameTime.ElapsedGameTime.Milliseconds;
            }

            // if (state.IsKeyDown(Keys.Down))
            if (InputManager.AllKeysDown(Keys.Down))
            {
                mActorYOffset -= 0.005f * gameTime.ElapsedGameTime.Milliseconds;
            }

            // if (state.IsKeyDown(Keys.Delete) && mSelectMode && mSelectedActor != null)
            if (InputManager.AllKeysDown(Keys.Delete) && mSelectMode && mSelectedActor != null)
            {
                mDeleteDown = true;
            }

            // if (state.IsKeyUp(Keys.Delete) && mDeleteDown)
            if (InputManager.AllKeysUp(Keys.Delete) && mDeleteDown)
            {
                if (mSelectedActor != null)
                {
                    mLevel.Remove(mSelectedActor);
                }
                mSelectedActor = null;
            }
            
            var mouseState = Mouse.GetState();

            if (!mSelectMode)
            {

                var intersection =
                    RayCasting.CalculateMouseRayTerrainIntersection(mViewport, mCamera, mLevel.mTerrain);

                if (intersection.mIntersected && mSelectedActor != null)
                {
                    intersection.mLocation.Y += mActorYOffset;
                    var matrix = Matrix.CreateRotationY(mActorYRotation) *
                                 Matrix.CreateTranslation(intersection.mLocation);
                    mSelectedActor.ModelMatrix = matrix;

                    // if (mouseState.RightButton == ButtonState.Pressed)
                    if (InputManager.MouseRightButtonPressed())
                    {
                        mRightMouseButtonDown = true;
                    }

                    // if (mouseState.RightButton != ButtonState.Pressed && mRightMouseButtonDown)
                    if (InputManager.MouseRightButtonReleased() && mRightMouseButtonDown)
                    {
                        mSelectedActor.Color = new Vector3(1.0f);
                        mSelectedActor = null;
                        mRightMouseButtonDown = false;
                        if (mSetOnce)
                        {
                            mSetOnce = false;
                            mSelectMode = true;
                            mSelectedMesh = null;
                        }
                    }
                }

            }
            else
            {

                // if (mouseState.RightButton == ButtonState.Pressed && mSelectedActor == null && !mDeleteDown)
                if (InputManager.MouseRightButtonPressed() && mSelectedActor == null && !mDeleteDown)
                {
                    var intersection =
                        RayCasting.CalculateMouseRayIntersection(mViewport, mCamera, mLevel, mLevel.mTerrain);

                    if (intersection.mIntersected && !intersection.mIsTerrainIntersection)
                    {
                        mSelectedActor = intersection.mActor;
                        mSelectedActor.Color = new Vector3(0.3f);
                        mActorYRotation = 0.0f;
                        mActorYOffset = 0.0f;
                        mSelectedMesh = mSelectedActor.mMesh;
                    }

                }

                // We need to check if the button is still pressed
                if (Mouse.GetState().RightButton == ButtonState.Pressed && mSelectedActor != null)
                {
                    var intersection =
                        RayCasting.CalculateMouseRayTerrainIntersection(mViewport, mCamera, mLevel.mTerrain);

                    if (intersection.mIntersected)
                    {
                        intersection.mLocation.Y += mActorYOffset;
                        mSelectedActor.ModelMatrix = Matrix.CreateRotationY(mActorYRotation) * Matrix.CreateTranslation(intersection.mLocation);
                    }
                }

                // if (mouseState.RightButton == ButtonState.Released)
                if (InputManager.MouseRightButtonReleased())
                {
                    if (mSelectedActor != null)
                    {
                        mSelectedActor.Color = new Vector3(1.0f);
                    }
                    mSelectedActor = null;
                    mDeleteDown = false;
                }
        

            }
            
            mLevel.Update(mCamera, mCameraHandler, gameTime);

        }

        public void Render(GameTime gameTime)
        {

            mMasterRenderer.Render(mViewport, mRenderTarget, mCamera, mLevel);

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
