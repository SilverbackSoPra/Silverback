using LevelEditor.Engine.Helper;
using Microsoft.Xna.Framework;

namespace LevelEditor.Engine.Animation
{
    internal sealed class BoneKeyFrames
    {

        public struct PositionKey
        {
            public Vector3 mPosition;
            public float mTime;
        }

        public struct RotationKey
        {
            public Quaternion mRotation;
            public float mTime;
        }

        public struct ScaleKey
        {
            public Vector3 mScale;
            public float mTime;
        }

        public readonly PositionKey[] mPositionKeys;
        public readonly RotationKey[] mRotationKeys;
        public readonly ScaleKey[] mScaleKeys;

        public BoneKeyFrames(int positionKeyCount, int rotationKeyCount, int scaleKeyCount)
        {

            mPositionKeys = new PositionKey[positionKeyCount];
            mRotationKeys = new RotationKey[rotationKeyCount];
            mScaleKeys = new ScaleKey[scaleKeyCount];

        }

        public void Interpolate(ref Vector3 position, ref Quaternion rotation, ref Vector3 scale, float animationTime)
        {

            InterpolatePositionKeys(ref position, animationTime);
            InterpolateRotationKeys(ref rotation, animationTime);
            InterpolateScaleKeys(ref scale, animationTime);

        }

        private void InterpolatePositionKeys(ref Vector3 position, float animationTime)
        {

            var keys = mPositionKeys;
            var keysCount = keys.Length;

            for (var i = 0; i < keysCount - 2; i++)
            {
                if (animationTime >= keys[i + 1].mTime)
                {
                    continue;
                }

                var mix = (animationTime - keys[i].mTime) / (keys[i + 1].mTime - keys[i].mTime);
                position = MathExtension.Mix(keys[i].mPosition, keys[i + 1].mPosition, mix);

                return;

            }

        }

        private void InterpolateRotationKeys(ref Quaternion rotation, float animationTime)
        {

            var keys = mRotationKeys;
            var keysCount = keys.Length;

            for (var i = 0; i < keysCount - 2; i++)
            {
                if (animationTime >= keys[i + 1].mTime)
                {
                    continue;
                }

                var mix = (animationTime - keys[i].mTime) / (keys[i + 1].mTime - keys[i].mTime);
                rotation = Quaternion.Slerp(keys[i].mRotation, keys[i + 1].mRotation, mix);

                return;

            }

        }

        private void InterpolateScaleKeys(ref Vector3 scale, float animationTime)
        {

            var keys = mScaleKeys;
            var keysCount = keys.Length;

            for (var i = 0; i < keysCount - 2; i++)
            {
                if (animationTime >= keys[i + 1].mTime)
                {
                    continue;
                }

                var mix = (animationTime - keys[i].mTime) / (keys[i + 1].mTime - keys[i].mTime);
                scale = MathExtension.Mix(keys[i].mScale, keys[i + 1].mScale, mix);

                return;

            }

        }

    }
}
