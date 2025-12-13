using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoardSystem boardSystem;
    [SerializeField] private GameUIController uiController;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private DynamicCameraController cameraController;

    [Header("Level System")]
    [SerializeField] private LevelSystem levelSystem;

    [Header("Save Settings")]
    [SerializeField] private bool autoSaveOnPause = true;
    [SerializeField] private bool loadSaveOnStart = true;

    private GameScoreSystem _scoreSystem;
    private SaveLoadSystem _saveLoadSystem;
    private bool _justRestarted = false;

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
            uiController.OnWinRestartButtonClicked += OnRestartGame;
            uiController.OnFailRestartButtonClicked += OnRestartGame;
            uiController.OnPauseRestartButtonClicked += OnRestartGame;
            uiController.OnNextLevelButtonClicked += OnNextLevel;
        }
    }

    void StartNewGame()
    {
        if (levelSystem == null)
        {
            Debug.LogError("LevelSystem is not assigned!");
            return;
        }

        LevelDataConfig currentLevel = levelSystem.GetCurrentLevel();
        if (currentLevel == null)
        {
            Debug.LogError("Failed to get current level!");
            return;
        }

        _scoreSystem = new GameScoreSystem();
        
        if (uiController != null)
        {
            uiController.Init(_scoreSystem);
        }

        if (boardSystem != null)
        {
            boardSystem.Init(currentLevel, _scoreSystem, uiController, audioManager);
            boardSystem.OnGameCompletedEvent += OnLevelCompleted;
            
            if (cameraController != null)
            {
                cameraController.AdjustCameraToGrid(currentLevel.rows, currentLevel.columns);
            }
        }

        if (_scoreSystem != null)
        {
            _scoreSystem.OnLevelFailed += OnLevelFailed;
        }

        Debug.Log($"Started {levelSystem.GetCurrentLevelDisplayName()} | {levelSystem.GetProgressSummary()}");
    }

    void LoadGame()
    {
        GameSaveData saveData = _saveLoadSystem.LoadGame();
        
        if (saveData == null)
        {
            StartNewGame();
            return;
        }

        if (levelSystem == null)
        {
            Debug.LogError("LevelSystem is not assigned!");
            return;
        }

        LevelDataConfig savedLevel = levelSystem.GetCurrentLevel();
        
        if (savedLevel == null)
        {
            Debug.LogWarning("Saved level not found. Starting new game.");
            StartNewGame();
            return;
        }

        _scoreSystem = new GameScoreSystem();
        
        if (uiController != null)
        {
            uiController.Init(_scoreSystem);
        }

        if (boardSystem != null)
        {
            boardSystem.InitFromSave(savedLevel, _scoreSystem, saveData.cards, uiController, audioManager);
            boardSystem.OnGameCompletedEvent += OnLevelCompleted;
            
            if (cameraController != null)
            {
                cameraController.AdjustCameraToGrid(savedLevel.rows, savedLevel.columns);
            }
        }

        _scoreSystem.LoadFromSave(saveData);

        if (boardSystem != null && boardSystem.CardController != null)
        {
            boardSystem.CardController.RestoreCardStates(saveData.cards);
        }

        if (_scoreSystem != null)
        {
            _scoreSystem.OnLevelFailed += OnLevelFailed;
        }

        Debug.Log($"Game loaded! {levelSystem.GetCurrentLevelDisplayName()} | {levelSystem.GetProgressSummary()}");
    }

    public void SaveGame()
    {
        if (_justRestarted)
        {
            Debug.Log("[GameManager] Not saving - game was just restarted. Make progress first!");
            return;
        }

        if (_scoreSystem == null || boardSystem == null || boardSystem.CardController == null)
            return;

        if (levelSystem == null)
            return;

        GameSaveData saveData = _scoreSystem.CreateSaveData(
            levelSystem.CurrentLevelIndex,
            boardSystem.CardController.CardsOnBoard
        );

        _saveLoadSystem.SaveGame(saveData);
    }

    public void OnRestartGame()
    {
        Debug.Log("[GameManager] Restarting level...");
        
        _saveLoadSystem.DeleteSave();
        Debug.Log("[GameManager] Save deleted before restart");
        
        _justRestarted = true;
        Debug.Log("[GameManager] Just restarted - will not auto-save until progress is made");
        
        if (boardSystem != null)
        {
            boardSystem.OnGameCompletedEvent -= OnLevelCompleted;
        }

        if (_scoreSystem != null)
        {
            _scoreSystem.OnLevelFailed -= OnLevelFailed;
        }

        if (uiController != null)
        {
            uiController.HideSummaryPanels();
        }

        if (levelSystem != null)
        {
            levelSystem.RestartCurrentLevel();
        }

        StartNewGame();
        Debug.Log("[GameManager] Restart complete - fresh game started");
    }

    public void OnNextLevel()
    {
        Debug.Log("Loading next level...");
        
        _saveLoadSystem.DeleteSave();
        
        if (boardSystem != null)
        {
            boardSystem.OnGameCompletedEvent -= OnLevelCompleted;
        }

        if (_scoreSystem != null)
        {
            _scoreSystem.OnLevelFailed -= OnLevelFailed;
        }

        if (uiController != null)
        {
            uiController.HideSummaryPanels();
        }

        if (levelSystem != null)
        {
            levelSystem.AdvanceToNextLevel();
        }

        StartNewGame();
    }

    void OnLevelCompleted()
    {
        Debug.Log($"[GameManager] OnLevelCompleted() called!");
        Debug.Log($"{levelSystem.GetCurrentLevelDisplayName()} completed!");
        _saveLoadSystem.DeleteSave();

        if (uiController != null && _scoreSystem != null)
        {
            Debug.Log($"[GameManager] Showing win panel with Score: {_scoreSystem.Score}, Turns: {_scoreSystem.TurnCount}");
            uiController.ShowLevelWinPanel(_scoreSystem.Score, _scoreSystem.TurnCount);
        }
        else
        {
            Debug.LogError($"[GameManager] Cannot show win panel! uiController: {uiController != null}, _scoreSystem: {_scoreSystem != null}");
        }
    }

    void OnLevelFailed()
    {
        Debug.Log($"{levelSystem.GetCurrentLevelDisplayName()} failed! Maximum turns exceeded.");
        
        if (uiController != null && _scoreSystem != null)
        {
            string failMessage = $"Out of Turns!\nMax Turns: {_scoreSystem.MaxTurnCount}";
            uiController.ShowLevelFailPanel(failMessage, _scoreSystem.Score);
        }
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
            Debug.Log("[GameManager] OnApplicationQuit - Saving game before quit");
            SaveGame();
        }
        else
        {
            Debug.Log("[GameManager] OnApplicationQuit - Auto-save disabled, not saving");
        }
    }

    void OnDestroy()
    {
        if (uiController != null)
        {
            uiController.OnRestartButtonClicked -= OnRestartGame;
            uiController.OnWinRestartButtonClicked -= OnRestartGame;
            uiController.OnFailRestartButtonClicked -= OnRestartGame;
            uiController.OnPauseRestartButtonClicked -= OnRestartGame;
            uiController.OnNextLevelButtonClicked -= OnNextLevel;
        }

        if (boardSystem != null)
        {
            boardSystem.OnGameCompletedEvent -= OnLevelCompleted;
        }

        if (_scoreSystem != null)
        {
            _scoreSystem.OnLevelFailed -= OnLevelFailed;
        }
    }

    public LevelDataConfig GetCurrentLevelData()
    {
        return levelSystem != null ? levelSystem.GetCurrentLevel() : null;
    }

    public LevelSystem GetLevelSystem()
    {
        return levelSystem;
    }
}
