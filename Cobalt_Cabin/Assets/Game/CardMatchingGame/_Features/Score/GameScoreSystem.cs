using System;
using UnityEngine;

public class GameScoreSystem
{
    private int _score = 0;
    private int _turnCount = 0;
    private int _consecutiveMatches = 0;
    private int _matchesFound = 0;
    private int _totalMatchesInLevel = 0;

    private const int BASE_MATCH_SCORE = 100;
    private const int COMBO_BONUS = 50;

    public int Score => _score;
    public int TurnCount => _turnCount;
    public int ConsecutiveMatches => _consecutiveMatches;
    public int MatchesFound => _matchesFound;
    public int TotalMatchesInLevel => _totalMatchesInLevel;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnTurnCountChanged;
    public event Action<int, int> OnMatchesChanged;

    public void SetTotalMatchesInLevel(int totalMatches)
    {
        _totalMatchesInLevel = totalMatches;
        OnMatchesChanged?.Invoke(_matchesFound, _totalMatchesInLevel);
    }

    public void RecordTurn()
    {
        _turnCount++;
        OnTurnCountChanged?.Invoke(_turnCount);
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
}
