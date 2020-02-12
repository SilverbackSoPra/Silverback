using Microsoft.Xna.Framework;
using System;

namespace LevelEditor.Engine.Animation
{ 

    public enum FadeState
    {
        Waiting,
        FadeIn,
        FadeOut,
        Playing,
        Stopped
    }

    [Serializable()]
    internal sealed class AnimationActor
    {

        private int mFadeInTime;
        private int mFadeTime;

        private float mFadeOutWeight;

        private readonly bool mLoop;

        public FadeState mFadeState;

        public Animation mAnimation;
   
        public float mAnimationTime;
        private float mAnimationStartTime;

        public float mMix;

        private Animation mLastAnimation;

        public AnimationActor(Animation animation, bool loop, int fadeInTime)
        {

            mAnimation = animation;
            mLastAnimation = animation;
            mLoop = loop;

            mFadeInTime = fadeInTime;
            mFadeState = FadeState.Waiting;

        }

        public void FadeIn(GameTime gameTime)
        {

            // In the case that the actor is fading in for the first time
            if (mFadeState == FadeState.Waiting)
            {
                mAnimationStartTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
                mAnimationTime = 0.0f;

                mFadeTime = 0;
                
            }
            else if (mFadeState == FadeState.FadeOut)
            {

                mFadeTime = (int)(mMix * mFadeInTime);
                
            }

            mFadeState = FadeState.FadeIn;

        }

        public void FadeOut()
        {

            mFadeState = FadeState.FadeOut;
            mFadeOutWeight = mMix;

        }

        public void Update(GameTime gameTime, float upperElementMix)
        {

            if (mFadeState == FadeState.Waiting)
            {
                return;
            }

            if (mFadeState == FadeState.FadeIn)
            {

                mFadeTime += gameTime.ElapsedGameTime.Milliseconds;

                if (mFadeTime > mFadeInTime)
                {
                    mFadeState = FadeState.Playing;
                    mMix = 1.0f;
                }
                else
                {
                    mMix = mFadeTime / (float)mFadeInTime;
                }

                // An animation which isn't looping we want to first fade in before we start playing it
                if (!mLoop)
                {
                    mAnimationStartTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
                }

            }

            if (mFadeState == FadeState.FadeOut)
            {

                mMix = mFadeOutWeight * (1.0f - upperElementMix);

                if (!(mMix > 0.0f))
                {
                    mFadeState = FadeState.Stopped;
                    mMix = 0.0f;
                }

            }

            UpdateAnimationTime(gameTime);

        }

        private void UpdateAnimationTime(GameTime gameTime)
        {
            if (mAnimation == null)
            {
                return;
            }

            var timeInTicks = mAnimation.mTickPerSecond * ((float)gameTime.TotalGameTime.TotalMilliseconds - mAnimationStartTime) / 1000.0f;

            if (mLoop)
            {
                mAnimationTime = timeInTicks % mAnimation.mDuration;
            }
            else
            {
                mAnimationTime = Math.Min(timeInTicks, mAnimation.mDuration);
            }
        }

    }

}
