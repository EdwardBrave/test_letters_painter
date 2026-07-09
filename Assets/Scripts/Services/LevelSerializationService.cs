using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Tools;
using TracingSystem.Dto;
using UnityEngine;

namespace Services
{
    
    public class LevelSerializationService
    {
        public static string LevelsFolder => Path.Combine(Application.streamingAssetsPath, "Levels");

        public string GetLevelPath(string levelName) => Path.Combine(LevelsFolder, levelName + ".json");


        public string[] GetLevelNames()
        {
            return Directory.GetFiles(LevelsFolder, "*.json", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();
        }
        
        public LevelDto ReadFromFile(string levelName, bool isFullPath = false)
        {
            string path = isFullPath ? levelName : GetLevelPath(levelName);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Level file not found for '{levelName}': {path}", path);
            }

            string json = File.ReadAllText(path);
            try
            {
                var dto = JsonConvert.DeserializeObject<LevelDto>(json, UnityJsonConverters.Settings);
                ValidateDto(dto, levelName);
                return dto;
            }
            catch (Exception e)
            {
                throw new InvalidDataException($"Level '{levelName}' contains invalid JSON: {e.Message}", e);
            }
        }

        public void WriteToFile(LevelDto dto, string levelName)
        {
            ValidateDto(dto, levelName);

            if (!Directory.Exists(LevelsFolder))
            {
                Directory.CreateDirectory(LevelsFolder);
            }

            File.WriteAllText(GetLevelPath(levelName),
                JsonConvert.SerializeObject(dto, UnityJsonConverters.Settings));
            Debug.Log($"[LevelSerialization] Saved level '{levelName}' to {GetLevelPath(levelName)}");
        }

        private void ValidateDto(LevelDto dto, string levelName)
        {
            if (string.IsNullOrEmpty(dto.shapeAssetGUID))
            {
                throw new InvalidDataException($"Level '{levelName}' is missing a shape asset GUID.");
            }

            if (dto.lines == null || dto.lines.Length == 0)
            {
                throw new InvalidDataException($"Level '{levelName}' has no lines.");
            }

            foreach (var line in dto.lines)
            {
                if (line.points == null || line.points.Length < 2)
                {
                    throw new InvalidDataException($"Level '{levelName}' has a line with fewer than 2 points.");
                }
            }
        }
    }
}
