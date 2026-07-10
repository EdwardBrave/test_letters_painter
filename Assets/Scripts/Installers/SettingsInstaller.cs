using System;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Data/GameSettings")]
    public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
    {
        [SerializeField] private Tracing tracing = new ();
        
        public override void InstallBindings()
        {
            Container.BindInstance(tracing).AsSingle();
        }
        
        [Serializable]
        public class Tracing
        {
            public float maxTracingDistance = 1f;
            public float completionOffsetDistance = 0.5f;
        }
    }
}