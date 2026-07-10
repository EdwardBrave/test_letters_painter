using System.IO;
using Game;
using UnityEditor;
using UnityEngine;

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
                builder.ExportToJson(presetName);
                AssetDatabase.Refresh();
                Debug.Log($"Exported preset '{presetName}'");
            }

            if (GUILayout.Button("⬇️ Import from JSON"))
            {
                string folderPath = Path.Combine(Application.streamingAssetsPath, "Levels");
                string filePath = EditorUtility.OpenFilePanel("Select Preset JSON", folderPath, "json");
                if (!string.IsNullOrEmpty(filePath))
                {
                    presetName = Path.GetFileNameWithoutExtension(filePath);
                    builder.ImportFromJson(filePath);
                    EditorUtility.SetDirty(builder);
                    Debug.Log($"Imported preset from {filePath}");
                }
            }
        }
    }
}