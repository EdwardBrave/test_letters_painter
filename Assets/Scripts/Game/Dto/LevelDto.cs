using System;
using Game.Model;
using UnityEngine;

namespace Game.Dto
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
