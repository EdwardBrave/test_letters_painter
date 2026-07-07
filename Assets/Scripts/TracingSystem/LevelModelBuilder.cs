#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using TracingSystem.Model;
using TracingSystem.View;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TracingSystem
{
    public class LevelModelBuilder : MonoBehaviour
    {
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
            {
                return;
            }
            
            if (shapeAssetRef != null && levelView.shapeMaskView != null)
            {
                Sprite sprite = AssetDatabase.LoadAssetByGUID<Sprite>(new GUID(shapeAssetRef.AssetGUID));
                levelView.shapeMaskView.UpdateSprite(sprite);
            }
            
            levelView.lineTracerViews.Clear();
            levelView.lineTracerViews = new List<LineTracerView>(
                levelView.linesContainer.GetComponentsInChildren<LineTracerView>());
            foreach (var lineTracerView in levelView.lineTracerViews)
            {
                lineTracerView.ApplyColor(mainColor);
            }
        }

        public void ApplyLevelModel(LevelModel model)
        {
            shapeAssetRef = new AssetReferenceSprite(model.shapeAssetGUID);
            goalAudioRef = new AssetReferenceT<AudioClip>(model.goalAudioAssetGUID);
            mainColor = model.mainColor;
                
            var oldComponents = GetComponentsInChildren<LineTracerView>();
            for (int i = oldComponents.Length - 1; i >= 0; --i)
            {
                DestroyImmediate(oldComponents[i].gameObject);
            }
            
            foreach (var lineModel in model.lines)
            {
                InstantiateLineTracerView(lineModel, model.mainColor);
            }
            
            OnValidate();
        }

        public void InstantiateLineTracerView(LineModel lineModel = null, Color defaultColor = default)
        {
            
            var newLine = PrefabUtility.InstantiatePrefab(lineTracerViewPrefab, levelView.linesContainer);
            if (newLine is LineTracerView view && lineModel != null)
            {
                view.Init(lineModel.points, lineModel.width, defaultColor, lineModel.Progress);
            }
        }

        public LevelModel BuildLevelModel()
        {
            OnValidate();

            var lines = new List<LineModel>();
            foreach (var lineTracerView in levelView.lineTracerViews)
            {
                var lineRenderer = lineTracerView.LineRenderer;
                if (lineRenderer == null || lineRenderer.positionCount == 0)
                {
                    continue;
                }

                var model = new LineModel
                {
                    width = lineRenderer.startWidth,
                    points = new List<Vector2>()
                };

                for (int i = 0; i < lineRenderer.positionCount; i++)
                {
                    var pos = lineRenderer.GetPosition(i);
                    model.points.Add(new Vector2(pos.x, pos.y));
                }
                
                lines.Add(model);
            }
            
            return new LevelModel
            {
                shapeAssetGUID = shapeAssetRef?.AssetGUID,
                goalAudioAssetGUID = goalAudioRef?.AssetGUID,
                mainColor = mainColor,
                lines = lines,
            };
        }
#endif
    }
}
