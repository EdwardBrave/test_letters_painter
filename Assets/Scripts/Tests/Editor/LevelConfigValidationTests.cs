using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Dto;
using Game.Model;
using NUnit.Framework;
using Services;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace Tests.Editor
{
    public class LevelConfigValidationTests
    {
        private readonly LevelSerializationService _serializationService = new();

        [Test]
        public void ProjectContainsLevelConfigs()
        {
            Assert.That(GetLevelFilePaths(), Is.Not.Empty, "No level JSON configs were found.");
        }

        [Test]
        public void AllLevelConfigsPassDtoValidation()
        {
            foreach (string path in GetLevelFilePaths())
            {
                Assert.DoesNotThrow(
                    () => _serializationService.ReadFromFilePath(path),
                    $"Level config failed DTO validation: {path}");
            }
        }

        [Test]
        public void AllLevelConfigsHaveValidCategoryAndId()
        {
            foreach (string path in GetLevelFilePaths())
            {
                LevelDto dto = _serializationService.ReadFromFilePath(path);
                string levelId = GetLevelId(path, dto);

                Assert.That(dto.category, Is.Not.EqualTo(LevelCategory.None), $"Level has no valid category: {path}");
                Assert.That(levelId, Is.Not.Empty, $"Level has no id/name: {path}");
                Assert.That(
                    Path.GetFileName(Path.GetDirectoryName(path)),
                    Is.EqualTo(dto.category.ToString()),
                    $"Level category does not match its folder: {path}");
            }
        }

        [Test]
        public void LevelIdsAreUnique()
        {
            var duplicateGroups = GetLevelFilePaths()
                .Select(path =>
                {
                    LevelDto dto = _serializationService.ReadFromFilePath(path);
                    return new LevelIdEntry(GetLevelId(path, dto), path);
                })
                .GroupBy(entry => entry.Id)
                .Where(group => group.Count() > 1)
                .ToArray();

            Assert.That(
                duplicateGroups,
                Is.Empty,
                "Duplicate level ids found: " + string.Join("; ", duplicateGroups.Select(FormatDuplicateGroup)));
        }

        [Test]
        public void LevelAddressableReferencesResolve()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            Assert.That(settings, Is.Not.Null, "AddressableAssetSettings could not be loaded.");

            foreach (string path in GetLevelFilePaths())
            {
                LevelDto dto = _serializationService.ReadFromFilePath(path);
                AssertAddressableExists(settings, dto.shapeAssetGUID, nameof(dto.shapeAssetGUID), path);
                AssertAddressableExists(settings, dto.goalAudioAssetGUID, nameof(dto.goalAudioAssetGUID), path);
            }
        }

        private static string[] GetLevelFilePaths()
        {
            if (!Directory.Exists(LevelSerializationService.LevelsFolder))
            {
                return System.Array.Empty<string>();
            }

            return Directory
                .GetFiles(LevelSerializationService.LevelsFolder, "*.json", SearchOption.AllDirectories)
                .OrderBy(path => path)
                .ToArray();
        }

        private static string GetLevelId(string path, LevelDto dto)
        {
            return string.IsNullOrWhiteSpace(dto.name)
                ? Path.GetFileNameWithoutExtension(path)
                : dto.name;
        }

        private static void AssertAddressableExists(
            AddressableAssetSettings settings,
            string guid,
            string fieldName,
            string levelPath)
        {
            Assert.That(guid, Is.Not.Null, $"Level '{levelPath}' is missing {fieldName}.");
            Assert.That(guid, Is.Not.Empty, $"Level '{levelPath}' is missing {fieldName}.");
            Assert.That(
                settings.FindAssetEntry(guid, true),
                Is.Not.Null,
                $"Level '{levelPath}' has unresolved Addressables reference {fieldName}: {guid}");
        }

        private static string FormatDuplicateGroup(IGrouping<string, LevelIdEntry> group)
        {
            return $"{group.Key} => {string.Join(", ", group.Select(entry => entry.Path))}";
        }

        private readonly struct LevelIdEntry
        {
            public readonly string Id;
            public readonly string Path;

            public LevelIdEntry(string id, string path)
            {
                Id = id;
                Path = path;
            }
        }
    }
}
