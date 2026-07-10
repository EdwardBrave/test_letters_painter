using Services;
using Zenject;

namespace Installers
{
    public class ServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Bound with all interfaces so IInitializable.Initialize()/IDisposable.Dispose()
            // run — the service owns the Addressables lifecycle across scenes.
            Container.BindInterfacesAndSelfTo<AssetLoadingService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LevelSerializationService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LevelLoadingService>().AsSingle().NonLazy();
        }
    }
}
