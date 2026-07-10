using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Dto;
using Game.Model;
using Newtonsoft.Json;
using Tools;
using UnityEngine;

namespace Services
{
    
    public class LevelSerializationService
    {
        public static string LevelsFolder => Path.Combine(Application.streamingAssetsPath, "Levels");

        public string GetLevelPath(LevelCategory category, string levelName) => 
            Path.Combine(LevelsFolder, category.ToString(), levelName + ".json");

        private Dictionary<LevelCategory, string[]> _cachedLevelNames;

        public Dictionary<LevelCategory, string[]> GetLevelNames()
        {
            return _cachedLevelNames ??= LoadLevelNames();
        }

        public string[] GetLevelNames(LevelCategory category)
        {
            _cachedLevelNames ??= LoadLevelNames();
            return _cachedLevelNames[category];
        }
        
        private Dictionary<LevelCategory, string[]> LoadLevelNames()
        {
            char[] separators = new char[] { '/', '\\', Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
            return Directory.GetFiles(LevelsFolder, "*.json", SearchOption.AllDirectories)
                .GroupBy(x => 
                    Enum.TryParse<LevelCategory>(x.Split(separators, StringSplitOptions.RemoveEmptyEntries)[^2], out var category)
                        ? category
                        : LevelCategory.None)
                .ToDictionary(
                    grouping => grouping.Key, 
                    grouping => grouping.Select(Path.GetFileNameWithoutExtension).ToArray());
        }

        public LevelDto ReadFromFile(LevelCategory category, string levelName)
        {  
            return ReadFromFilePath(GetLevelPath(category, levelName));
        }

        public LevelDto ReadFromFilePath(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Level file not found: {path}", path);
            }

            string json = File.ReadAllText(path);
            try
            {
                var dto = JsonConvert.DeserializeObject<LevelDto>(json, UnityJsonConverters.Settings);
                ValidateDto(dto, path);
                return dto;
            }
            catch (Exception e)
            {
                throw new InvalidDataException($"Level '{path}' contains invalid JSON: {e.Message}", e);
            }
        }

        public void WriteToFile(LevelDto dto, string levelName)
        {
            ValidateDto(dto, levelName);

            if (!Directory.Exists(LevelsFolder))
            {
                Directory.CreateDirectory(LevelsFolder);
            }

            File.WriteAllText(GetLevelPath(dto.category, levelName),
                JsonConvert.SerializeObject(dto, UnityJsonConverters.Settings));
            Debug.Log($"[LevelSerialization] Saved level '{levelName}' to {GetLevelPath(dto.category, levelName)}");
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
