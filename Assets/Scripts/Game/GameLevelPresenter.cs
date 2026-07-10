using System;
using Game.Model;
using Game.View;
using Installers;
using Unity.Mathematics;
using UnityEngine;
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
        private readonly SettingsInstaller.Tracing _tracingSettings;
        
        private Spline _activeSpline;
        public int ActiveLineIndex { get; private set; } = -1;
        public bool IsLineFinished => IsFinished || (0 <= ActiveLineIndex && _model.Lines[ActiveLineIndex].IsFinished);
        public bool IsFinished => ActiveLineIndex >= _model.Lines.Count;
        
        public GameLevelPresenter(FullLevelModel model, GameLevelView view, LineTracerView.Factory lineViewFactory,
            SettingsInstaller.Tracing tracing)
        {
            _tracingSettings = tracing;
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
        
        public void StartHelperSequence(Spline spline, float progress)
        {
            _view.linePathView.StartHelperSequence(spline, progress);
        }
        
        public void EndHelperSequence()
        {
            _view.linePathView.EndHelperSequence();
        }
        
        public void UpdateTracing()
        {
            if (ActiveLineIndex < 0 || IsFinished)
            {
                return;
            }

            if (!TryGetPressedScreenPosition(out Vector2 screenPosition) || 
                !TryGetWorldPointOnLinePlane(screenPosition, out float3 worldPosition))
            {
                return;
            }

            var line = _model.Lines[ActiveLineIndex];
            float3 currentPosition = _activeSpline.EvaluatePosition(line.Progress);
            
            if (Vector3.Distance(currentPosition, worldPosition) > _tracingSettings.maxTracingDistance)
            {
                return;
            }

            SplineUtility.GetNearestPoint(_activeSpline, worldPosition, out float3 nearest, out float progress);
            
            if (_activeSpline.GetLength() * (1 - progress) <= _tracingSettings.completionOffsetDistance)
            {
                progress = 1f;
            }
            
            line.Progress = progress;
            _view.linePathView.UpdatePointer(_activeSpline, line.Progress);
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
            StartHelperSequence(_activeSpline, _model.Lines[ActiveLineIndex].Progress);
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

        private static bool TryGetPressedScreenPosition(out Vector2 screenPosition)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                {
                    screenPosition = touch.position;
                    return true;
                }
            }

            if (Input.GetMouseButton(0))
            {
                screenPosition = Input.mousePosition;
                return true;
            }

            screenPosition = default;
            return false;
        }

        private static bool TryGetWorldPointOnLinePlane(Vector2 screenPosition, out float3 worldPosition)
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                worldPosition = default;
                return false;
            }

            Ray ray = camera.ScreenPointToRay(screenPosition);
            var linePlane = new Plane(Vector3.forward, Vector3.zero);
            if (!linePlane.Raycast(ray, out float distance))
            {
                worldPosition = default;
                return false;
            }

            Vector3 position = ray.GetPoint(distance);
            worldPosition = new float3(position.x, position.y, 0f);
            return true;
        }
        
        public class Factory : PlaceholderFactory<GameLevelPresenter>
        {
        }
    }
}
