using UnityEngine;
using Zenject;

namespace CoreInstallers
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Data/GameSettings")]
    public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
    {
        public override void InstallBindings()
        {
            
        }
    }
}