using System;
using Core.States;
using Zenject;

namespace UI
{
    public class HomeButtonPresenter : IInitializable, IDisposable
    {
        private readonly ButtonView _view;
        private readonly GameState _gameState;

        public HomeButtonPresenter(ButtonView view, GameState gameState)
        {
            _view = view;
            _gameState = gameState;
        }

        public void Initialize()
        {
            _view.OnClick += OnLevelButtonClick;
        }

        private void OnLevelButtonClick()
        {
            _gameState.GoToMainMenu();
        }

        public void Dispose()
        {
            _view.OnClick -= OnLevelButtonClick;
        }
    }
}