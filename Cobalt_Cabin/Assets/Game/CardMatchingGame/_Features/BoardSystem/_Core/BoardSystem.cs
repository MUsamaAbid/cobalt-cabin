using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSystem : MonoBehaviour
{
    [SerializeField] CardSystemConfig cardSystemConfig;
    [SerializeField] Transform cardHolder;
    
    CardSystemController controller;

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

