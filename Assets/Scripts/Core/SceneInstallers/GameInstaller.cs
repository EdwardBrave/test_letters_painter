using Core.States;
using Services.Scene;
using UnityEngine;
using Zenject;

namespace Core.SceneInstallers
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [SerializeField] private Transform tracingSpace;

        public override void InstallBindings()
        {
            Container.Bind<GameAudioService>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<GameState>().AsSingle().NonLazy();
        }
    }
}
