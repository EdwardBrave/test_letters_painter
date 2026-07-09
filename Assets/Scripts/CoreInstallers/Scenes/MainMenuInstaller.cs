using Services.Scene;
using Zenject;

namespace CoreInstallers.Scenes
{
    public class MainMenuInstaller : MonoInstaller<GameInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<GameAudioService>().AsSingle().NonLazy();
        }
    }
}