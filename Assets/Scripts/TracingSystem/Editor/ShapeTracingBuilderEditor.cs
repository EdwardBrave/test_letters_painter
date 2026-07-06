using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Tools;
using TracingSystem.Model;

namespace TracingSystem.Editor
{
    [CustomEditor(typeof(LevelModelBuilder))]
    public class ShapeTracingBuilderEditor : UnityEditor.Editor
    {
        private string presetName = "NewShapePreset";

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LevelModelBuilder builder = (LevelModelBuilder)target;
            if (GUILayout.Button("Add Line"))
            {
                builder.InstantiateLineTracerView();
                builder.OnValidate();
            }
            
            GUILayout.Space(20);
            GUILayout.Label("Preset Management", EditorStyles.boldLabel);

            presetName = EditorGUILayout.TextField("Preset Name", presetName);
            
            if (GUILayout.Button("⬆️ Export to JSON"))
            {
                ExportToJson(builder);
            }
            
            if (GUILayout.Button("⬇️ Import from JSON"))
            {
                ImportFromJson(builder);
            }
        }

        private void ExportToJson(LevelModelBuilder builder)
        {
            string folderPath = Path.Combine(Application.streamingAssetsPath, "Levels");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, presetName + ".json");
            string json = JsonConvert.SerializeObject(builder.BuildLevelModel(), UnityJsonConverters.Settings);
            File.WriteAllText(filePath, json);

            AssetDatabase.Refresh();
            Debug.Log($"Exported preset to {filePath}");
        }

        private void ImportFromJson(LevelModelBuilder builder)
        {
            string folderPath = Path.Combine(Application.streamingAssetsPath, "Levels");
            string filePath = EditorUtility.OpenFilePanel("Select Preset JSON", folderPath, "json");

            if (string.IsNullOrEmpty(filePath)) return;

            string json = File.ReadAllText(filePath);
            LevelModel preset = JsonConvert.DeserializeObject<LevelModel>(json, UnityJsonConverters.Settings);

            builder.ApplyLevelModel(preset);

            EditorUtility.SetDirty(builder);
            Debug.Log($"Imported preset from {filePath}");
        }
    }
}