using System;
using Game.Model;
using Game.View;
using Unity.Mathematics;
using UnityEngine.Splines;
using Zenject;

namespace Game
{
    public class GameLevelPresenter : IInitializable, IDisposable
    {
        private const float ToLineLayer = 0.01f;
        
        private readonly GameLevelView _view;
        private readonly FullLevelModel _model;
        private readonly LineTracerView.Factory _lineViewFactory;
        
        private Spline _activeSpline;
        public int ActiveLineIndex { get; private set; } = -1;
        public bool IsLineFinished => IsFinished || (0 < ActiveLineIndex && _model.Lines[ActiveLineIndex].IsFinished);
        public bool IsFinished => ActiveLineIndex >= _model.Lines.Count;
        
        public GameLevelPresenter(FullLevelModel model, GameLevelView view, LineTracerView.Factory lineViewFactory)
        {
            _activeSpline = new Spline();
            _model = model;
            _view = view;
            _lineViewFactory = lineViewFactory;
        }

        public void Initialize()
        {
            _view.shapeMaskView.UpdateSprite(_model.Shape);
            PopulateLineTracerViews();
        }

        public bool PlayNextLine()
        {
            ActiveLineIndex++;
            if (IsFinished)
            {
                _view.linePathView.Clear();
                return false;
            }
            
            StartTracingForLine(ActiveLineIndex);
            return true;
        }

        public void Dispose()
        {
            int count = _model.Lines.Count;
            for (int i = 0; i < count; i++)
            {
                _model.Lines[i].OnProgressChanged -= _view.lineTracerViews[i].ApplyProgressChange;
            }
            
            _view.Dispose();
        }
        
        private void PopulateLineTracerViews()
        {
            foreach (var existing in _view.lineTracerViews)
            {
                existing.Clear();
            }
            _view.lineTracerViews.Clear();

            int count = _model.Lines.Count;
            for (int i = 0; i < count; i++)
            {
                var lineModel = _model.Lines[i];
                var view = _lineViewFactory.Create();
                view.Init(i * ToLineLayer, lineModel.Points, lineModel.Width, _model.MainColor, lineModel.Progress);
                lineModel.OnProgressChanged += view.ApplyProgressChange;
                _view.lineTracerViews.Add(view);
            }
        }
        
        private void StartTracingForLine(int lineIndex)
        {
            _activeSpline.Clear();
            
            ActiveLineIndex = lineIndex;
            var line = _model.Lines[ActiveLineIndex];
            
            foreach (var linePoint in line.Points)
            {
                _activeSpline.Add(new float3(linePoint.x, linePoint.y, 0));
            }
            
            _view.linePathView.DrawPath(_activeSpline, ActiveLineIndex * ToLineLayer - ToLineLayer/2);
        }
        
        public class Factory : PlaceholderFactory<GameLevelPresenter>
        {
        }
    }
}
