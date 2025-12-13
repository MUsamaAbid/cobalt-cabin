using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoardSystem boardSystem;
    [SerializeField] private GameUIController uiController;
    [SerializeField] private AudioManager audioManager;

    [Header("Level Configuration")]
    [SerializeField] private LevelDataConfig levelDataConfig;

    [Header("Save Settings")]
    [SerializeField] private bool autoSaveOnPause = true;
    [SerializeField] private bool loadSaveOnStart = true;

    private GameScoreSystem _scoreSystem;
    private SaveLoadSystem _saveLoadSystem;
    private string _currentLevelPath;

    void Start()
    {
        _saveLoadSystem = new SaveLoadSystem();

        if (loadSaveOnStart && _saveLoadSystem.HasSaveData())
        {
            LoadGame();
        }
        else
        {
            StartNewGame();
        }

        if (uiController != null)
        {
            uiController.OnRestartButtonClicked += OnRestartGame;
        }
    }

    void StartNewGame()
    {
        _scoreSystem = new GameScoreSystem();
        
        if (uiController != null)
        {
            uiController.Init(_scoreSystem);
        }

        if (boardSystem != null && levelDataConfig != null)
        {
            _currentLevelPath = GetAssetPath(levelDataConfig);
            boardSystem.Init(levelDataConfig, _scoreSystem, uiController, audioManager);
            boardSystem.OnGameCompletedEvent += OnLevelCompleted;
        }
    }

    void LoadGame()
    {
        GameSaveData saveData = _saveLoadSystem.LoadGame();
        
        if (saveData == null)
        {
            StartNewGame();
            return;
        }

        _scoreSystem = new GameScoreSystem();
        
        if (uiController != null)
        {
            uiController.Init(_scoreSystem);
        }

        LevelDataConfig savedLevel = levelDataConfig;
        if (!string.IsNullOrEmpty(saveData.levelConfigPath))
        {
            LevelDataConfig loadedLevel = Resources.Load<LevelDataConfig>(saveData.levelConfigPath);
            if (loadedLevel != null)
            {
                savedLevel = loadedLevel;
            }
        }

        _currentLevelPath = GetAssetPath(savedLevel);

        if (boardSystem != null)
        {
            boardSystem.Init(savedLevel, _scoreSystem, uiController, audioManager);
            boardSystem.OnGameCompletedEvent += OnLevelCompleted;
        }

        _scoreSystem.LoadFromSave(saveData);

        if (boardSystem != null && boardSystem.CardController != null)
        {
            boardSystem.CardController.RestoreCardStates(saveData.cards);
        }

        Debug.Log("Game loaded successfully!");
    }

    public void SaveGame()
    {
        if (_scoreSystem == null || boardSystem == null || boardSystem.CardController == null)
            return;

        GameSaveData saveData = _scoreSystem.CreateSaveData(
            _currentLevelPath,
            boardSystem.CardController.CardsOnBoard
        );

        _saveLoadSystem.SaveGame(saveData);
    }

    public void OnRestartGame()
    {
        _saveLoadSystem.DeleteSave();
        
        if (boardSystem != null)
        {
            boardSystem.OnGameCompletedEvent -= OnLevelCompleted;
        }

        StartNewGame();
    }

    void OnLevelCompleted()
    {
        Debug.Log("Level completed! Deleting save...");
        _saveLoadSystem.DeleteSave();
    }

    void OnApplicationPause(bool isPaused)
    {
        if (isPaused && autoSaveOnPause)
        {
            SaveGame();
        }
    }

    void OnApplicationQuit()
    {
        if (autoSaveOnPause)
        {
            SaveGame();
        }
    }

    void OnDestroy()
    {
        if (uiController != null)
        {
            uiController.OnRestartButtonClicked -= OnRestartGame;
        }

        if (boardSystem != null)
        {
            boardSystem.OnGameCompletedEvent -= OnLevelCompleted;
        }
    }

    string GetAssetPath(Object asset)
    {
        if (asset == null)
            return string.Empty;

        return asset.name;
    }
}
