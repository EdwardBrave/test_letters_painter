using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace TracingSystem.Model
{
    [Serializable]
    public class LevelModel
    {
        public string shapeAssetGUID;
        public Color mainColor;
        public string goalAudioAssetGUID;
        public List<LineModel> lines;

        [JsonIgnore]
        public int ActiveLine { get; set; } = -1;
    }
}