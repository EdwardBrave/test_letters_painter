using System;
using UnityEngine;

namespace Game.Model
{
    public class LineModel
    {
        public float Width { get; private set; }
        public Vector2[] Points { get; private set; }

        public event Action<float> OnProgressChanged;
        
        private float _progress = 0f;
        public float Progress
        {
            get => _progress;
            set
            {
                if (Mathf.Approximately(value, _progress))
                {
                    return;
                }
                
                _progress = value;
                OnProgressChanged?.Invoke(_progress);
            }
        }

        public LineModel(float width, Vector2[] points)
        {
            Width = width;
            Points = points;
        }
    }
}