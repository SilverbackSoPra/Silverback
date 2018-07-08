using System;
using Microsoft.Xna.Framework;

namespace LevelEditor.Engine.Animation
{
    internal sealed class Animation
    {
        public struct BoneKey
        {
            public BoneKeyFrames mKeyFrames;
            public bool mHasKeyFrames;
        }

        public string Path { get; set; }

        public readonly string mName;

        public readonly float mDuration;
        public readonly float mTickPerSecond;

        public readonly BoneKey[] mBoneKeys;

        public Animation(Assimp.Animation animation, Bone[] bones)
        {

            // We need to repair the string the assimp wrapper gives us
            var cutoffIndex = animation.Name.LastIndexOf('|') + 1;
            mName = animation.Name.Substring(cutoffIndex);

            mDuration = (float)animation.DurationInTicks;
            mTickPerSecond = (float)animation.TicksPerSecond;

            mBoneKeys = new BoneKey[bones.Length];

            for (var i = 0; i < animation.NodeAnimationChannelCount; i++)
            {

                // We want one additional key to store the bind pose
                var positionKeys = animation.NodeAnimationChannels[i].PositionKeyCount + 1;
                var rotationKeys = animation.NodeAnimationChannels[i].RotationKeyCount + 1;
                var scaleKeys = animation.NodeAnimationChannels[i].ScalingKeyCount + 1;

                var keyFrames = new BoneKeyFrames(positionKeys, rotationKeys, scaleKeys);

                // Copy the position keys
                for (var j = 0; j < positionKeys - 1; j++)
                {
                    var position = animation.NodeAnimationChannels[i].PositionKeys[j].Value;
                    keyFrames.mPositionKeys[j].mPosition = new Vector3(position.X, position.Y, position.Z);
                    keyFrames.mPositionKeys[j].mTime = (float)animation.NodeAnimationChannels[i].PositionKeys[j].Time;
                }

                // Copy the position keys
                for (var j = 0; j < rotationKeys - 1; j++)
                {
                    var quaternion = animation.NodeAnimationChannels[i].RotationKeys[j].Value;
                    keyFrames.mRotationKeys[j].mRotation = new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
                    keyFrames.mRotationKeys[j].mTime = (float)animation.NodeAnimationChannels[i].RotationKeys[j].Time;
                }

                // Copy the scale keys
                for (var j = 0; j < scaleKeys - 1; j++)
                {
                    var scale = animation.NodeAnimationChannels[i].ScalingKeys[j].Value;
                    keyFrames.mScaleKeys[j].mScale = new Vector3(scale.X, scale.Y, scale.Z);
                    keyFrames.mScaleKeys[j].mTime = (float)animation.NodeAnimationChannels[i].ScalingKeys[j].Time;
                }

                // Find the bone corresponding to the NodeChannel
                var index = Array.FindIndex(bones, ele => ele.mName == animation.NodeAnimationChannels[i].NodeName);

                // Set the binding pose
                keyFrames.mPositionKeys[positionKeys - 1].mPosition = bones[index].mTransformation.Translation;
                keyFrames.mRotationKeys[rotationKeys - 1].mRotation = bones[index].mTransformation.Rotation;
                keyFrames.mScaleKeys[scaleKeys - 1].mScale = bones[index].mTransformation.Scale;

                mBoneKeys[index].mKeyFrames = keyFrames;
                mBoneKeys[index].mHasKeyFrames = true;

            }

        }

    }
}
