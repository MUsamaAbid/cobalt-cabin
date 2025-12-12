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
    private void Start()
    {
        Init(levelDataConfig);
    }

    public void Init(LevelDataConfig levelDataConfig)
    {
        controller = new CardSystemController(cardSystemConfig, cardHolder);
        controller.GenerateCards(levelDataConfig);
    }
}
