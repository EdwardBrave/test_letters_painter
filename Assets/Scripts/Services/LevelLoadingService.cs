using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TracingSystem.Dto;
using TracingSystem.Model;
using UnityEngine;

namespace Services
{

    public class LevelLoadingService
    {
        private readonly AssetLoadingService _assetLoadingService;
        private readonly LevelSerializationService levelLevelSerializationServiceService;

        public LevelLoadingService(LevelSerializationService levelLevelSerializationServiceService, AssetLoadingService assetLoadingService)
        {
            this.levelLevelSerializationServiceService = levelLevelSerializationServiceService;
            _assetLoadingService = assetLoadingService;
        }
        
        public async UniTask<List<LightLevelModel>> LoadAllLightLevelsAsync(CancellationToken token = default)
        {
            var lightLevels = new List<LightLevelModel>();
            
            var levelNames = levelLevelSerializationServiceService.GetLevelNames();
            if (levelNames.Length == 0)
            {
                Debug.LogWarning("No levels found.");
                return lightLevels;
            }
            
            foreach (var levelName in levelNames)
            {
                LevelDto dto = levelLevelSerializationServiceService.ReadFromFile(levelName);

                using var contextLoading = _assetLoadingService.StartAsyncContextLoading(token);
                
                var model = new LightLevelModel(
                    string.IsNullOrEmpty(dto.name) ? levelName : dto.name,
                    dto.category,
                    dto.mainColor,
                    await contextLoading.LoadAssetAsync<Sprite>(dto.shapeAssetGUID));

                contextLoading.Bind(model);
                lightLevels.Add(model);
            }
            
            return lightLevels;
        }
        
        public async UniTask<FullLevelModel> LoadFullLevelAsync(string levelName, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(levelName))
            {
                throw new ArgumentException("Level name is required.", nameof(levelName));
            }

            LevelDto dto = levelLevelSerializationServiceService.ReadFromFile(levelName);

            using var contextLoading = _assetLoadingService.StartAsyncContextLoading(token);

            var model = new FullLevelModel(
                string.IsNullOrEmpty(dto.name) ? levelName : dto.name,
                dto.category,
                dto.mainColor,
                await contextLoading.LoadAssetAsync<Sprite>(dto.shapeAssetGUID),
                await contextLoading.LoadAssetAsync<AudioClip>(dto.goalAudioAssetGUID),
                BuildLines(dto));

            contextLoading.Bind(model);
            return model;
        }

        private static IReadOnlyList<LineModel> BuildLines(LevelDto dto)
        {
            var lines = new List<LineModel>(dto.lines.Length);
            foreach (var lineDto in dto.lines)
            {
                lines.Add(new LineModel(
                    lineDto.width, 
                    lineDto.points));
            }
            return lines;
        }
    }
}
