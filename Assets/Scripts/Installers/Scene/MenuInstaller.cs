using AppStates;
using Game.Model;
using Services.Scene;
using UI;
using UI.Menu;
using UnityEngine;
using Zenject;

namespace Installers.Scene
{
    public class MenuInstaller : MonoInstaller<GameInstaller>
    {
        [SerializeField] private Transform categoriesContainer;
        
        [SerializeField] private LevelCategoryView levelCategoryViewPrefab;
        [SerializeField] private ButtonView levelButtonViewPrefab;
        
        public override void InstallBindings()
        {
            Container.Bind<GameAudioService>().AsSingle().NonLazy();

            Container
                .BindFactory<LevelCategoryModel, LevelCategoryPresenter, LevelCategoryPresenter.Factory>()
                .FromSubContainerResolve()
                .ByMethod(InstallCategorySubContainer);
            
            Container
                .BindFactory<LightLevelModel, Transform, LevelButtonPresenter, LevelButtonPresenter.Factory>()
                .FromSubContainerResolve()
                .ByMethod(InstallLevelButtonSubContainer);
            
            Container.BindInterfacesAndSelfTo<MenuState>().AsSingle().NonLazy();
        }

        private void InstallCategorySubContainer(DiContainer subContainer, LevelCategoryModel model)
        {
            subContainer.BindInstance(model).AsSingle();
            subContainer.Bind<LevelCategoryView>()
                .FromComponentInNewPrefab(levelCategoryViewPrefab)
                .UnderTransform(categoriesContainer)
                .AsSingle();
            
            subContainer.BindInterfacesAndSelfTo<LevelCategoryPresenter>().AsSingle().NonLazy();
        }
        
        private void InstallLevelButtonSubContainer(DiContainer subContainer, LightLevelModel model, Transform parent)
        {
            subContainer.BindInstance(model).AsSingle();
            subContainer.Bind<ButtonView>()
                .FromComponentInNewPrefab(levelButtonViewPrefab)
                .UnderTransform(parent)
                .AsSingle();
            
            subContainer.BindInterfacesAndSelfTo<LevelButtonPresenter>().AsSingle().NonLazy();
        }
    }
}