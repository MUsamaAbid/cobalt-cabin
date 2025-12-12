using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoardSystem boardSystem;
    [SerializeField] private GameUIController uiController;

    [Header("Level Configuration")]
    [SerializeField] private LevelDataConfig levelDataConfig;

    private GameScoreSystem _scoreSystem;

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        _scoreSystem = new GameScoreSystem();
        
        if (uiController != null)
        {
            uiController.Init(_scoreSystem);
        }

        if (boardSystem != null)
        {
            boardSystem.Init(levelDataConfig, _scoreSystem, uiController);
        }
    }

    public void RestartGame()
    {
        _scoreSystem.Reset();
        
        if (boardSystem != null)
        {
            boardSystem.Init(levelDataConfig, _scoreSystem, uiController);
        }
    }
}
