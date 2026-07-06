using System.Collections.Generic;
using TracingSystem.Model;
using TracingSystem.View;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TracingSystem
{
    public class ShapeTracingPresenter : MonoBehaviour
    {
        [Header("Configs and references")]
        public LineTracerView lineTracerViewPrefab;
    
        public ShapeMaskView shapeMaskView;
        
        public LevelModel levelModel = new ();
        
        [Header("Live Objects")]
        public AssetReferenceSprite shapeAsset;
        public Color mainColor;
        public AssetReferenceT<AudioClip> goalAudio;
        
        public List<LineTracerView> lineTracers;
        
    }
}
