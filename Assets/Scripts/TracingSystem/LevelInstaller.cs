using TracingSystem.Model;
using TracingSystem.View;
using UnityEngine;
using Zenject;

namespace TracingSystem
{
    public class LevelInstaller : MonoInstaller<LevelInstaller>
    {
        [SerializeField] private LevelView levelView;

        public override void InstallBindings()
        {
            // Service Layer
            //Container.Bind<IAssetProvider>().To<AddressableAssetProvider>().AsSingle();

            // Pass direct or mocked configuration data into the model for this level
            Container.Bind<LevelModel>().AsSingle().IfNotBound();
            Container.BindInstance(levelView).AsSingle();
            Container.BindInterfacesAndSelfTo<LevelPresenter>().AsSingle().NonLazy();
        }
    }
}