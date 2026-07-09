using Services.Scene;
using UnityEngine;
using Zenject;

namespace CoreInstallers.Scenes
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [SerializeField] private Transform tracingSpace;

        public override void InstallBindings()
        {
            Container.Bind<GameAudioService>().AsSingle().NonLazy();
        }
    }
}
