using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSystem : MonoBehaviour
{
    [SerializeField] CardSystemConfig cardSystemConfig;
    [SerializeField] Transform cardHolder;
    
    CardSystemController controller;
    public CardSystemController CardController => controller;

    public event Action OnGameCompletedEvent;

    public void Init(LevelDataConfig levelDataConfig, GameScoreSystem scoreSystem, GameUIController uiController = null, AudioManager audioManager = null)
    {
        ClearBoard();
        
        controller = new CardSystemController(cardSystemConfig, scoreSystem, cardHolder, this, uiController, audioManager);
        controller.GenerateCards(levelDataConfig);
        controller.OnGameCompleted += OnGameCompleted;
    }

    public void InitFromSave(LevelDataConfig levelDataConfig, GameScoreSystem scoreSystem, List<CardSaveData> savedCards, GameUIController uiController = null, AudioManager audioManager = null)
    {
        ClearBoard();
        
        controller = new CardSystemController(cardSystemConfig, scoreSystem, cardHolder, this, uiController, audioManager);
        controller.GenerateCardsFromSave(levelDataConfig, savedCards);
        controller.OnGameCompleted += OnGameCompleted;
    }

    void ClearBoard()
    {
        if (controller != null)
        {
            controller.Cleanup();
            controller.OnGameCompleted -= OnGameCompleted;
        }

        if (cardHolder != null)
        {
            foreach (Transform child in cardHolder)
            {
                Destroy(child.gameObject);
            }
        }
    }

    void OnGameCompleted()
    {
        Debug.Log("[BoardSystem] Game Completed! Congratulations!");
        Debug.Log($"[BoardSystem] Invoking OnGameCompletedEvent. Subscribers: {(OnGameCompletedEvent != null ? OnGameCompletedEvent.GetInvocationList().Length : 0)}");
        OnGameCompletedEvent?.Invoke();
    }

    void OnDestroy()
    {
        if (controller != null)
        {
            controller.OnGameCompleted -= OnGameCompleted;
            controller.Cleanup();
        }
    }
}

