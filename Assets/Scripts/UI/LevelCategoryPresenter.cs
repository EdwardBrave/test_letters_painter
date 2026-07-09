using Zenject;

namespace UI
{
    public class LevelCategoryPresenter
    {
        private readonly LevelCategoryModel _model;
        private readonly LevelCategoryView _view;
        private readonly LevelButtonPresenter.Factory _levelButtonFactory;

        public LevelCategoryPresenter(LevelCategoryModel model, LevelCategoryView view, LevelButtonPresenter.Factory levelButtonFactory)
        {
            _model = model;
            _view = view;
            _levelButtonFactory = levelButtonFactory;
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