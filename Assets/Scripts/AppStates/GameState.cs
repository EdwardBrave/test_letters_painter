using Cysharp.Threading.Tasks;
using Game;
using Game.Model;
using Services;
using UnityEngine.SceneManagement;
using Zenject;

namespace AppStates
{
    public class GameState : IAppState
    {
        private readonly GameLevelPresenter.Factory _gameLevelPresenterFactory;
        private readonly LevelSerializationService _levelSerializationService;
        private readonly LevelLoadingService _levelLoadingService;
        private readonly DiContainer _container;
        private readonly DiContainer _projectContainer;
        
        private GameLevelPresenter _levelPresenter;

        public GameState(DiContainer container, GameLevelPresenter.Factory gameLevelPresenterFactory,
            LevelSerializationService levelSerializationService, LevelLoadingService levelLoadingService)
        {
            _container = container;
            _projectContainer = ProjectContext.Instance.Container;
            _gameLevelPresenterFactory = gameLevelPresenterFactory;
            _levelSerializationService = levelSerializationService;
            _levelLoadingService = levelLoadingService;
        }
        
        public void Initialize()
        {
            // Unbinding dummy model
            if (_container.HasBinding<FullLevelModel>())
            {
                _container.Unbind<FullLevelModel>();
            }
            
            if (_projectContainer.HasBinding<FullLevelModel>())
            {
                PlayLevelAsync().Forget();
            }
            else
            {
                InitializeAsync().Forget();
            }
        }
        
        public void Dispose()
        {
            if (_projectContainer.HasBinding<FullLevelModel>())
            {
                _projectContainer.Unbind<FullLevelModel>();
            }
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("Menu");
        }
        
        private async UniTaskVoid InitializeAsync()
        {
            var levelNames = _levelSerializationService.GetLevelNames();
            if (levelNames.Length == 0)
            {
                return;
            }
            
            var fullLevelModel = await _levelLoadingService.LoadFullLevelAsync(levelNames[0]);
            _projectContainer.BindInstance(fullLevelModel);

            await PlayLevelAsync();
        }

        private async UniTask PlayLevelAsync()
        {
            _levelPresenter = _gameLevelPresenterFactory.Create();
            _levelPresenter.Initialize();

            while (_levelPresenter.PlayNextLine())
            {
                await UniTask.WaitForSeconds(2f);
            }
        }

    }
}