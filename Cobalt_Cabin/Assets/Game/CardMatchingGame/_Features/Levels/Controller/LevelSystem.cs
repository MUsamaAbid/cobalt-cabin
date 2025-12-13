using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelDataSystem levelDataSystem;

    private const string LEVEL_INDEX_KEY = "CurrentLevelIndex";
    private const string ROTATING_INDEX_KEY = "CurrentRotatingIndex";

    private int _currentLevelIndex;
    private int _currentRotatingIndex;
    private bool _isInRotatingPhase;

    public LevelDataSystem DataSystem => levelDataSystem;
    public int CurrentLevelIndex => _currentLevelIndex;
    public bool IsInRotatingPhase => _isInRotatingPhase;
    public int CurrentRotatingCycle => _currentRotatingIndex / levelDataSystem.TotalRotatingLevels;

    void Awake()
    {
        LoadProgress();
    }

    public void LoadProgress()
    {
        if (!ValidateDataSystem())
            return;

        _currentLevelIndex = PlayerPrefs.GetInt(LEVEL_INDEX_KEY, 0);
        _currentRotatingIndex = PlayerPrefs.GetInt(ROTATING_INDEX_KEY, 0);

        _isInRotatingPhase = _currentLevelIndex >= levelDataSystem.TotalMainLevels;

        if (_isInRotatingPhase && !levelDataSystem.HasRotatingLevels)
        {
            Debug.LogWarning("In rotating phase but no rotating levels available. Resetting to last main level.");
            _currentLevelIndex = Mathf.Max(0, levelDataSystem.TotalMainLevels - 1);
            _isInRotatingPhase = false;
        }

        LogCurrentProgress();
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt(LEVEL_INDEX_KEY, _currentLevelIndex);
        PlayerPrefs.SetInt(ROTATING_INDEX_KEY, _currentRotatingIndex);
        PlayerPrefs.Save();

        Debug.Log($"Progress saved: Level {_currentLevelIndex}, Rotating Index {_currentRotatingIndex}");
    }

    public LevelDataConfig GetCurrentLevel()
    {
        if (!ValidateDataSystem())
            return null;

        if (_isInRotatingPhase)
        {
            if (!levelDataSystem.HasRotatingLevels)
            {
                Debug.LogError("No rotating levels available!");
                return null;
            }

            int rotatingIndex = _currentRotatingIndex % levelDataSystem.TotalRotatingLevels;
            return levelDataSystem.rotatingLevels[rotatingIndex];
        }
        else
        {
            if (_currentLevelIndex < 0 || _currentLevelIndex >= levelDataSystem.TotalMainLevels)
            {
                Debug.LogWarning($"Invalid main level index {_currentLevelIndex}. Resetting to 0.");
                _currentLevelIndex = 0;
            }

            return levelDataSystem.levels[_currentLevelIndex];
        }
    }

    public void AdvanceToNextLevel()
    {
        if (!ValidateDataSystem())
            return;

        if (_isInRotatingPhase)
        {
            _currentRotatingIndex++;
            _currentLevelIndex++;

            int cycleNumber = CurrentRotatingCycle;
            int positionInCycle = _currentRotatingIndex % levelDataSystem.TotalRotatingLevels;

            Debug.Log($"Advanced in rotating levels: Cycle {cycleNumber + 1}, Position {positionInCycle + 1}/{levelDataSystem.TotalRotatingLevels}");
        }
        else
        {
            if (_currentLevelIndex < levelDataSystem.TotalMainLevels - 1)
            {
                _currentLevelIndex++;
                Debug.Log($"Advanced to Main Level {_currentLevelIndex + 1}/{levelDataSystem.TotalMainLevels}");
            }
            else
            {
                if (levelDataSystem.HasRotatingLevels)
                {
                    _isInRotatingPhase = true;
                    _currentLevelIndex = levelDataSystem.TotalMainLevels;
                    _currentRotatingIndex = 0;
                    Debug.Log($"Main levels complete! Starting rotating levels. Cycle 1, Level 1/{levelDataSystem.TotalRotatingLevels}");
                }
                else
                {
                    Debug.Log("All levels complete! No rotating levels available.");
                }
            }
        }

        SaveProgress();
    }

    public void RestartCurrentLevel()
    {
        SaveProgress();
        LogCurrentProgress();
    }

    public void ResetProgress()
    {
        _currentLevelIndex = 0;
        _currentRotatingIndex = 0;
        _isInRotatingPhase = false;
        SaveProgress();
        Debug.Log("Progress reset to Level 1.");
    }

    public string GetCurrentLevelDisplayName()
    {
        if (_isInRotatingPhase)
        {
            int cycleNumber = CurrentRotatingCycle + 1;
            int positionInCycle = (_currentRotatingIndex % levelDataSystem.TotalRotatingLevels) + 1;
            return $"Rotating Level {positionInCycle} (Cycle {cycleNumber})";
        }
        else
        {
            return $"Level {_currentLevelIndex + 1}";
        }
    }

    public string GetProgressSummary()
    {
        if (_isInRotatingPhase)
        {
            int cycleNumber = CurrentRotatingCycle + 1;
            int positionInCycle = (_currentRotatingIndex % levelDataSystem.TotalRotatingLevels) + 1;
            return $"Rotating: Cycle {cycleNumber}, {positionInCycle}/{levelDataSystem.TotalRotatingLevels}";
        }
        else
        {
            return $"Main: {_currentLevelIndex + 1}/{levelDataSystem.TotalMainLevels}";
        }
    }

    private bool ValidateDataSystem()
    {
        if (levelDataSystem == null)
        {
            Debug.LogError("LevelDataSystem is not assigned to LevelSystem!");
            return false;
        }

        if (!levelDataSystem.HasLevels)
        {
            Debug.LogError("LevelDataSystem has no main levels!");
            return false;
        }

        return true;
    }

    private void LogCurrentProgress()
    {
        string levelName = GetCurrentLevelDisplayName();
        string summary = GetProgressSummary();
        Debug.Log($"Current Progress: {levelName} | {summary}");
    }

    public void JumpToLevel(int mainLevelIndex)
    {
        if (mainLevelIndex < 0 || mainLevelIndex >= levelDataSystem.TotalMainLevels)
        {
            Debug.LogError($"Cannot jump to level {mainLevelIndex}. Valid range: 0-{levelDataSystem.TotalMainLevels - 1}");
            return;
        }

        _currentLevelIndex = mainLevelIndex;
        _isInRotatingPhase = false;
        _currentRotatingIndex = 0;
        SaveProgress();
        Debug.Log($"Jumped to Main Level {_currentLevelIndex + 1}");
    }

    public void JumpToRotatingLevel(int rotatingIndex)
    {
        if (!levelDataSystem.HasRotatingLevels)
        {
            Debug.LogError("No rotating levels available!");
            return;
        }

        _isInRotatingPhase = true;
        _currentRotatingIndex = rotatingIndex;
        _currentLevelIndex = levelDataSystem.TotalMainLevels + rotatingIndex;
        SaveProgress();

        int cycleNumber = CurrentRotatingCycle + 1;
        int positionInCycle = (_currentRotatingIndex % levelDataSystem.TotalRotatingLevels) + 1;
        Debug.Log($"Jumped to Rotating Level: Cycle {cycleNumber}, Position {positionInCycle}");
    }
}
