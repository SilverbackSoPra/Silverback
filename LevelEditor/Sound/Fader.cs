using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace LevelEditor.Sound
{

    public enum FadeState
    {
        FadeIn,
        FadeOut,
        Playing,
        Stopped
    }

    internal sealed class Fader
    {

        private const int FadeOutTime = 2000;
        private const int FadeInTime = 2000;

        private int mFadeTime;

        private readonly SoundEffectInstance mInstance;

        public FadeState mFadeState;

        public Fader(SoundEffectInstance instance)
        {

            mFadeState = FadeState.FadeIn;
            mFadeTime = 0;
            mInstance = instance;

        }

        public void FadeOut()
        {

            mFadeState = FadeState.FadeOut;
            mFadeTime = 0;
        }

        public void Update(GameTime gameTime, float volume)
        {

            if (mFadeState == FadeState.FadeIn)
            {
                mFadeTime += gameTime.ElapsedGameTime.Milliseconds;

                if (mFadeTime > FadeInTime)
                {
                    // fade in is over, now the song can be played
                    mFadeState = FadeState.Playing;
                    mInstance.Volume = volume;
                }
                else
                {
                    mInstance.Volume = mFadeTime / (float)FadeInTime * volume;
                }

            }

            if (mFadeState == FadeState.FadeOut)
            {
                mFadeTime += gameTime.ElapsedGameTime.Milliseconds;

                if (mFadeTime > FadeOutTime)
                {
                    // fade out is over, now the song have to be stopped
                    mFadeState = FadeState.Stopped;
                    mInstance.Volume = 0.0f;
                }
                else
                {
                    mInstance.Volume = (1.0f - mFadeTime / (float)FadeInTime) * volume;
                }
            }

            if (mFadeState == FadeState.Playing)
            {
                mInstance.Volume = volume;
            }

        }

    }

}
