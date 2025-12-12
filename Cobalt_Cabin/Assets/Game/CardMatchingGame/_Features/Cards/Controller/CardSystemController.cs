using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSystemController
{
    private CardSystemConfig _cardSystemConfig;
    private LevelDataConfig _levelDataConfig;
    private Transform _boardTransform;
    
    private Card[] _cardsOnBoard;
    public Card[] CardsOnBoard => _cardsOnBoard;

    public List<Card> ActiveCards { get; private set; } = new List<Card>();

    public CardSystemController(CardSystemConfig cardSystemConfig, Transform boardTransform = null)
    {
        _cardSystemConfig = cardSystemConfig;
        _boardTransform = boardTransform;
    }
    
    public void GenerateCards(LevelDataConfig levelData)
    {
        var _cardFactory = new CardFactory(_cardSystemConfig, _boardTransform);
        _cardsOnBoard = _cardFactory.CreateCards(levelData);

        SetCardPositions(_cardsOnBoard, levelData);
    }

    void SetCardPositions(Card[] cardsOnBoard, LevelDataConfig levelData)
    {
        if (cardsOnBoard == null || cardsOnBoard.Length == 0)
            return;

        int columns = levelData.columns;
        int rows = levelData.rows;

        float cardSpacing = 2.5f;
        float totalWidth = (columns - 1) * cardSpacing;
        float totalHeight = (rows - 1) * cardSpacing;

        Vector3 startPosition = new Vector3(-totalWidth / 2f, totalHeight / 2f, 0f);

        int cardIndex = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (cardIndex >= cardsOnBoard.Length)
                    break;

                Vector3 position = startPosition + new Vector3(col * cardSpacing, -row * cardSpacing, 0f);
                cardsOnBoard[cardIndex].transform.position = position;
                cardIndex++;
            }
        }
    }

    public void AddActiveCard(Card card)
    {
        ActiveCards.Add(card);
    }

    public void RemoveActiveCard(Card card)
    {
        ActiveCards.Remove(card);
    }
    public void ResetActiveCards(Card card)
    {
        ActiveCards.Clear();
    }
}
