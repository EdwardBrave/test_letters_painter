using UnityEngine;

namespace Services.Scene
{
    public class GameAudioService
    {
        private AudioSource _audioSource;

        public GameAudioService(AudioSource audioSource)
        {
            _audioSource = audioSource;
        }
        
        public void Play(AudioClip clip)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}