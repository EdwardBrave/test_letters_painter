using System;
using TracingSystem.Model;
using TracingSystem.View;
using Zenject;

namespace TracingSystem
{
    public class GameLevelPresenter : IInitializable, IDisposable
    {
        private readonly GameLevelView _view;
        private readonly FullLevelModel _model;
        private readonly LineTracerView.Factory _lineViewFactory;

        public GameLevelPresenter(FullLevelModel model, GameLevelView view, LineTracerView.Factory lineViewFactory)
        {
            _model = model;
            _view = view;
            _lineViewFactory = lineViewFactory;
        }

        public void Initialize()
        {
            _view.shapeMaskView.UpdateSprite(_model.Shape);
            PopulateLineTracerViews();
        }

        private void PopulateLineTracerViews()
        {
            foreach (var existing in _view.lineTracerViews)
                existing.Clear();
            _view.lineTracerViews.Clear();

            foreach (var lineModel in _model.Lines)
            {
                var view = _lineViewFactory.Create();
                view.Init(lineModel.Points, lineModel.Width, _model.MainColor, lineModel.Progress);
                _view.lineTracerViews.Add(view);
            }
        }

        public void Dispose()
        {
        }
        
        public class Factory : PlaceholderFactory<GameLevelPresenter>
        {
        }
    }
}
