using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSystem : MonoBehaviour
{
    [SerializeField] CardSystemConfig cardSystemConfig;
    [SerializeField] Transform cardHolder;
    
    CardSystemController controller;

    [SerializeField] LevelDataConfig levelDataConfig;
    
    void Start()
    {
        Init(levelDataConfig);
    }

    public void Init(LevelDataConfig levelDataConfig)
    {
        controller = new CardSystemController(cardSystemConfig, cardHolder, this);
        controller.GenerateCards(levelDataConfig);
    }

    void OnDestroy()
    {
        controller?.Cleanup();
    }
}
