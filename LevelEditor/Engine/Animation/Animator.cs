using System;
using System.Collections.Generic;
using LevelEditor.Engine.Mesh;
using Microsoft.Xna.Framework;

namespace LevelEditor.Engine.Animation
{
    internal sealed class Animator
    {

        private struct Attachment
        {
            private readonly Bone mBone;
            public readonly Actor mActor;

            public Attachment(Bone bone, Actor actor)
            {
                mBone = bone;
                mActor = actor;
            }

            public void Update(Matrix actorModelMatrix)
            {           
                mActor.ModelMatrix = mBone.mMatrix * actorModelMatrix;
            }

        }

        private readonly List<Animation> mAnimations;
        private readonly Bone[] mBones;

        public readonly Matrix[] mTransformations;

        private readonly List<AnimationActor> mAnimationActors;
        private readonly List<Attachment> mAttachments;

        private Animation mStandardAnimation;

        private readonly bool mIsSkinned;

        public Animator(Mesh.Mesh mesh)
        {

            mIsSkinned = mesh.mMeshData.mIsSkinned;

            if (mIsSkinned)
            {
                mBones = mesh.mMeshData.mBones;
                mAnimations = mesh.mMeshData.mAnimations;

                mTransformations = new Matrix[mBones.Length];
            }

            mAnimationActors = new List<AnimationActor>();
            mAttachments = new List<Attachment>();
            mStandardAnimation = null;

        }

        public void AttachActor(string boneName, Actor actor)
        {

            var bone = FindBone(boneName);
            mAttachments.Add(new Attachment(bone, actor));

        }

        public void DeattachActor(Actor actor)
        {

            var attachment = mAttachments.Find(ele => ele.mActor == actor);
            mAttachments.Remove(attachment);

        }

        public void SetStandardAnimation(string name)
        {
            mStandardAnimation = FindAnimation(name);
        }

        public void PlayAnimation(string name, bool loop, int fadeInTime)
        {

            var animation = FindAnimation(name);
            AnimationActor foundAnimationActor = null;
            
            foreach (var animationActor in mAnimationActors)
            {
                                
                if (animation == animationActor.mAnimation && animationActor.mFadeState != FadeState.FadeOut)
                {
                    // We don't want to play the animation twice
                    foundAnimationActor = animationActor;
                }
                else if (animation == animationActor.mAnimation && (animationActor.mFadeState == FadeState.FadeOut))
                {
                    // Here we just fade the animation in again
                    foundAnimationActor = animationActor;
                    animationActor.FadeIn(null);
                }
                else
                {
                    // The other animations we want to fade out
                    animationActor.FadeOut();
                }

            }

            if (foundAnimationActor == null)
            {
                mAnimationActors.Add(new AnimationActor(animation, loop, fadeInTime));
            }
            else
            {
                // The animation which is fading in should be at the last position in the list
                mAnimationActors.Remove(foundAnimationActor);
                mAnimationActors.Add(foundAnimationActor);
            }

        }

        public void Update(GameTime gameTime, Matrix actorModelMatrix, bool updateTransformations)
        {

            if (mIsSkinned && mAnimationActors.Count == 0)
            {
                
                if (mStandardAnimation != null)
                {
                    mAnimationActors.Add(new AnimationActor(mStandardAnimation, true, 500));
                }
                else
                {
                    return;
                }
            }

            if (!mIsSkinned)
            {
                return;
            }

            var upperAnimationActor = mAnimationActors[mAnimationActors.Count - 1];

            foreach (var animationActor in mAnimationActors.ToArray())
            {
                if (animationActor.mFadeState == FadeState.Waiting)
                {
                    animationActor.FadeIn(gameTime);
                }

                animationActor.Update(gameTime, upperAnimationActor.mMix);

                if (animationActor.mFadeState == FadeState.Stopped)
                {
                    mAnimationActors.Remove(animationActor);
                }                

            }

            // Actors which aren't visible shouldn't be calculated
            if (!updateTransformations)
            {
                return;
            }

            CalculateBonesTransformation();

            // Update the attachments
            foreach (var attachment in mAttachments)
            {

                attachment.Update(actorModelMatrix);

            }

        }

        private void CalculateBonesTransformation()
        {

            Vector3[] positions = new Vector3[mAnimationActors.Count];
            Quaternion[] rotations = new Quaternion[mAnimationActors.Count];
            Vector3[] scales = new Vector3[mAnimationActors.Count];

            for (var i = 0; i < mBones.Length; i++)
            {

                var bone = mBones[i];

                var totalMix = 0.0f;

                foreach (var animationActor in mAnimationActors)
                {
                    if (animationActor.mAnimation.mBoneKeys[i].mHasKeyFrames)
                    {
                        totalMix += animationActor.mMix;
                    }
                }

                if (totalMix > 0.0f)
                {
                    
                    var position = Vector3.Zero;
                    var rotation = Quaternion.Identity;
                    var scale = Vector3.Zero;

                    /*
                    To slerp more than 2 quaternions we need an approximation. We first calculate 
                    m_0 by using the following loop. Afterwards we could make the approximation better
                    by using the following formula: e_k-1 = sum_i(w_i * log((m_k-1)^(-1) * q_i). To get m_k
                    we use: m_k = m_k-1 * exp(e_k-1).
                    This means for every improvement in quality of the rotation we need to loop through all the
                    quaternions all over again. For simplification we could just allow for 2 animations at once.
                    For more information have a look at https://gamedev.stackexchange.com/questions/62354/method-for-interpolation-between-3-quaternions
                    */
                    for (var j = 0; j < mAnimationActors.Count; j++)
                    {

                        var animationActor = mAnimationActors[j];

                        if (animationActor.mAnimation.mBoneKeys[i].mHasKeyFrames)
                        {

                            animationActor.mAnimation.mBoneKeys[i].mKeyFrames.Interpolate(ref positions[j], ref rotations[j], ref scales[j], animationActor.mAnimationTime);

                            var mixValue = (animationActor.mMix / totalMix);

                            position += positions[j] * mixValue;
                            scale += scales[j] * mixValue;

                        }

                    }

                    var sum = 0.0f;

                    // Now we can approximate the quaternion. We could improve it by using the suggested formula from above.
                    for (var j = 0; j < mAnimationActors.Count; j++)
                    {
                        sum += mAnimationActors[j].mMix;
                        var amount = mAnimationActors[j].mMix / sum;
                        rotation = Quaternion.Slerp(rotation, rotations[j], amount);
                    }


                    var s = Matrix.CreateScale(scale);
                    var r = Matrix.CreateFromQuaternion(rotation);
                    var t = Matrix.CreateTranslation(position);

                    var matrix = r * t * s;


                    if (bone.mParent != null)
                    {
                        bone.mMatrix = matrix * bone.mParent.mMatrix;
                    }
                    else
                    {
                        bone.mMatrix = matrix;
                    }

                    mTransformations[i] = bone.mOffset * bone.mMatrix;

                }
                else
                {

                    if (bone.mParent != null)
                    {
                        bone.mMatrix = bone.mTransformation * bone.mParent.mMatrix;
                    }
                    else
                    {
                        bone.mMatrix = bone.mTransformation;
                    }

                    mTransformations[i] = bone.mOffset * bone.mMatrix;

                }

            }

        }

        private Animation FindAnimation(string name)
        {
            return mIsSkinned ? mAnimations.Find(ele => ele.mName == name) : null;
        }

        private Bone FindBone(string name)
        {
            return mIsSkinned ? Array.Find(mBones, ele => ele.mName == name) : null;
        }

    }

}
