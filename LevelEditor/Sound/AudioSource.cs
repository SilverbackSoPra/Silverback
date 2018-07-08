using Microsoft.Xna.Framework;
using OpenTK.Audio.OpenAL;
using System;

namespace LevelEditor.Sound
{
    internal sealed class AudioSource
    {

        public enum AudioState
        {
            Initial,
            Playing,
            Paused,
            Stopped
        }

        public bool Loop {
            get
            {
                return mLoop;
            }
            set
            {
                AL.Source(mSource, ALSourceb.Looping, value);
                mLoop = value;
            }
        }

        public Vector3 Location
        {
            get
            {
                return mLocation;
            }
            set
            {
                mLocation = value;
                var location = new OpenTK.Vector3(value.X, value.Y, value.Z);
                AL.Source(mSource, ALSource3f.Position, ref location);
            }
        }

        private Vector3 Direction
        {
            get
            {
                return mDirection;
            }
            set
            {
                mDirection = value;
                var direction = new OpenTK.Vector3(value.X, value.Y, value.Z);
                AL.Source(mSource, ALSource3f.Direction, ref direction);
            }
        }

        public float Pitch
        {
            get
            {
                return mPitch;
            }
            set
            {
                mPitch = value;
                AL.Source(mSource, ALSourcef.Pitch, value);
            }
        }

        public float Volume
        {
            get
            {
                return mVolume;
            }
            set
            {
                mVolume = Math.Min(Math.Max(value, 0.0f), 1.0f);
                AL.Source(mSource, ALSourcef.Gain, mVolume);
            }
        }

        private float MinGain
        {
            get
            {
                return mMinGain;
            }
            set
            {
                mMinGain = value;
                AL.Source(mSource, ALSourcef.MinGain, value);
            }
        }

        private float MaxGain
        {
            get
            {
                return mMaxGain;
            }
            set
            {
                mMaxGain = value;
                AL.Source(mSource, ALSourcef.MaxGain, value);
            }
        }

        private float ReferenceDistance
        {
            get
            {
                return mReferenceDistance;
            }
            set
            {
                mReferenceDistance = value;
                AL.Source(mSource, ALSourcef.ReferenceDistance, value);
            }
        }

        private float RolloffFactor
        {
            get
            {
                return mRolloffFactor;
            }
            set
            {
                mRolloffFactor = value;
                AL.Source(mSource, ALSourcef.RolloffFactor, value);
            }
        }

        private float MaxDistance
        {
            get
            {
                return mMaxDistance;
            }
            set
            {
                mMaxDistance = value;
                AL.Source(mSource, ALSourcef.MaxDistance, value);
            }
        }

        private readonly AudioBuffer mAudioBuffer;
        private readonly int mSource;

        // General attributes
        private bool mLoop;
        private Vector3 mLocation;
        private Vector3 mDirection;
        private float mPitch;
        private float mVolume;

        // Attenuation attributes (depending on distance model)
        private float mMinGain;
        private float mMaxGain;
        private float mReferenceDistance;
        private float mRolloffFactor;
        private float mMaxDistance;        

        public AudioSource(AudioBuffer buffer)
        {

            mSource = AL.GenSource();
            mAudioBuffer = buffer;

            AL.Source(mSource, ALSourcei.Buffer, mAudioBuffer.GetId());

            Loop = false;
            Location = new Vector3(0.0f);
            Direction = new Vector3(0.0f);
            Pitch = 1.0f;
            Volume = Options.SoundEffectVolume / 100.0f;

            MinGain = 0.0f;
            MaxGain = 1.0f;
            ReferenceDistance = 1.0f;
            RolloffFactor = 1.0f;
            MaxDistance = 25.0f;

        }

        public void Play()
        {

            AL.SourcePlay(mSource);

        }

        public void Stop()
        {

            AL.SourceStop(mSource);

        }

        public AudioState GetState()
        {

            var sourceState = AL.GetSourceState(mSource);
            AudioState audioState;

            switch(sourceState)
            {
                case ALSourceState.Initial: audioState = AudioState.Initial; break;
                case ALSourceState.Playing: audioState = AudioState.Playing; break;
                case ALSourceState.Paused: audioState = AudioState.Paused; break;
                case ALSourceState.Stopped: audioState = AudioState.Stopped; break;
                default: audioState = AudioState.Initial; break;
            }

            return audioState;

        }

    }
}
