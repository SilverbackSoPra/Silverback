using LevelEditor.Engine;
using LevelEditor.Engine.Helper;
using LevelEditor.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.Serialization;

namespace LevelEditor
{
    [Serializable()]
    public sealed class CameraHandler: ISerializable
    {

        public float mRotationOffset;
        public bool mMoving;
        
        public Camera mCamera;

        // These vectors are the actual location and rotation of the camera
        // We use these because we want to have a smooth camera movement
        public Vector2 mRotation;

        public Vector3 mLocation;

        public float mDistance;
        
        public Vector2 mLastMousePosition;

        public readonly float mMouseSensibility;
        public readonly float mMovementSpeed;
        public readonly float mReactivity;

        public const float Pi = (float)Math.PI;
        public const float CameraRange = 20.0f;

        public float mScrollWheelMin = 1000.0f;
        public float mScrollWheelMax = 0.0f;

        public int mLastScrollWheelValue;

        private int mLastKeyDown = (int)Keys.W;

        private bool mLock = false;
        
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
            mDistance = mCamera.mThirdPersonDistance;

            mMouseSensibility = mouseSensibility;
            mMovementSpeed = movementSpeed;
            mReactivity = reactivity;

            mDistance = 10.0f;

            Unlock();

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
           
            if (mCamera.mThirdPerson)
            {
                mCamera.mThirdPersonDistance = MathExtension.Mix(mCamera.mThirdPersonDistance, mDistance, progress * 0.25f);
            }  

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
            mLastScrollWheelValue = mouseState.ScrollWheelValue;

            mScrollWheelMax = mLastScrollWheelValue;
            mScrollWheelMin = mScrollWheelMax + 1000.0f;

            mRotation = mCamera.mRotation;
            mLocation = mCamera.mLocation;

        }

        public void UpdateMouse(float deltatime)
        {

            var mouseState = Mouse.GetState();
            
            var mousePosition = new Vector2(mouseState.X, mouseState.Y);

            if (mouseState.ScrollWheelValue < mScrollWheelMax)
            {
                mScrollWheelMax = mouseState.ScrollWheelValue;
                mScrollWheelMin = mScrollWheelMax + 1000.0f;
            }
            else
            {
                if (mouseState.ScrollWheelValue - mLastScrollWheelValue > 0 && mouseState.ScrollWheelValue > mScrollWheelMin)
                {
                    mScrollWheelMin = mouseState.ScrollWheelValue;
                    mScrollWheelMax = mScrollWheelMin - 1000.0f;
                }
            }

            mDistance = (mScrollWheelMin - mouseState.ScrollWheelValue) / (mScrollWheelMin - mScrollWheelMax) * 7.0f + 3.0f;            

            mLastScrollWheelValue = mouseState.ScrollWheelValue;      
            
            if (InputManager.MouseLeftButtonPressed())
            {
                mLastMousePosition = mousePosition;
            }
            
            if (InputManager.MouseLeftButtonDown())
            {
                mRotation +=
                    new Vector2(mLastMousePosition.X - mousePosition.X, mLastMousePosition.Y - mousePosition.Y) *
                    mMouseSensibility * 0.001f;
                mLastMousePosition = mousePosition;
            }

        }

