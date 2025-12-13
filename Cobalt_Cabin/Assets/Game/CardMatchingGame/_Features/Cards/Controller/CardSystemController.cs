using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardSystemController
{
    private CardSystemConfig _cardSystemConfig;
    private LevelDataConfig _levelDataConfig;
    private Transform _boardTransform;
    private MonoBehaviour _coroutineRunner;
    private GameScoreSystem _scoreSystem;
    private GameUIController _uiController;
    private AudioManager _audioManager;
    
    private Card[] _cardsOnBoard;
    public Card[] CardsOnBoard => _cardsOnBoard;
    public LevelDataConfig CurrentLevelData => _levelDataConfig;

    public List<Card> ActiveCards { get; private set; } = new List<Card>();

    private const int MAX_ACTIVE_CARDS = 2;
    private bool _isCheckingMatch = false;
    private int _matchedPairs = 0;

    public event Action OnGameCompleted;

    public CardSystemController(CardSystemConfig cardSystemConfig, GameScoreSystem scoreSystem, Transform boardTransform = null, MonoBehaviour coroutineRunner = null, GameUIController uiController = null, AudioManager audioManager = null)
    {
        _cardSystemConfig = cardSystemConfig;
        _boardTransform = boardTransform;
        _coroutineRunner = coroutineRunner;
        _scoreSystem = scoreSystem;
        _uiController = uiController;
        _audioManager = audioManager;
    }
    
    public void GenerateCards(LevelDataConfig levelData)
    {
        _levelDataConfig = levelData;
        var _cardFactory = new CardFactory(_cardSystemConfig, _boardTransform);
        _cardsOnBoard = _cardFactory.CreateCards(levelData);

        int totalPairs = _cardsOnBoard.Length / 2;
        _scoreSystem.SetTotalMatchesInLevel(totalPairs);

        SetCardPositions(_cardsOnBoard, levelData);
        SubscribeToCards(_cardsOnBoard);
    }

    public void GenerateCardsFromSave(LevelDataConfig levelData, List<CardSaveData> savedCards)
    {
        _levelDataConfig = levelData;
        var _cardFactory = new CardFactory(_cardSystemConfig, _boardTransform);
        _cardsOnBoard = _cardFactory.CreateCardsFromSave(savedCards);

        int totalPairs = _cardsOnBoard.Length / 2;
        _scoreSystem.SetTotalMatchesInLevel(totalPairs);

        SetCardPositions(_cardsOnBoard, levelData);
        SubscribeToCards(_cardsOnBoard);
    }

    public void RestoreCardStates(List<CardSaveData> cardSaveDataList)
    {
        if (_cardsOnBoard == null || cardSaveDataList == null)
            return;

        bool wasCheckingMatch = _isCheckingMatch;
        _isCheckingMatch = true;

        int matchedCardsCount = 0;

        foreach (CardSaveData cardSaveData in cardSaveDataList)
        {
            if (cardSaveData.cardIndex >= 0 && cardSaveData.cardIndex < _cardsOnBoard.Length)
            {
                Card card = _cardsOnBoard[cardSaveData.cardIndex];
                
                if (cardSaveData.isMatched)
                {
                    card.RevealImmediate();
                    card.SetMatched();
                    matchedCardsCount++;
                }
                else if (cardSaveData.isRevealed)
                {
                    card.RevealImmediate();
                }
            }
        }

        _matchedPairs = matchedCardsCount / 2;

        _isCheckingMatch = wasCheckingMatch;

        if (_uiController != null)
        {
            _uiController.UpdateComboDisplay(_scoreSystem.ConsecutiveMatches);
        }

        Debug.Log($"RestoreCardStates complete: {_matchedPairs}/{_cardsOnBoard.Length / 2} pairs matched (from {matchedCardsCount} matched cards)");

        if (_matchedPairs == _cardsOnBoard.Length / 2)
        {
            Debug.Log($"Game was already completed when loaded! Triggering completion event. ({_matchedPairs} == {_cardsOnBoard.Length / 2})");
            OnGameCompleted?.Invoke();
        }
    }

    void SubscribeToCards(Card[] cards)
    {
        foreach (Card card in cards)
        {
            card.OnCardRevealed += OnCardRevealed;
            card.OnCardFlippedByPlayer += OnCardFlippedByPlayer;
        }
    }

    void OnCardFlippedByPlayer()
    {
        if (_audioManager != null)
        {
            _audioManager.PlayCardFlip();
        }
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

    void OnCardRevealed(Card card)
    {
        if (_isCheckingMatch)
            return;

        if (ActiveCards.Contains(card))
            return;

        if (ActiveCards.Count >= MAX_ACTIVE_CARDS)
            return;

        ActiveCards.Add(card);

        if (ActiveCards.Count == MAX_ACTIVE_CARDS)
        {
            _isCheckingMatch = true;
            _scoreSystem.RecordTurn();
            DisableAllCardInput();
            _coroutineRunner.StartCoroutine(CheckMatch());
        }
    }

    void DisableAllCardInput()
    {
        if (_cardsOnBoard == null)
            return;

        foreach (Card card in _cardsOnBoard)
        {
            if (card != null && !card.IsMatched)
            {
                card.SetInputEnabled(false);
            }
        }
    }

    void EnableUnmatchedCardInput()
    {
        if (_cardsOnBoard == null)
            return;

        foreach (Card card in _cardsOnBoard)
        {
            if (card != null && !card.IsMatched)
            {
                card.SetInputEnabled(true);
            }
        }
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.5f);

        Card firstCard = ActiveCards[0];
        Card secondCard = ActiveCards[1];

        if (firstCard.CardType == secondCard.CardType)
        {
            firstCard.SetMatched();
            secondCard.SetMatched();
            _matchedPairs++;

            _scoreSystem.RecordMatch();
            
            if (_audioManager != null)
            {
                _audioManager.PlayMatch();
            }
            
            if (_uiController != null)
            {
                _uiController.UpdateComboDisplay(_scoreSystem.ConsecutiveMatches);
            }

            Debug.Log($"Match! Type: {firstCard.CardType}. Total matches: {_matchedPairs}/{_cardsOnBoard.Length / 2}");

            if (_matchedPairs == _cardsOnBoard.Length / 2)
            {
                Debug.Log($"All pairs matched! Game completed! ({_matchedPairs} == {_cardsOnBoard.Length / 2})");
                
                if (_audioManager != null)
                {
                    _audioManager.PlayGameOver();
                }
                
                OnGameCompleted?.Invoke();
            }
            else
            {
                Debug.Log($"Not complete yet: {_matchedPairs} != {_cardsOnBoard.Length / 2}");
            }
        }
        else
        {
            firstCard.Hide();
            secondCard.Hide();
            _scoreSystem.RecordMismatch();
            
            if (_audioManager != null)
            {
                _audioManager.PlayMismatch();
            }
            
            if (_uiController != null)
            {
                _uiController.UpdateComboDisplay(0);
            }
            
            Debug.Log($"No match. {firstCard.CardType} != {secondCard.CardType}");
        }

        ActiveCards.Clear();
        _isCheckingMatch = false;
        EnableUnmatchedCardInput();
    }

    public void Cleanup()
    {
        if (_cardsOnBoard != null)
        {
            foreach (Card card in _cardsOnBoard)
            {
                if (card != null)
                {
                    card.OnCardRevealed -= OnCardRevealed;
                    card.OnCardFlippedByPlayer -= OnCardFlippedByPlayer;
                }
            }
        }
    }
}

