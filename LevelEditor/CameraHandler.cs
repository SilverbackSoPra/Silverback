using LevelEditor.Engine;
using LevelEditor.Engine.Helper;
using LevelEditor.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LevelEditor
{
    internal sealed class CameraHandler
    {

        public float mRotationOffset;
        public bool mMoving;

        private readonly Camera mCamera;

        // These vectors are the actual location and rotation of the camera
        // We use these because we want to have a smooth camera movement
        public Vector2 mRotation;

        public Vector3 mLocation;
        
        private Vector2 mLastMousePosition;

        private readonly float mMouseSensibility;
        private readonly float mMovementSpeed;
        private readonly float mReactivity;

        private const float Pi = (float)System.Math.PI;

        bool mLock = false;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="movementSpeed"></param>
        /// <param name="mouseSensibility"></param>
        /// <param name="reactivity"></param>
        public CameraHandler(Camera camera, float movementSpeed, float mouseSensibility, float reactivity)
        {
            mCamera = camera;

            mRotation = mCamera.mRotation;
            mLocation = mCamera.mLocation;

            mMouseSensibility = mouseSensibility;
            mMovementSpeed = movementSpeed;
            mReactivity = reactivity;

        }

        public void Update(float deltatime)
        {

            mMoving = false;

            if (mLock)
            {
                return;
            }

            float progress = MathExtension.Clamp(mReactivity * deltatime / 16.0f, 0.0f, 1.0f);

            // Update the mouse and keyboard
            UpdateMouse(deltatime);
            UpdateKeyboard(deltatime);

            // We mix the actual vectors with the camera vector to get a smoother movement
            mCamera.mRotation = MathExtension.Mix(mCamera.mRotation, mRotation, progress);
            mCamera.mLocation = MathExtension.Mix(mCamera.mLocation, mLocation, progress);           

        }

        public void Lock()
        {
            mLock = true;
        }

        public void Unlock()
        {
            mLock = false;
            var mouseState = Mouse.GetState();

            mLastMousePosition = new Vector2(mouseState.X, mouseState.Y);

            mRotation = mCamera.mRotation;
            mLocation = mCamera.mLocation;

        }

        private void UpdateMouse(float deltatime)
        {

            var mouseState = Mouse.GetState();
            
            var mousePosition = new Vector2(mouseState.X, mouseState.Y);
            
            if (InputManager.MouseLeftButtonPressed())
            {

                mLastMousePosition = mousePosition;
            }
            
            if (InputManager.MouseLeftButtonDown())
            {
                mRotation +=
                    new Vector2((mLastMousePosition.X - mousePosition.X), mLastMousePosition.Y - mousePosition.Y) *
                    mMouseSensibility * 0.001f;
                mLastMousePosition = mousePosition;
            }

        }

        private void UpdateKeyboard(float deltatime)
        {

            // if (Keyboard.GetState().IsKeyDown(Keys.W))
            if (InputManager.AllKeysDown(Keys.W))
            {
                mRotationOffset = 0.0f;

                mLocation += mCamera.Direction * deltatime / 1000.0f * mMovementSpeed;

                mMoving = true;

            }
            // if (Keyboard.GetState().IsKeyDown(Keys.S))
            if (InputManager.AllKeysDown(Keys.S))
            {
                mRotationOffset = Pi;

                mLocation -= mCamera.Direction * deltatime / 1000.0f * mMovementSpeed;

                mMoving = true;

            }
            // if (Keyboard.GetState().IsKeyDown(Keys.A))
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                mRotationOffset = Pi / 2.0f;

                mLocation -= mCamera.Right * deltatime / 1000.0f * mMovementSpeed;

                mMoving = true;

            }
            // if (Keyboard.GetState().IsKeyDown(Keys.D))
            if (InputManager.AllKeysDown(Keys.D))
            {
                mRotationOffset = Pi * 1.5f;

                mLocation += mCamera.Right * deltatime / 1000.0f * mMovementSpeed;

                mMoving = true;

            }

        }

    }
}
