using OpenTK.Audio.OpenAL;
using System;
using NAudio.Wave;

namespace LevelEditor.Sound
{
    internal sealed class Wave
    {
        private readonly int mChannels;
        private readonly int mBitsPerSample;
        public readonly int mSampleRate;

        public readonly byte[] mData;

        public Wave(string filename)
        {

            using (var reader = new WaveFileReader(filename))
            {

                mData = new byte[reader.Length];
                var read = reader.Read(mData, 0, mData.Length);
                mBitsPerSample = reader.WaveFormat.BitsPerSample;
                mChannels = reader.WaveFormat.Channels;
                mSampleRate = reader.WaveFormat.SampleRate;

                reader.Close();

            }

        }

        public ALFormat GetFormat()
        {

            switch (mChannels)
            {
                case 1: return mBitsPerSample == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return mBitsPerSample == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }

        }

    }

}
