using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace TracingSystem.Model
{
    [Serializable]
    public class LineModel
    {
        public float width;
        public List<Vector2> points;
        
        [JsonIgnore]
        public float Progress { get; set; } = 0f;
    }
}