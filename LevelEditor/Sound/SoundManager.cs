using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using LevelEditor.Engine;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System;

/// <summary>
/// For more informations how the sound system is build up: https://www.openal.org/documentation/openal-1.1-specification.pdf
/// </summary>
namespace LevelEditor.Sound
{

    /// <summary>
    /// https://www.openal.org/documentation/openal-1.1-specification.pdf
    /// </summary>
    public enum AudioDistanceModel
    {
        LinearDistance,
        LinearDistanceClamped,
        InverseDistance,
        InverseDistanceClamped,
        None
    }

    /// <summary>
    /// Used to manage the sounds and background music.
    /// </summary>
    public sealed class SoundManager
    {
        private AudioDistanceModel DistanceModel
        {
            get
            {
                return mDistanceModel;
            }
            set
            {
                mDistanceModel = value;
                ALDistanceModel model;
                switch(value)
                {
                    case AudioDistanceModel.LinearDistance: model = ALDistanceModel.LinearDistance; break;
                    case AudioDistanceModel.LinearDistanceClamped: model = ALDistanceModel.LinearDistanceClamped; break;
                    case AudioDistanceModel.InverseDistance: model = ALDistanceModel.InverseDistance; break;
                    case AudioDistanceModel.InverseDistanceClamped: model = ALDistanceModel.InverseDistanceClamped; break;
                    case AudioDistanceModel.None: model = ALDistanceModel.None; break;
                    default: model = ALDistanceModel.LinearDistance; break;
                }
                AL.DistanceModel(model);
            }
        }

        private AudioContext mAudioContext;
        private bool mAudioDeviceAvailable = true;

        private readonly List<Fader> mMusic;
        private readonly List<AudioSource> mAudioSources;
        private AudioDistanceModel mDistanceModel;

        public SoundManager()
        {

            mMusic = new List<Fader>();
            mAudioSources = new List<AudioSource>();
            try
            {
                mAudioContext = new AudioContext();
            }
            catch (Exception e)
            {
                mAudioDeviceAvailable = false;
                return;
            }
            mAudioContext.MakeCurrent();
            DistanceModel = AudioDistanceModel.LinearDistance;

        }

        public void AddMusic(SoundEffect music)
        {

            if (mMusic.Count > 0)
            {

                // if we want to add new music we have to fade out the current song
                var lastFader = mMusic[mMusic.Count - 1];

                lastFader.FadeOut();

            }

            var instance = music.CreateInstance();
            instance.IsLooped = true;
            instance.Volume = 0.0f;

            var fader = new Fader(instance);

            mMusic.Add(fader);
            instance.Play();

        }

        public void AddSound(SoundEffect sound)
        {
            var instance = sound.CreateInstance();
            instance.Volume = Options.SoundEffectVolume / 100.0f;
            instance.Play();
        }

        public void AddSound(AudioSource audioSource)
        {
            mAudioSources.Add(audioSource);
            audioSource.Play();
        }

        public void RemoveSound(AudioSource audioSource)
        {
            mAudioSources.Remove(audioSource);
            audioSource.Stop();
        }

        public void Update(GameTime gameTime)
        {

            if (!mAudioDeviceAvailable)
            {
                return;
            }

            // we want to find all elements which are already faded out to delete them
            var elements = mMusic.FindAll(ele => ele.mFadeState == FadeState.Stopped);

            foreach (var element in elements)
            {
                mMusic.Remove(element);
            }

            foreach (var fader in mMusic)
            {
                fader.Update(gameTime, Options.MusicVolume / 100.0f);
            }

        }

        public void Update(GameTime gameTime, Scene scene, Camera camera)
        {

            if (!mAudioDeviceAvailable)
            {
                return;
            }

            var location = camera.mThirdPerson ? camera.mLocation - camera.Direction * camera.mThirdPersonDistance : camera.mLocation;
            var listener = new OpenTK.Vector3(location.X, location.Y, location.Z);
            AL.Listener(ALListener3f.Position, ref listener);

            var up = camera.Up;
            var dir = -camera.Direction;
            var field = new[] {dir.X, dir.Y, dir.Z, up.X, up.Y, up.Z};
            AL.Listener(ALListenerfv.Orientation, ref field);

            foreach (var audioSource in mAudioSources)
            {
                audioSource.VolumeMultiplier = Options.SoundEffectVolume / 100.0f;
            }

            foreach (var actorbatch in scene.mActorBatches)
            {

                foreach (var actor in actorbatch.mActors)
                {

                    foreach (var audioSource in actor.mAudioSources)
                    {

                        audioSource.Location = actor.ModelMatrix.Translation;
                        audioSource.VolumeMultiplier = Options.SoundEffectVolume / 100.0f;

                    }

                }

            }

        }

    }

}