        public void UpdateKeyboard(float deltatime)
        {

            var keysPressed = 0;

            var wDown = InputManager.AllKeysDown(Keys.W);
            var sDown = InputManager.AllKeysDown(Keys.S);
            var aDown = InputManager.AllKeysDown(Keys.A);
            var dDown = InputManager.AllKeysDown(Keys.D);

            keysPressed = wDown ? keysPressed + 1 : keysPressed;
            keysPressed = sDown ? keysPressed + 1 : keysPressed;
            keysPressed = aDown ? keysPressed + 1 : keysPressed;
            keysPressed = dDown ? keysPressed + 1 : keysPressed;

            // We just want to allow one key pressed at a time
            if (keysPressed != 1)
            {                
                return;
            }

            if (wDown)
            {

                if (mLastKeyDown != (int)Keys.W)
                {
                    if (mLastKeyDown == (int)Keys.D)
                    {
                        mRotationOffset += Pi / 2.0f;
                    }
                    else if (mLastKeyDown == (int)Keys.S)
                    {
                        mRotationOffset += Pi;
                    }
                    else if (mLastKeyDown == (int)Keys.A)
                    {
                        mRotationOffset -= Pi / 2.0f;
                    }
                }

                mLocation += mCamera.Direction * deltatime / 1000.0f * mMovementSpeed;

                mMoving = true;

                mLastKeyDown = (int)Keys.W;

            }

            if (sDown)
            {
                if (mLastKeyDown == (int)Keys.D)
                {
                    mRotationOffset -= Pi / 2.0f;
                }
                else if (mLastKeyDown == (int)Keys.W)
                {
                    mRotationOffset -= Pi;
                }
                else if (mLastKeyDown == (int)Keys.A)
                {
                    mRotationOffset += Pi / 2.0f;
                }

                mLocation -= mCamera.Direction * deltatime / 1000.0f * mMovementSpeed;

                mMoving = true;

                mLastKeyDown = (int)Keys.S;

            }

            if (aDown)
            {
                if (mLastKeyDown == (int)Keys.D)
                {
                    mRotationOffset -= Pi;
                }
                else if (mLastKeyDown == (int)Keys.W)
                {
                    mRotationOffset += Pi / 2.0f;
                }
                else if (mLastKeyDown == (int)Keys.S)
                {
                    mRotationOffset -= Pi / 2.0f;
                }

                mLocation -= mCamera.Right * deltatime / 1000.0f * mMovementSpeed;

                mMoving = true;

                mLastKeyDown = (int)Keys.A;

            }

            if (dDown)
            {

                if (mLastKeyDown != (int)Keys.D)
                {
                    if (mLastKeyDown == (int)Keys.W)
                    {
                        mRotationOffset -= Pi / 2.0f;
                    }
                    else if (mLastKeyDown == (int)Keys.S)
                    {
                        mRotationOffset += Pi / 2.0f;
                    }
                    else if (mLastKeyDown == (int)Keys.A)
                    {
                        mRotationOffset += Pi;
                    }
                }

                mLocation += mCamera.Right * deltatime / 1000.0f * mMovementSpeed;

                mMoving = true;

                mLastKeyDown = (int)Keys.D;

            }

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("mRotationOffset", mRotationOffset);
            info.AddValue("mMoving", mMoving);
            info.AddValue("mCamera", mCamera);
            info.AddValue("mRotation", mRotation);
            info.AddValue("mLocation", mLocation);
            info.AddValue("mDistance", mDistance);
            info.AddValue("mLastMousePosition", mLastMousePosition);
            info.AddValue("mMouseSensibility", mMouseSensibility);
            info.AddValue("mMovementSpeed", mMovementSpeed);
            info.AddValue("mReactivity", mReactivity);
            info.AddValue("mScrollWheelMin", mScrollWheelMin);
            info.AddValue("mScrollWheelMax", mScrollWheelMax);
            info.AddValue("mLastScrollWheelValue", mLastScrollWheelValue);
            info.AddValue("mLock", mLock);
        }

        public CameraHandler(SerializationInfo info, StreamingContext context)
        {
            mRotationOffset = (float)info.GetValue("mRotationOffset", typeof(float));
            mMoving = (bool)info.GetValue("mMoving", typeof(bool));
            mCamera = (Camera)info.GetValue("mCamera", typeof(Camera));
            mRotation = (Vector2)info.GetValue("mRotation", typeof(Vector2));
            mLocation = (Vector3)info.GetValue("mLocation", typeof(Vector3));
            mDistance = (float)info.GetValue("mDistance", typeof(float));
            mLastMousePosition = (Vector2)info.GetValue("mLastMousePosition", typeof(Vector2));
            mMouseSensibility = (float)info.GetValue("mMouseSensibility", typeof(float));
            mMovementSpeed = (float)info.GetValue("mMovementSpeed", typeof(float));
            mReactivity = (float)info.GetValue("mReactivity", typeof(float));
            mScrollWheelMin = (float)info.GetValue("mScrollWheelMin", typeof(float));
            mScrollWheelMax = (float)info.GetValue("mScrollWheelMax", typeof(float));
            mLastScrollWheelValue = (int)info.GetValue("mLastScrollWheelValue", typeof(int));
            mLock = (bool)info.GetValue("mLock", typeof(bool));
        }

        public CameraHandler()
        { }

    }
}
