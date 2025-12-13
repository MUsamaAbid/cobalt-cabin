using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI turnCountText;
    [SerializeField] private TextMeshProUGUI matchesText;
    [SerializeField] private TextMeshProUGUI comboText;
    
    [Header("Button References")]
    [SerializeField] private Button restartButton;

    [Header("Summary Panels")]
    [SerializeField] private GameObject levelWinPanel;
    [SerializeField] private GameObject levelFailPanel;

    [Header("Win Panel UI")]
    [SerializeField] private TextMeshProUGUI winScoreText;
    [SerializeField] private TextMeshProUGUI winTurnsText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button winRestartButton;

    [Header("Fail Panel UI")]
    [SerializeField] private TextMeshProUGUI failMessageText;
    [SerializeField] private TextMeshProUGUI failScoreText;
    [SerializeField] private Button failRestartButton;

    [Header("Pause Panel")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseRestartButton;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;

    private GameScoreSystem _scoreSystem;
    private bool _isPaused = false;
    
    public event Action OnRestartButtonClicked;
    public event Action OnNextLevelButtonClicked;
    public event Action OnWinRestartButtonClicked;
    public event Action OnFailRestartButtonClicked;
    public event Action OnPauseRestartButtonClicked;
    public event Action<bool> OnPauseStateChanged;

    void Awake()
    {
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        }

        if (winRestartButton != null)
        {
            winRestartButton.onClick.AddListener(OnWinRestartClicked);
        }

        if (failRestartButton != null)
        {
            failRestartButton.onClick.AddListener(OnFailRestartClicked);
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnPauseClicked);
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeClicked);
        }

        if (pauseRestartButton != null)
        {
            pauseRestartButton.onClick.AddListener(OnPauseRestartClicked);
        }

        HideSummaryPanels();
        HidePausePanel();
    }

    public void Init(GameScoreSystem scoreSystem)
    {
        _scoreSystem = scoreSystem;
        
        _scoreSystem.OnScoreChanged += UpdateScoreDisplay;
        _scoreSystem.OnTurnCountChanged += UpdateTurnCountDisplay;
        _scoreSystem.OnMatchesChanged += UpdateMatchesDisplay;

        UpdateScoreDisplay(_scoreSystem.Score);
        UpdateTurnCountDisplay(_scoreSystem.TurnCount);
        UpdateMatchesDisplay(_scoreSystem.MatchesFound, _scoreSystem.TotalMatchesInLevel);
        UpdateComboDisplay(0);
    }

    void UpdateScoreDisplay(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    void UpdateTurnCountDisplay(int turnCount)
    {
        if (turnCountText != null)
        {
            turnCountText.text = $"Turns: {turnCount}";
        }
    }

    void UpdateMatchesDisplay(int matchesFound, int totalMatches)
    {
        if (matchesText != null)
        {
            matchesText.text = $"Matches: {matchesFound}/{totalMatches}";
        }
    }

    public void UpdateComboDisplay(int combo)
    {
        if (comboText != null)
        {
            if (combo > 1)
            {
                comboText.gameObject.SetActive(true);
                comboText.text = $"Combo x{combo}!";
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }

    void OnRestartClicked()
    {
        PlayButtonClickSound();
        OnRestartButtonClicked?.Invoke();
    }

    void OnNextLevelClicked()
    {
        PlayButtonClickSound();
        OnNextLevelButtonClicked?.Invoke();
    }

    void OnWinRestartClicked()
    {
        PlayButtonClickSound();
        HideSummaryPanels();
        OnWinRestartButtonClicked?.Invoke();
    }

    void OnFailRestartClicked()
    {
        PlayButtonClickSound();
        HideSummaryPanels();
        OnFailRestartButtonClicked?.Invoke();
    }

    void OnPauseClicked()
    {
        PlayButtonClickSound();
        TogglePause();
    }

    void OnResumeClicked()
    {
        PlayButtonClickSound();
        TogglePause();
    }

    void OnPauseRestartClicked()
    {
        Debug.Log("Pause Restart button clicked!");
        PlayButtonClickSound();
        if (_isPaused)
        {
            ResumeGame();
        }
        HidePausePanel();
        Debug.Log("Firing OnPauseRestartButtonClicked event...");
        OnPauseRestartButtonClicked?.Invoke();
    }

    void PlayButtonClickSound()
    {
        if (audioManager != null)
        {
            audioManager.PlayButtonClick();
        }
    }

    public void TogglePause()
    {
        if (_isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        ShowPausePanel();
        OnPauseStateChanged?.Invoke(true);
        Debug.Log("Game Paused");
    }

    void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        HidePausePanel();
        OnPauseStateChanged?.Invoke(false);
        Debug.Log("Game Resumed");
    }

    void ShowPausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    void HidePausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public void ShowLevelWinPanel(int finalScore, int totalTurns)
    {
        if (levelWinPanel == null)
        {
            Debug.LogWarning("Level Win Panel is not assigned!");
            return;
        }

        levelWinPanel.SetActive(true);

        if (winScoreText != null)
        {
            winScoreText.text = $"Final Score: {finalScore}";
        }

        if (winTurnsText != null)
        {
            winTurnsText.text = $"Total Turns: {totalTurns}";
        }

        Debug.Log($"Level Win! Score: {finalScore}, Turns: {totalTurns}");
    }

    public void ShowLevelFailPanel(string failMessage, int currentScore)
    {
        if (levelFailPanel == null)
        {
            Debug.LogWarning("Level Fail Panel is not assigned!");
            return;
        }

        levelFailPanel.SetActive(true);

        if (failMessageText != null)
        {
            failMessageText.text = failMessage;
        }

        if (failScoreText != null)
        {
            failScoreText.text = $"Score: {currentScore}";
        }

        Debug.Log($"Level Failed! {failMessage}");
    }

    public void HideSummaryPanels()
    {
        if (levelWinPanel != null)
        {
            levelWinPanel.SetActive(false);
        }

        if (levelFailPanel != null)
        {
            levelFailPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void OnDestroy()
    {
        if (_isPaused)
        {
            Time.timeScale = 1f;
        }

        if (_scoreSystem != null)
        {
            _scoreSystem.OnScoreChanged -= UpdateScoreDisplay;
            _scoreSystem.OnTurnCountChanged -= UpdateTurnCountDisplay;
            _scoreSystem.OnMatchesChanged -= UpdateMatchesDisplay;
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartClicked);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveListener(OnNextLevelClicked);
        }

        if (winRestartButton != null)
        {
            winRestartButton.onClick.RemoveListener(OnWinRestartClicked);
        }

        if (failRestartButton != null)
        {
            failRestartButton.onClick.RemoveListener(OnFailRestartClicked);
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveListener(OnPauseClicked);
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(OnResumeClicked);
        }

        if (pauseRestartButton != null)
        {
            pauseRestartButton.onClick.RemoveListener(OnPauseRestartClicked);
        }
    }
}
