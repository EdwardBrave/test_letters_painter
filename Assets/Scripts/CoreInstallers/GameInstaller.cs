using System.IO;
using Newtonsoft.Json;
using Services;
using Tools;
using TracingSystem.Model;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [SerializeField] private Transform tracingSpace;
        
        public override void InstallBindings()
        {
            Container.Bind<GameAudioService>().AsSingle().NonLazy();
            
            // TODO remove after level loading testing testing
            string filePath = Path.Combine(Application.streamingAssetsPath, "Levels", "Shape_A.json");
            string json = File.ReadAllText(filePath);
            LevelModel preset = JsonConvert.DeserializeObject<LevelModel>(json, UnityJsonConverters.Settings);
            Container.BindInstance(preset).AsSingle();
            Container.QueueForInject(preset);
        }   
    }
}