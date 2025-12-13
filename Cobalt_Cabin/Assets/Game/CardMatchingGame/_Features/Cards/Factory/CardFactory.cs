using System.Collections.Generic;
using UnityEngine;

public class CardFactory
{
    CardSystemConfig _cardSystemConfig;
    Transform _parentTransform;

    public CardFactory(CardSystemConfig cardSystem, Transform parentTransform = null)
    {
        _cardSystemConfig = cardSystem;
        _parentTransform = parentTransform;
    }

    public Card[] CreateCards(LevelDataConfig levelData)
    {
        int totalCards = levelData.columns * levelData.rows;
        
        if (totalCards % 2 != 0)
        {
            Debug.LogError($"Total cards must be even for matching pairs. Current: {totalCards}");
            return new Card[0];
        }

        int pairsNeeded = totalCards / 2;
        List<CardType> availableTypes = levelData.cardTypes;

        if (availableTypes.Count == 0)
        {
            Debug.LogError("No card types available in level data");
            return new Card[0];
        }

        List<Card> cards = new List<Card>();
        List<CardType> cardTypesToSpawn = new List<CardType>();

        for (int i = 0; i < pairsNeeded; i++)
        {
            CardType selectedType = availableTypes[i % availableTypes.Count];
            cardTypesToSpawn.Add(selectedType);
            cardTypesToSpawn.Add(selectedType);
        }

        ShuffleList(cardTypesToSpawn);

        foreach (CardType cardType in cardTypesToSpawn)
        {
            GameObject cardObject = Object.Instantiate(_cardSystemConfig.CardPrefab, _parentTransform);
            Card card = cardObject.GetComponent<Card>();
            
            if (card != null)
            {
                CardData cardData = GetCardDataByType(cardType);
                if (cardData != null)
                {
                    card.Init(cardType, cardData.Front, cardData.Back);
                    cards.Add(card);
                }
                else
                {
                    Debug.LogError($"No CardData found for CardType: {cardType}");
                    Object.Destroy(cardObject);
                }
            }
            else
            {
                Debug.LogError("Card prefab does not have a Card component!");
                Object.Destroy(cardObject);
            }
        }

        return cards.ToArray();
    }

    public Card[] CreateCardsFromSave(List<CardSaveData> savedCards)
    {
        if (savedCards == null || savedCards.Count == 0)
        {
            Debug.LogError("No saved card data provided");
            return new Card[0];
        }

        List<Card> cards = new List<Card>();

        foreach (CardSaveData savedCard in savedCards)
        {
            GameObject cardObject = Object.Instantiate(_cardSystemConfig.CardPrefab, _parentTransform);
            Card card = cardObject.GetComponent<Card>();
            
            if (card != null)
            {
                CardData cardData = GetCardDataByType(savedCard.cardType);
                if (cardData != null)
                {
                    card.Init(savedCard.cardType, cardData.Front, cardData.Back);
                    cards.Add(card);
                }
                else
                {
                    Debug.LogError($"No CardData found for saved CardType: {savedCard.cardType}");
                    Object.Destroy(cardObject);
                }
            }
            else
            {
                Debug.LogError("Card prefab does not have a Card component!");
                Object.Destroy(cardObject);
            }
        }

        return cards.ToArray();
    }

    CardData GetCardDataByType(CardType cardType)
    {
        foreach (CardData cardData in _cardSystemConfig.Cards)
        {
            if (cardData.Type == cardType)
            {
                return cardData;
            }
        }
        return null;
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
