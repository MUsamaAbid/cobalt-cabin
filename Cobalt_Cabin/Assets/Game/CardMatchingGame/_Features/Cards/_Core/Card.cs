using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private SpriteRenderer FrontSpriteRenderer;
    [SerializeField] private SpriteRenderer BackSpriteRenderer;
    public CardType CardType { get; private set; }

    private bool _isActive = false;
    public bool IsRevealed => _isActive;

    public void Init(CardType cardType, Sprite FrontSprite, Sprite BackSprite)
    {
        CardType = cardType;
        FrontSpriteRenderer.sprite = FrontSprite;
        
        BackSpriteRenderer.sprite = BackSprite;
    }

    public void Reveal()
    {
        _isActive = true;
        //Do Revealing animation
        //On Action Back change it to
        // _isActive = false;
    }
}
