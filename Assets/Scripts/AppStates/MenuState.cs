using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Model;
using Services;
using UI.Menu;
using UnityEngine.SceneManagement;
using Zenject;

namespace AppStates
{
    public class MenuState : IAppState
    {
        private readonly LevelLoadingService _levelLoadingService;
        private readonly LevelCategoryPresenter.Factory _categoryPresenterFactory;
        private List<LightLevelModel> _lightLevelModels;
        
        private bool _isLevelLoading = false;

        public MenuState(LevelLoadingService levelLoadingService, LevelCategoryPresenter.Factory categoryPresenterFactory)
        {
            _levelLoadingService = levelLoadingService;
            _categoryPresenterFactory = categoryPresenterFactory;
        }
        
        public void Initialize()
        {
            InitializeAsync().Forget();
        }
        
        private async UniTaskVoid InitializeAsync()
        {
            _lightLevelModels = await _levelLoadingService.LoadAllLightLevelsAsync();
            
            var categories = new Dictionary<LevelCategory, LevelCategoryModel>();
            
            foreach (var levelModel in _lightLevelModels)
            {
                if (!categories.ContainsKey(levelModel.Category))
                {
                    categories.Add(levelModel.Category, new LevelCategoryModel(levelModel.Category));
                }
                
                categories[levelModel.Category].Levels.Add(levelModel);
            }
            
            var sortedCategories = categories.Values
                .OrderBy(x => x.Category)
                .ToList();
            
            foreach (var category in sortedCategories)
            {
                var categoryPresenter = _categoryPresenterFactory.Create(category);
                categoryPresenter.PopulateLevels();
            }
        }

        public void Dispose()
        {
            foreach(var model in _lightLevelModels)
            {
                model.Dispose();
            }

            _lightLevelModels.Clear();
        }

        public void TryLoadLevel(LevelCategory category, string modelName)
        {
            if (_isLevelLoading)
            {
                return;
            }

            _isLevelLoading = true;
            TryLoadLevelAsync(category, modelName).Forget();
        }
        
        private async UniTaskVoid TryLoadLevelAsync(LevelCategory category, string modelName)
        {
            try
            {
                var level = await _levelLoadingService.LoadFullLevelAsync(category, modelName);
                var projectContainer = ProjectContext.Instance.Container;
                if (projectContainer.HasBinding<FullLevelModel>())
                {
                    projectContainer.Unbind<FullLevelModel>();
                }
               
                projectContainer.BindInterfacesAndSelfTo<FullLevelModel>().FromInstance(level);
                
                SceneManager.LoadScene("Game");
            }
            finally
            {
                _isLevelLoading = false;
            }
        }
    }
}