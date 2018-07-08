using Microsoft.Xna.Framework;
using System;

namespace LevelEditor.Engine
{

    /// <summary>
    /// Represents a shadow which can be attached to a light.
    /// </summary>
    internal sealed class Shadow
    {

        private readonly Light mLight;

        public float mDistance;
        public readonly float mBias;

        public int mNumSamples;
        public readonly float mSampleRange;

        public Matrix mViewMatrix;
        public Matrix mProjectionMatrix;

        public Vector3 mMinProj;
        public Vector3 mMaxProj;
        public Vector3 mShadowCenter;

        public bool mActivated;

        /// <summary>
        /// Constructs a <see cref="Shadow"/>
        /// </summary>
        /// <param name="light">The light where the shadow is attached to.</param>
        public Shadow(Light light)
        {

            mLight = light;

            mDistance = 60.0f;
            mBias = 0.002f;
            mNumSamples = 8;
            mSampleRange = 0.4f;

            mProjectionMatrix = Matrix.CreateOrthographicOffCenter(-25.0f, 25.0f, -25.0f, 25.0f, -100.0f, 100.0f);
            mViewMatrix = Matrix.Identity;

            mMinProj = new Vector3();
            mMaxProj = new Vector3();
            mShadowCenter = new Vector3();

            mActivated = true;

        }

        /// <summary>
        /// Updates the shadow matrices according to the cameras view frustum.
        /// </summary>
        /// <param name="camera">The camera which is used in the scene</param>
        public void Update(Camera camera)
        {

            /*
             * Here is a quick overview of what this method does:
             * First we create the middle point of the shadow map in view space
             * Afterwards we use this information to create a light view matrix.
             * We then calulcate the widths and heights of the nearPlane and the
             * projected shadow distance plane. We then calculate all 8 points of
             * the resulting frustum. After we've done that we loop trough all of
             * these points, transform them into light view space and check the for
             * the mins and max of the X, Y, and Z components of these points. These
             * mins and maxs are needed to calculate the light projection matrix which
             * should fit the view frustum perfectly now.
             */
            var cameraLocation = camera.mThirdPerson ? camera.mLocation - camera.Direction * camera.mThirdPersonDistance : camera.mLocation;

            mShadowCenter = cameraLocation + camera.Direction * mDistance / 2.0f;

            var direction = Vector3.Normalize(mShadowCenter - mLight.mLocation);

            mViewMatrix = Matrix.CreateLookAt(mShadowCenter, mShadowCenter + direction, Vector3.Up);

            var tang = (float)Math.Tan(camera.mFieldOfView * Math.PI / 360.0f);

            var farPlaneHeight = mDistance * tang;
            var farPlaneWidth = camera.mAspectRatio * farPlaneHeight;

            var nearPlaneHeight = camera.mNearPlane * tang;
            var nearPlaneWidth = camera.mAspectRatio * nearPlaneHeight;

            var far = cameraLocation + camera.Direction * mDistance;
            var near = cameraLocation + camera.Direction * camera.mNearPlane;

            var farUpperLeft = far + farPlaneHeight * camera.Up - farPlaneWidth * camera.Right;
            var farUpperRight = far + farPlaneHeight * camera.Up + farPlaneWidth * camera.Right;
            var farLowerLeft = far - farPlaneHeight * camera.Up - farPlaneWidth * camera.Right;
            var farLowerRight = far - farPlaneHeight * camera.Up + farPlaneWidth * camera.Right;

            var nearUpperLeft = near + nearPlaneHeight * camera.Up - nearPlaneWidth * camera.Right;
            var nearUpperRight = near + nearPlaneHeight * camera.Up + nearPlaneWidth * camera.Right;
            var nearLowerLeft = near - nearPlaneHeight * camera.Up - nearPlaneWidth * camera.Right;
            var nearLowerRight = near - nearPlaneHeight * camera.Up + nearPlaneWidth * camera.Right;

            var array = new[] {farUpperLeft, farUpperRight, farLowerLeft, farLowerRight, nearUpperLeft, nearUpperRight, nearLowerLeft, nearLowerRight};

            var vec = Vector3.Transform(farUpperLeft, mViewMatrix);

            mMinProj = vec;
            mMaxProj = vec;

            foreach (var vector in array)
            {

                var transform = Vector3.Transform(vector, mViewMatrix);

                if (transform.X > mMaxProj.X)
                {
                    mMaxProj.X = transform.X;
                }
                if (transform.X < mMinProj.X)
                {
                    mMinProj.X = transform.X;
                }
                if (transform.Y > mMaxProj.Y)
                {
                    mMaxProj.Y = transform.Y;
                }
                if (transform.Y < mMinProj.Y)
                {
                    mMinProj.Y = transform.Y;
                }
                if (transform.Z > mMaxProj.Z)
                {
                    mMaxProj.Z = transform.Z;
                }
                if (transform.Z < mMinProj.Z)
                {
                    mMinProj.Z = transform.Z;
                }

            }

            /*
             * We have to reduce -z even further to get the shadows nearer to light rendered correctly
             * To be honest I don't even know why we have to use -maxZ for -z and -minZ for z. My guess
             * is that it has to do with the different orientation of the coordinate system in DirectX.
             */
            mProjectionMatrix = Matrix.CreateOrthographicOffCenter(mMinProj.X, mMaxProj.X, mMinProj.Y, mMaxProj.Y, -mMaxProj.Z - 150.0f, -mMinProj.Z);

        }

    }
}
