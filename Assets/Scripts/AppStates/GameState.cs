using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game;
using Game.Model;
using Installers;
using ModestTree;
using Services;
using Services.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace AppStates
{
    public class GameState : IAppState
    {
        private readonly GameLevelPresenter.Factory _gameLevelPresenterFactory;
        private readonly LevelSerializationService _levelSerializationService;
        private readonly LevelLoadingService _levelLoadingService;
        private readonly GameAudioService _gameAudioService;
        private readonly SettingsInstaller.Audio _audioSettings;
        private readonly DiContainer _container;
        private readonly DiContainer _projectContainer;
        private CancellationTokenSource _cts;
        
        private GameLevelPresenter _levelPresenter;

        public GameState(DiContainer container, GameLevelPresenter.Factory gameLevelPresenterFactory,
            LevelSerializationService levelSerializationService, LevelLoadingService levelLoadingService,
            GameAudioService gameAudioService, SettingsInstaller.Audio audioSettings)
        {
            _cts = new CancellationTokenSource();
            _container = container;
            _projectContainer = ProjectContext.Instance.Container;
            _gameLevelPresenterFactory = gameLevelPresenterFactory;
            _levelSerializationService = levelSerializationService;
            _levelLoadingService = levelLoadingService;
            _gameAudioService = gameAudioService;
            _audioSettings = audioSettings;
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
                PlayLevelAsync(_cts.Token).Forget();
            }
            else
            {
                InitializeAndPlayAsync(_cts.Token).Forget();
            }
        }
        
        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();

            if (_projectContainer.HasBinding<FullLevelModel>())
            {
                _projectContainer.Unbind<FullLevelModel>();
            }
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("Menu");
        }
        
        private async UniTaskVoid InitializeAndPlayAsync(CancellationToken token)
        {
            var levelNames = _levelSerializationService.GetLevelNames();
            if (levelNames.Count == 0)
            {
                return;
            }

            var levelsCategoryPair = levelNames.First();
            
            await LoadFullLevelModel(levelsCategoryPair.Key, levelsCategoryPair.Value[0], token);
            await PlayLevelAsync(token);
        }
        
        private async UniTask PlayLevelAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _levelPresenter = _gameLevelPresenterFactory.Create();
                _levelPresenter.Initialize();
                
                var model = _projectContainer.Resolve<FullLevelModel>();
                _gameAudioService.Play(model.GoalAudio);

                while (_levelPresenter.PlayNextLine())
                {
                    await UniTask.WaitUntil(() =>
                    {
                        _levelPresenter.UpdateTracing();
                        return _levelPresenter.IsLineFinished;
                    }, PlayerLoopTiming.Update, token,true);
                    token.ThrowIfCancellationRequested();
                }

                AudioClip congratsClip = PlayRandomCongratsAudio();
                if (congratsClip != null)
                {
                    await UniTask.Delay(System.TimeSpan.FromSeconds(congratsClip.length), DelayType.DeltaTime, PlayerLoopTiming.Update, token);
                }

                await UniTask.Delay(100, DelayType.DeltaTime, PlayerLoopTiming.Update, token);
                
                var levelNames = _levelSerializationService.GetLevelNames(model.Category);
                int nextIndex = (levelNames.IndexOf(model.Name) + 1) % levelNames.Length;
                
                _levelPresenter.Dispose();
                _container.Unbind<GameLevelPresenter>();
                await LoadFullLevelModel(model.Category, levelNames[nextIndex], token);
            }
        }
        
        private async UniTask LoadFullLevelModel(LevelCategory category, string levelName, CancellationToken token)
        {
            if (_projectContainer.HasBinding<FullLevelModel>())
            {
                _projectContainer.Unbind<FullLevelModel>();
            }
            
            var fullLevelModel = await _levelLoadingService.LoadFullLevelAsync(category, levelName, token);
            token.ThrowIfCancellationRequested();
            
            _projectContainer.BindInterfacesAndSelfTo<FullLevelModel>().FromInstance(fullLevelModel);
        }
        
        private AudioClip PlayRandomCongratsAudio()
        {
            if (_audioSettings.congratsAudioClips == null || _audioSettings.congratsAudioClips.Count == 0)
            {
                return null;
            }

            int startIndex = Random.Range(0, _audioSettings.congratsAudioClips.Count);
            for (int i = 0; i < _audioSettings.congratsAudioClips.Count; i++)
            {
                int index = (startIndex + i) % _audioSettings.congratsAudioClips.Count;
                AudioClip clip = _audioSettings.congratsAudioClips[index];
                if (clip == null)
                {
                    continue;
                }
                
                _gameAudioService.Play(clip);
                return clip;
            }

            return null;
        }

    }
}
