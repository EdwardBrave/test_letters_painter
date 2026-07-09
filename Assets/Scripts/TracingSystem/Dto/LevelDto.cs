using System;
using TracingSystem.Model;
using UnityEngine;

namespace TracingSystem.Dto
{
    [Serializable]
    public struct LevelDto
    {
        public LevelCategory category;
        public string name;
        
        public string shapeAssetGUID;
        public string goalAudioAssetGUID;
        public Color mainColor;
        public LineDto[] lines;
        
        public struct LineDto
        {
            public float width;
            public Vector2[] points;
        }
    }
}
