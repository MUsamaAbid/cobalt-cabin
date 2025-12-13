using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data System", menuName = "CardMatch/Level Data System", order = 0)]
public class LevelDataSystem : ScriptableObject
{
    [Header("Main Levels")]
    [Tooltip("Main story levels - played once in order")]
    public List<LevelDataConfig> levels = new List<LevelDataConfig>();

    [Header("Rotating Levels")]
    [Tooltip("Repeating levels after main levels are complete - loops infinitely")]
    public List<LevelDataConfig> rotatingLevels = new List<LevelDataConfig>();

    [Header("Level Progression Settings")]
    [Tooltip("Current level index (0-based). Saved in PlayerPrefs.")]
    public int currentLevelIndex = 0;

    private const string LEVEL_PROGRESS_KEY = "CurrentLevelIndex";

    public int TotalMainLevels => levels != null ? levels.Count : 0;
    public int TotalRotatingLevels => rotatingLevels != null ? rotatingLevels.Count : 0;
    public int TotalLevels => TotalMainLevels;
    public bool HasLevels => TotalMainLevels > 0;
    public bool HasRotatingLevels => TotalRotatingLevels > 0;
    public bool IsInMainLevels => currentLevelIndex < TotalMainLevels;
    public bool IsInRotatingLevels => currentLevelIndex >= TotalMainLevels;
    public bool IsLastMainLevel => currentLevelIndex >= TotalMainLevels - 1;
    public bool HasNextLevel => currentLevelIndex < TotalMainLevels - 1;

    public LevelDataConfig GetCurrentLevel()
    {
        if (!HasLevels)
        {
            Debug.LogError("No levels assigned to LevelDataSystem!");
            return null;
        }

        if (currentLevelIndex < 0 || currentLevelIndex >= TotalLevels)
        {
            Debug.LogWarning($"Invalid level index {currentLevelIndex}. Resetting to 0.");
            currentLevelIndex = 0;
        }

        return levels[currentLevelIndex];
    }

    public LevelDataConfig GetLevel(int levelIndex)
    {
        if (!HasLevels || levelIndex < 0 || levelIndex >= TotalLevels)
        {
            Debug.LogError($"Invalid level index: {levelIndex}. Total levels: {TotalLevels}");
            return null;
        }

        return levels[levelIndex];
    }

    public void LoadLevelProgress()
    {
        if (PlayerPrefs.HasKey(LEVEL_PROGRESS_KEY))
        {
            currentLevelIndex = PlayerPrefs.GetInt(LEVEL_PROGRESS_KEY, 0);
            Debug.Log($"Level progress loaded: Level {currentLevelIndex + 1}/{TotalLevels}");
        }
        else
        {
            currentLevelIndex = 0;
            Debug.Log("No level progress found. Starting from Level 1.");
        }

        if (currentLevelIndex >= TotalLevels)
        {
            Debug.LogWarning($"Saved level index {currentLevelIndex} exceeds total levels {TotalLevels}. Resetting to last level.");
            currentLevelIndex = TotalLevels - 1;
        }
    }

    public void SaveLevelProgress()
    {
        PlayerPrefs.SetInt(LEVEL_PROGRESS_KEY, currentLevelIndex);
        PlayerPrefs.Save();
        Debug.Log($"Level progress saved: Level {currentLevelIndex + 1}/{TotalLevels}");
    }

    public void AdvanceToNextLevel()
    {
        if (HasNextLevel)
        {
            currentLevelIndex++;
            SaveLevelProgress();
            Debug.Log($"Advanced to Level {currentLevelIndex + 1}/{TotalLevels}");
        }
        else
        {
            Debug.Log("Already at the last level!");
        }
    }

    public void RestartCurrentLevel()
    {
        SaveLevelProgress();
        Debug.Log($"Restarting Level {currentLevelIndex + 1}/{TotalLevels}");
    }

    public void ResetProgress()
    {
        currentLevelIndex = 0;
        SaveLevelProgress();
        Debug.Log("Level progress reset to Level 1.");
    }

    public void GoToLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < TotalLevels)
        {
            currentLevelIndex = levelIndex;
            SaveLevelProgress();
            Debug.Log($"Jumped to Level {currentLevelIndex + 1}/{TotalLevels}");
        }
        else
        {
            Debug.LogError($"Cannot go to level {levelIndex}. Valid range: 0-{TotalLevels - 1}");
        }
    }

    public string GetLevelDisplayName(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= TotalLevels)
            return "Invalid Level";

        return $"Level {levelIndex + 1}";
    }

    public string GetCurrentLevelDisplayName()
    {
        return GetLevelDisplayName(currentLevelIndex);
    }
}
