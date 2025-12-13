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

    private GameScoreSystem _scoreSystem;
    
    public event Action OnRestartButtonClicked;

    void Awake()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
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
        OnRestartButtonClicked?.Invoke();
    }

    void OnDestroy()
    {
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
    }
}
