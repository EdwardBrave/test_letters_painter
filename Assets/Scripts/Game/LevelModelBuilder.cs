#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Services;
using TracingSystem.Dto;
using TracingSystem.Model;
using TracingSystem.View;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace TracingSystem
{
    public class LevelModelBuilder : MonoBehaviour
    {
        public LevelSerializationService levelSerializationService = new ();
        
        [Header("Configs and references")]
        public LineTracerView lineTracerViewPrefab;

        [FormerlySerializedAs("levelView")] public GameLevelView gameLevelView;

        [Header("Current State Data")]
        public LevelCategory levelCategory;
        public AssetReferenceSprite shapeAssetRef;
        public AssetReferenceT<AudioClip> goalAudioRef;

        public Color mainColor;

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (!gameLevelView)
                return;

            if (shapeAssetRef != null && !string.IsNullOrEmpty(shapeAssetRef.AssetGUID) && gameLevelView.shapeMaskView != null)
            {
                string path = AssetDatabase.GUIDToAssetPath(shapeAssetRef.AssetGUID);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                gameLevelView.shapeMaskView.UpdateSprite(sprite);
            }

            RefreshLineTracerViews();
        }
        
        public void RefreshLineTracerViews()
        {
            gameLevelView.lineTracerViews = new List<LineTracerView>(
                gameLevelView.linesContainer.GetComponentsInChildren<LineTracerView>());
            foreach (var lineTracerView in gameLevelView.lineTracerViews)
            {
                lineTracerView.ApplyColor(mainColor);
            }
        }

        public void InstantiateLineTracerView(LevelDto.LineDto? lineDto = null)
        {
            var newLine = PrefabUtility.InstantiatePrefab(lineTracerViewPrefab, gameLevelView.linesContainer);
            if (newLine is LineTracerView view && lineDto.HasValue)
            {
                view.Init(lineDto.Value.points, lineDto.Value.width, mainColor);
            }
        }
        
        public LevelDto BuildLevelDto()
        {
            OnValidate();

            var lines = new List<LevelDto.LineDto>();
            foreach (var lineTracerView in gameLevelView.lineTracerViews)
            {
                var lineRenderer = lineTracerView.LineRenderer;
                if (lineRenderer == null || lineRenderer.positionCount == 0)
                    continue;

                var points = new Vector2[lineRenderer.positionCount];
                for (int i = 0; i < lineRenderer.positionCount; i++)
                {
                    var pos = lineRenderer.GetPosition(i);
                    points[i] = new Vector2(pos.x, pos.y);
                }

                lines.Add(new LevelDto.LineDto
                {
                    width = lineRenderer.startWidth,
                    points = points,
                });
            }

            return new LevelDto
            {
                category = levelCategory,
                shapeAssetGUID = shapeAssetRef?.AssetGUID,
                goalAudioAssetGUID = goalAudioRef?.AssetGUID,
                mainColor = mainColor,
                lines = lines.ToArray(),
            };
        }
        
        public void ApplyLevelDto(LevelDto dto)
        {
            levelCategory = dto.category;
            shapeAssetRef = new AssetReferenceSprite(dto.shapeAssetGUID);
            goalAudioRef = new AssetReferenceT<AudioClip>(dto.goalAudioAssetGUID);
            mainColor = dto.mainColor;

            var oldComponents = gameLevelView.linesContainer.GetComponentsInChildren<LineTracerView>();
            for (int i = oldComponents.Length - 1; i >= 0; --i)
            {
                DestroyImmediate(oldComponents[i].gameObject);
            }

            if (dto.lines != null)
            {
                foreach (var line in dto.lines)
                    InstantiateLineTracerView(line);
            }

            OnValidate();
        }

        public void ExportToJson(string levelName)
        {
            var dto = BuildLevelDto();
            dto.name = levelName;
            
            levelSerializationService.WriteToFile(dto, levelName);
        }

        public void ImportFromJson(string filePath)
        {
            LevelDto dto = levelSerializationService.ReadFromFile(filePath, true);
            ApplyLevelDto(dto);
        }
#endif
    }
}
