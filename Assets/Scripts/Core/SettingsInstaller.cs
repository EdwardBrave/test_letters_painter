using UnityEngine;
using Zenject;

namespace Core
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Data/GameSettings")]
    public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
    {
        public override void InstallBindings()
        {
            
        }
    }
}