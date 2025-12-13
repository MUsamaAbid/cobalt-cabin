using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public int score;
    public int turnCount;
    public int matchesFound;
    public int consecutiveMatches;
    public string levelConfigPath;
    public List<CardSaveData> cards;

    public GameSaveData()
    {
        cards = new List<CardSaveData>();
    }
}

[Serializable]
public class CardSaveData
{
    public int cardIndex;
    public CardType cardType;
    public bool isMatched;
    public bool isRevealed;
}
