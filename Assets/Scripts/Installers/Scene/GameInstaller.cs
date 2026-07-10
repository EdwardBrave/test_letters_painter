using AppStates;
using Services.Scene;
using UI;
using UI.Game;
using UnityEngine;
using Zenject;

namespace Installers.Scene
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [SerializeField] private Transform tracingSpace;
        [SerializeField] private ButtonView homeButton;

        public override void InstallBindings()
        {
            Container.Bind<GameAudioService>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<GameState>().AsSingle().NonLazy();
            
            Container.BindInstance(homeButton).WhenInjectedInto<HomeButtonPresenter>();
            Container.BindInterfacesAndSelfTo<HomeButtonPresenter>().AsSingle().NonLazy();
        }
    }
}
