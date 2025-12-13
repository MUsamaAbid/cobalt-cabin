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

    public void Init(LevelDataConfig levelDataConfig, GameScoreSystem scoreSystem, GameUIController uiController = null)
    {
        ClearBoard();
        
        controller = new CardSystemController(cardSystemConfig, scoreSystem, cardHolder, this, uiController);
        controller.GenerateCards(levelDataConfig);
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
        Debug.Log("Game Completed! Congratulations!");
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

