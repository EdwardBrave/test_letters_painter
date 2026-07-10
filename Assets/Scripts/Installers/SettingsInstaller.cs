using System;
using System.Collections.Generic;
using Game.Model;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Data/GameSettings")]
    public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
    {
        [SerializeField] private Tracing tracing = new ();
        [SerializeField] private Audio audio = new ();
        [SerializeField] private Helper helper = new ();
        [SerializeField] private CategoryNames categoryNames = new ();
        
        public override void InstallBindings()
        {
            Container.BindInstance(tracing).AsSingle();
            Container.BindInstance(audio).AsSingle();
            Container.BindInstance(helper).AsSingle();
            Container.BindInstance(categoryNames).AsSingle();
        }
        
        [Serializable]
        public class CategoryNames
        {
            [Serializable]
            public struct Pair
            {
                public string displayName;
                public LevelCategory categoryType;
            }
            
            public List<Pair> pairs;
        }
        
        [Serializable]
        public class Tracing
        {
            public float maxTracingDistance = 1f;
            public float completionOffsetDistance = 0.5f;
            [Range(0f, 1f)]
            public float progressThreshold = 0.5f;
        }
        
        [Serializable]
        public class Helper
        {
            public float audioRepeatDelay = 7f;
            public float helperStartDelay = 14f;
        }
        
        [Serializable]
        public class Audio
        {
            public float volume = 0.8f;
            public List<AudioClip> congratsAudioClips;
        }
    }
}
