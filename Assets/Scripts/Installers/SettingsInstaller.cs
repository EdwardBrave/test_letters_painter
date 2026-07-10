using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Data/GameSettings")]
    public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
    {
        [SerializeField] private Tracing tracing = new ();
        [SerializeField] private Audio audio = new ();
        
        public override void InstallBindings()
        {
            Container.BindInstance(tracing).AsSingle();
            Container.BindInstance(audio).AsSingle();
        }
        
        [Serializable]
        public class Tracing
        {
            public float maxTracingDistance = 1f;
            public float completionOffsetDistance = 0.5f;
        }
        
        [Serializable]
        public class Audio
        {
            public float volume = 0.8f;
            public List<AudioClip> congratsAudioClips;
        }
    }
}