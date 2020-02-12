using Microsoft.Xna.Framework;
using OpenTK.Audio.OpenAL;
using System;
using System.Runtime.Serialization;

namespace LevelEditor.Sound
{
    [Serializable()]
    public sealed class AudioSource : IDisposable
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

        public bool Relative
        {
            get
            {
                return mRelative;
            }
            set
            {
                AL.Source(mSource, ALSourceb.SourceRelative, value);
                mRelative = value;
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

        public Vector3 Direction
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
                mVolume = Math.Max(value, 0.0f);
                AL.Source(mSource, ALSourcef.Gain, mVolume * mVolumeMultiplier);
            }
        }

        public float VolumeMultiplier {
            get
            {
                return mVolumeMultiplier;
            }
            set
            {
                mVolumeMultiplier = Math.Min(Math.Max(value, 0.0f), 1.0f);
                AL.Source(mSource, ALSourcef.Gain, mVolume * mVolumeMultiplier);
            }
        }

        public float MinGain
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

        public float MaxGain
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

        public float ReferenceDistance
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

        public float RolloffFactor
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

        public float MaxDistance
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
        private bool mRelative;
        private Vector3 mLocation;
        private Vector3 mDirection;
        private float mPitch;
        private float mVolume;
        private float mVolumeMultiplier;

        // Attenuation attributes (depending on distance model)
        private float mMinGain;
        private float mMaxGain;
        private float mReferenceDistance;
        private float mRolloffFactor;
        private float mMaxDistance;

        private bool mDisposed = false;

        public AudioSource(AudioBuffer buffer)
        {

            mSource = AL.GenSource();
            mAudioBuffer = buffer;

            AL.Source(mSource, ALSourcei.Buffer, mAudioBuffer.GetId());

            Loop = false;
            Relative = false;
            Location = new Vector3(0.0f);
            Direction = new Vector3(0.0f);
            Pitch = 1.0f;
            Volume = 1.0f;
            VolumeMultiplier = 0.0f;

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

        private AudioSource()
        { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {

            if (mDisposed)
            {
                return;
            }

            if (disposing)
            {
                AL.SourceStop(mSource);
                AL.DeleteSource(mSource);
            }

            mDisposed = true;

        }

    }
}
