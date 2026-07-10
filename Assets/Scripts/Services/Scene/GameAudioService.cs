using System;
using Installers;
using UnityEngine;
using Zenject;

namespace Services.Scene
{
    public class GameAudioService : IInitializable, IDisposable
    {
        private AudioSource _audioSource;
        private SettingsInstaller.Audio _audioSettings;

        public GameAudioService(AudioSource audioSource, SettingsInstaller.Audio audio)
        {
            _audioSource = audioSource;
            _audioSettings = audio;
        }

        public void Initialize()
        {
            _audioSource.enabled = true;
        }

        public void Play(AudioClip clip, bool stopOthers = true)
        {
            if (clip == null)
            {
                return;
            }
            
            if (stopOthers)
            {
                _audioSource.Stop();
            }
            
            _audioSource.PlayOneShot(clip, _audioSettings.volume);
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public void Dispose()
        {
            _audioSource.Stop();
            _audioSource.enabled = false;
        }
    }
}
