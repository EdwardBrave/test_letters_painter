using TracingSystem.Model;
using TracingSystem.View;
using UnityEngine;
using Zenject;

namespace TracingSystem
{
    public class GameLevelInstaller : MonoInstaller<GameLevelInstaller>
    {
        [SerializeField] private LevelView levelView;
        [SerializeField] private LineTracerView lineViewPrefab;
        [SerializeField] private Transform lineViewContainer;

        public override void InstallBindings()
        {
            // Service Layer
            //Container.Bind<IAssetProvider>().To<AddressableAssetProvider>().AsSingle();

            // Pass direct or mocked configuration data into the model for this level
            Container.Bind<FullLevelModel>().AsSingle().IfNotBound();
            Container.BindFactory<LineTracerView, LineTracerView.Factory>()
                .FromComponentInNewPrefab(lineViewPrefab)
                .UnderTransform(lineViewContainer)
                .AsSingle();
            Container.BindInstance(levelView).AsSingle();
            Container.BindInterfacesAndSelfTo<GameLevelPresenter>().AsSingle().NonLazy();
        }
    }
}
