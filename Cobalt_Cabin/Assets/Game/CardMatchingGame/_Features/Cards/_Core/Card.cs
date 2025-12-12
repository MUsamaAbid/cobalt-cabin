using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Card : MonoBehaviour
{
    [SerializeField] private SpriteRenderer FrontSpriteRenderer;
    [SerializeField] private SpriteRenderer BackSpriteRenderer;
    
    [Header("Animation Settings")]
    [SerializeField] private float flipDuration = 0.3f;
    
    public CardType CardType { get; private set; }

    private bool _isRevealed = false;
    public bool IsRevealed => _isRevealed;
    
    private bool _isFlipping = false;
    private Coroutine _flipCoroutine;
    private BoxCollider2D _collider;

    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        if (_collider == null)
        {
            _collider = gameObject.AddComponent<BoxCollider2D>();
        }
        
        _collider.size = new Vector2(2f, 2.5f);
    }

    public void Init(CardType cardType, Sprite FrontSprite, Sprite BackSprite)
    {
        CardType = cardType;
        FrontSpriteRenderer.sprite = FrontSprite;
        BackSpriteRenderer.sprite = BackSprite;
        
        SetCardToBack();
    }

    void SetCardToBack()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        FrontSpriteRenderer.enabled = false;
        BackSpriteRenderer.enabled = true;
        _isRevealed = false;
    }

    void OnMouseDown()
    {
        if (!_isFlipping)
        {
            OnCardClicked();
        }
    }

    void OnCardClicked()
    {
        if (!_isRevealed)
        {
            Reveal();
        }
    }

    public void Reveal()
    {
        if (_isFlipping || _isRevealed)
            return;

        _isRevealed = true;
        
        if (_flipCoroutine != null)
        {
            StopCoroutine(_flipCoroutine);
        }
        
        _flipCoroutine = StartCoroutine(FlipCard(true));
    }

    public void Hide()
    {
        if (_isFlipping || !_isRevealed)
            return;

        _isRevealed = false;
        
        if (_flipCoroutine != null)
        {
            StopCoroutine(_flipCoroutine);
        }
        
        _flipCoroutine = StartCoroutine(FlipCard(false));
    }

    IEnumerator FlipCard(bool showFront)
    {
        _isFlipping = true;

        float elapsed = 0f;
        float startRotationY = transform.eulerAngles.y;
        float endRotationY = showFront ? 180f : 0f;

        while (elapsed < flipDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / flipDuration;
            
            float currentRotationY = Mathf.Lerp(startRotationY, endRotationY, progress);
            transform.rotation = Quaternion.Euler(0f, currentRotationY, 0f);

            if (progress >= 0.5f)
            {
                FrontSpriteRenderer.enabled = showFront;
                BackSpriteRenderer.enabled = !showFront;
            }

            yield return null;
        }

        transform.rotation = Quaternion.Euler(0f, endRotationY, 0f);
        FrontSpriteRenderer.enabled = showFront;
        BackSpriteRenderer.enabled = !showFront;

        _isFlipping = false;
    }
}
