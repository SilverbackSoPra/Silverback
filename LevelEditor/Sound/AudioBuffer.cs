using OpenTK.Audio.OpenAL;

namespace LevelEditor.Sound
{
    internal sealed class AudioBuffer
    {
        private readonly int mBufferId;

        public AudioBuffer(string wavFilename)
        {
            mBufferId = AL.GenBuffer();
            var waveFile = new Wave(wavFilename);
          
            AL.BufferData(mBufferId, waveFile.GetFormat(), waveFile.mData, waveFile.mData.Length, waveFile.mSampleRate);

        }

        public int GetId()
        {
            return mBufferId;
        }

    }
}
