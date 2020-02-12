using OpenTK.Audio.OpenAL;
using System;

namespace LevelEditor.Sound
{
    public sealed class AudioBuffer : IDisposable
    {

        private readonly int mBufferId;
        private Wave mWave;

        private bool mDisposed = false;

        public AudioBuffer(string wavFilename)
        {
            mBufferId = AL.GenBuffer();
            mWave = new Wave(wavFilename);
          
            AL.BufferData(mBufferId, mWave.GetFormat(), mWave.mData, mWave.mData.Length, mWave.mSampleRate);

        }

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
                AL.DeleteBuffer(mBufferId);
            }

            mDisposed = true;

        }

        public int GetId()
        {
            return mBufferId;
        }

    }
}
