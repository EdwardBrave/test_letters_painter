using Game.Model;
using Installers;
using Zenject;

namespace UI.Menu
{
    public class LevelCategoryPresenter
    {
        private readonly LevelCategoryModel _model;
        private readonly LevelCategoryView _view;
        private readonly LevelButtonPresenter.Factory _levelButtonFactory;

        public LevelCategoryPresenter(LevelCategoryModel model, LevelCategoryView view, 
            LevelButtonPresenter.Factory levelButtonFactory, SettingsInstaller.CategoryNames settings)
        {
            _model = model;
            _view = view;
            _levelButtonFactory = levelButtonFactory;

            var result = settings.pairs.Find(pair => pair.categoryType == _model.Category);
            
            if (result.categoryType != LevelCategory.None)
            {
                _view.Text.text = result.displayName;
            }
        }
        
        public void PopulateLevels()
        {
            foreach (var level in _model.Levels)
            {
                var levelPresenter = _levelButtonFactory.Create(level, _view.LevelsContainer);
                levelPresenter.Initialize();
            }
        }

        public class Factory : PlaceholderFactory<LevelCategoryModel, LevelCategoryPresenter>
        {
        }
    }
}