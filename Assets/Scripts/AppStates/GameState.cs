using System;
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
                CreateLevel();
            }
            else
            {
                InitializeAsync().Forget();
            }
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

            CreateLevel();
        }

        private void CreateLevel()
        {
            var levelPresenter = _gameLevelPresenterFactory.Create();
            levelPresenter.Initialize();
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
    }
}