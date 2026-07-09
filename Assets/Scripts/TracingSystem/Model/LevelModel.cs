using System;
using System.Collections.Generic;
using Services;
using Tools;
using UnityEngine;

namespace TracingSystem.Model
{
    public enum LevelCategory
    {
        None,
        Letter,
        Number,
        Shape
    }
    
    public class LightLevelModel : IDisposeNotifier
    {
        public string Name { get; private set; }
        public LevelCategory Category { get; private set; }

        public Color MainColor { get; private set;}
        public Sprite Shape { get; private set;}

        private bool _disposed;

        public LightLevelModel() { }
        
        public LightLevelModel(string name, LevelCategory category, Color mainColor, Sprite shape)
        {
            Name = name;
            Category = category;
            MainColor = mainColor;
            Shape = shape;
        }

        public event Action OnDispose;

        /// <summary>Idempotent. Raises <see cref="OnDispose"/> once; releases nothing directly.</summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            OnDispose?.Invoke();
            OnDispose = null;
        }
    }
    
    public class FullLevelModel : LightLevelModel
    {
        public AudioClip GoalAudio { get; private set;}
        public IReadOnlyList<LineModel> Lines { get; private set; } = new List<LineModel>();
        public int ActiveLine { get; set; } = -1;

        public FullLevelModel() { }
        
        public FullLevelModel(
            string name,
            LevelCategory category,
            Color mainColor,
            Sprite shape,
            AudioClip goalAudio,
            IReadOnlyList<LineModel> lines)
            : base(name, category, mainColor, shape)
        {
            GoalAudio = goalAudio;
            Lines = lines ?? Array.Empty<LineModel>();
        }
    }
}
