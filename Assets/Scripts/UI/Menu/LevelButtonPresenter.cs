using System;
using AppStates;
using Game.Model;
using UnityEngine;
using Zenject;

namespace UI.Menu
{
    public class LevelButtonPresenter : IDisposable
    {
        private readonly LightLevelModel _model;
        private readonly ButtonView _view;
        private readonly MenuState menuState;

        public LevelButtonPresenter(LightLevelModel model, ButtonView view, MenuState menuState)
        {
            _model = model;
            _view = view;
            this.menuState = menuState;
        }

        public void Initialize()
        {
            _view.UpdateImage(_model.Shape, _model.MainColor);
            _view.OnClick += OnLevelButtonClick;
        }

        private void OnLevelButtonClick()
        {
            menuState.TryLoadLevel(_model.Name);
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