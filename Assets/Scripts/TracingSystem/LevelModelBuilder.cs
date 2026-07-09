#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Services;
using TracingSystem.Dto;
using TracingSystem.View;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TracingSystem
{
    public class LevelModelBuilder : MonoBehaviour
    {
        public LevelSerializationService levelSerializationService = new ();
        
        [Header("Configs and references")]
        public LineTracerView lineTracerViewPrefab;

        public LevelView levelView;

        [Header("Current State Data")]
        public AssetReferenceSprite shapeAssetRef;
        public AssetReferenceT<AudioClip> goalAudioRef;

        public Color mainColor;

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (!levelView)
                return;

            if (shapeAssetRef != null && !string.IsNullOrEmpty(shapeAssetRef.AssetGUID) && levelView.shapeMaskView != null)
            {
                string path = AssetDatabase.GUIDToAssetPath(shapeAssetRef.AssetGUID);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                levelView.shapeMaskView.UpdateSprite(sprite);
            }

            RefreshLineTracerViews();
        }
        
        public void RefreshLineTracerViews()
        {
            levelView.lineTracerViews = new List<LineTracerView>(
                levelView.linesContainer.GetComponentsInChildren<LineTracerView>());
            foreach (var lineTracerView in levelView.lineTracerViews)
            {
                lineTracerView.ApplyColor(mainColor);
            }
        }

        public void InstantiateLineTracerView(LevelDto.LineDto? lineDto = null)
        {
            var newLine = PrefabUtility.InstantiatePrefab(lineTracerViewPrefab, levelView.linesContainer);
            if (newLine is LineTracerView view && lineDto.HasValue)
            {
                view.Init(lineDto.Value.points, lineDto.Value.width, mainColor);
            }
        }
        
        public LevelDto BuildLevelDto()
        {
            OnValidate();

            var lines = new List<LevelDto.LineDto>();
            foreach (var lineTracerView in levelView.lineTracerViews)
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
                shapeAssetGUID = shapeAssetRef?.AssetGUID,
                goalAudioAssetGUID = goalAudioRef?.AssetGUID,
                mainColor = mainColor,
                lines = lines.ToArray(),
            };
        }
        
        public void ApplyLevelDto(LevelDto dto)
        {
            shapeAssetRef = new AssetReferenceSprite(dto.shapeAssetGUID);
            goalAudioRef = new AssetReferenceT<AudioClip>(dto.goalAudioAssetGUID);
            mainColor = dto.mainColor;

            var oldComponents = levelView.linesContainer.GetComponentsInChildren<LineTracerView>();
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
            levelSerializationService.WriteToFile(BuildLevelDto(), levelName);
        }

        public void ImportFromJson(string filePath)
        {
            LevelDto dto = levelSerializationService.ReadFromFile(filePath, true);
            ApplyLevelDto(dto);
        }
#endif
    }
}
