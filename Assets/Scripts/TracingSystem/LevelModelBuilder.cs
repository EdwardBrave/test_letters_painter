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
    
        public ShapeMaskView shapeMaskView;
        
        [Header("Current State Data")]
        public AssetReferenceSprite shapeAssetRef;
        public AssetReferenceT<AudioClip> goalAudioRef;
        
        public Color mainColor;
        public List<LineTracerView> lineTracerViews;

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (shapeAssetRef != null && shapeMaskView != null)
            {
                Sprite sprite = AssetDatabase.LoadAssetByGUID<Sprite>(new GUID(shapeAssetRef.AssetGUID));
                shapeMaskView.Init(sprite);
            }
            
            lineTracerViews.Clear();
            lineTracerViews = new List<LineTracerView>(GetComponentsInChildren<LineTracerView>());
            foreach (var lineTracerView in lineTracerViews)
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
            
            var newLine = PrefabUtility.InstantiatePrefab(lineTracerViewPrefab, transform);
            if (newLine is LineTracerView view && lineModel != null)
            {
                view.Init(lineModel.points, lineModel.width, defaultColor, lineModel.Progress);
            }
        }

        public LevelModel BuildLevelModel()
        {
            OnValidate();

            var lines = new List<LineModel>();
            foreach (var lineTracerView in lineTracerViews)
            {
                lineTracerView.CollectState(out List<Vector2> points, out float width);
                lines.Add(new LineModel
                {
                    points = points,
                    width = width,
                });
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
