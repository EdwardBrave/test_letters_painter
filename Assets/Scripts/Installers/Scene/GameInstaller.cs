using AppStates;
using Services.Scene;
using TracingSystem;
using TracingSystem.View;
using UI;
using UI.Game;
using UnityEngine;
using Zenject;

namespace Installers.Scene
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [SerializeField] private GameLevelView gameLevelViewPrefab;
        [SerializeField] private LineTracerView lineViewPrefab;
        
        [SerializeField] private Transform levelContainer;
        
        [SerializeField] private ButtonView homeButton;
        

        public override void InstallBindings()
        {
            Container.Bind<GameAudioService>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<GameState>().AsSingle().NonLazy();
            
            Container.BindInstance(homeButton).WhenInjectedInto<HomeButtonPresenter>();
            Container.BindInterfacesAndSelfTo<HomeButtonPresenter>().AsSingle().NonLazy();

            Container.BindFactory<GameLevelPresenter, GameLevelPresenter.Factory>()
                .FromSubContainerResolve()
                .ByMethod(InstallGameLevelSubContainer);
        }

        private void InstallGameLevelSubContainer(DiContainer subContainer)
        {
            subContainer.Bind<GameLevelView>()
                .FromComponentInNewPrefab(gameLevelViewPrefab)
                .UnderTransform(levelContainer)
                .AsSingle();
            
            subContainer.BindFactory<LineTracerView, LineTracerView.Factory>()
                .FromComponentInNewPrefab(lineViewPrefab)
                .UnderTransform(context => context.Container.Resolve<GameLevelView>().transform)
                .AsSingle();
            
            subContainer.BindInterfacesAndSelfTo<GameLevelPresenter>().AsSingle().NonLazy();
        }
    }
}
