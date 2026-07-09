using UnityEngine.SceneManagement;

namespace Core.States
{
    public class GameState : IAppState
    {
        
        public void Initialize()
        {
            // todo implement logic
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}