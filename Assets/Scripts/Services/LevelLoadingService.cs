using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Dto;
using Game.Model;
using UnityEngine;

namespace Services
{

    public class LevelLoadingService
    {
        private readonly AssetLoadingService _assetLoadingService;
        private readonly LevelSerializationService _levelSerializationService;

        public LevelLoadingService(LevelSerializationService levelSerializationService, AssetLoadingService assetLoadingService)
        {
            _levelSerializationService = levelSerializationService;
            _assetLoadingService = assetLoadingService;
        }
        
        public async UniTask<List<LightLevelModel>> LoadAllLightLevelsAsync(CancellationToken token = default)
        {
            var lightLevels = new List<LightLevelModel>();
            
            var levelNames = _levelSerializationService.GetLevelNames();
            if (levelNames.Length == 0)
            {
                Debug.LogWarning("No levels found.");
                return lightLevels;
            }
            
            foreach (var levelName in levelNames)
            {
                LevelDto dto = _levelSerializationService.ReadFromFile(levelName);

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

            LevelDto dto = _levelSerializationService.ReadFromFile(levelName);

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
