using System;
using UnityEngine;

public class GameScoreSystem
{
    private int _score = 0;
    private int _turnCount = 0;
    private int _consecutiveMatches = 0;
    private int _matchesFound = 0;
    private int _totalMatchesInLevel = 0;
    private int _maxTurnCount = -1;
    private bool _isRestrainedLevel = false;

    private const int BASE_MATCH_SCORE = 100;
    private const int COMBO_BONUS = 50;

    public int Score => _score;
    public int TurnCount => _turnCount;
    public int ConsecutiveMatches => _consecutiveMatches;
    public int MatchesFound => _matchesFound;
    public int TotalMatchesInLevel => _totalMatchesInLevel;
    public int MaxTurnCount => _maxTurnCount;
    public bool IsRestrainedLevel => _isRestrainedLevel;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnTurnCountChanged;
    public event Action<int, int> OnMatchesChanged;
    public event Action OnLevelFailed;

    public void SetTotalMatchesInLevel(int totalMatches)
    {
        _totalMatchesInLevel = totalMatches;
        OnMatchesChanged?.Invoke(_matchesFound, _totalMatchesInLevel);
    }

    public void SetLevelConstraints(bool isRestrained, int maxTurns)
    {
        _isRestrainedLevel = isRestrained;
        _maxTurnCount = maxTurns;
    }

    public void RecordTurn()
    {
        _turnCount++;
        OnTurnCountChanged?.Invoke(_turnCount);

        if (_isRestrainedLevel && _maxTurnCount > 0 && _turnCount >= _maxTurnCount)
        {
            if (_matchesFound < _totalMatchesInLevel)
            {
                Debug.Log($"Level Failed! Exceeded max turns: {_turnCount}/{_maxTurnCount}");
                OnLevelFailed?.Invoke();
            }
        }
    }

    public void RecordMatch()
    {
        _consecutiveMatches++;
        _matchesFound++;
        
        int matchScore = BASE_MATCH_SCORE;
        if (_consecutiveMatches > 1)
        {
            matchScore += COMBO_BONUS * (_consecutiveMatches - 1);
        }

        _score += matchScore;
        OnScoreChanged?.Invoke(_score);
        OnMatchesChanged?.Invoke(_matchesFound, _totalMatchesInLevel);

        Debug.Log($"Match! Score: +{matchScore} (Combo x{_consecutiveMatches}). Total: {_score}");
    }

    public void RecordMismatch()
    {
        _consecutiveMatches = 0;
    }

    public void Reset()
    {
        _score = 0;
        _turnCount = 0;
        _consecutiveMatches = 0;
        _matchesFound = 0;
        OnScoreChanged?.Invoke(_score);
        OnTurnCountChanged?.Invoke(_turnCount);
        OnMatchesChanged?.Invoke(_matchesFound, _totalMatchesInLevel);
    }

    public void LoadFromSave(GameSaveData saveData)
    {
        _score = saveData.score;
        _turnCount = saveData.turnCount;
        _matchesFound = saveData.matchesFound;
        _consecutiveMatches = saveData.consecutiveMatches;
        
        OnScoreChanged?.Invoke(_score);
        OnTurnCountChanged?.Invoke(_turnCount);
        OnMatchesChanged?.Invoke(_matchesFound, _totalMatchesInLevel);
    }

    public GameSaveData CreateSaveData(int currentLevelIndex, Card[] cards)
    {
        GameSaveData saveData = new GameSaveData
        {
            score = _score,
            turnCount = _turnCount,
            matchesFound = _matchesFound,
            consecutiveMatches = _consecutiveMatches,
            currentLevelIndex = currentLevelIndex
        };

        if (cards != null)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] != null)
                {
                    saveData.cards.Add(new CardSaveData
                    {
                        cardIndex = i,
                        cardType = cards[i].CardType,
                        isMatched = cards[i].IsMatched,
                        isRevealed = cards[i].IsRevealed
                    });
                }
            }
        }

        return saveData;
    }
}
