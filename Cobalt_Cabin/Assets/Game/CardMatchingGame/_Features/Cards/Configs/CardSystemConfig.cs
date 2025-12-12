using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card System Config", menuName = "CardMatch/Card System Config", order = 0)]
public class CardSystemConfig : ScriptableObject
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] List<CardData> cards;

    public GameObject CardPrefab => cardPrefab;
    public List<CardData> Cards => cards;
}
