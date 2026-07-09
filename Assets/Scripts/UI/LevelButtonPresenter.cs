using System;
using Core.States;
using TracingSystem.Model;
using UnityEngine;
using Zenject;

namespace UI
{
    public class LevelButtonPresenter : IDisposable
    {
        private readonly LightLevelModel _model;
        private readonly LevelButtonView _view;
        private readonly MainMenuState _mainMenuState;

        public LevelButtonPresenter(LightLevelModel model, LevelButtonView view, MainMenuState mainMenuState)
        {
            _model = model;
            _view = view;
            _mainMenuState = mainMenuState;
        }

        public void Initialize()
        {
            _view.UpdateImage(_model.Shape, _model.MainColor);
            _view.OnClick += OnLevelButtonClick;
        }

        private void OnLevelButtonClick()
        {
            _mainMenuState.TryLoadLevel(_model.Name);
        }

        public void Dispose()
        {
            _view.OnClick -= OnLevelButtonClick;
            _view.UpdateImage(null, default);
        }
        
        public class Factory : PlaceholderFactory<LightLevelModel, Transform, LevelButtonPresenter>
        {
        }
    }
}