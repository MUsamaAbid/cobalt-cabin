using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    private Button _button;
    private static AudioManager _audioManager;

    void Awake()
    {
        _button = GetComponent<Button>();
        
        if (_audioManager == null)
        {
            _audioManager = FindObjectOfType<AudioManager>();
        }

        if (_button != null)
        {
            _button.onClick.AddListener(PlayClickSound);
        }
    }

    void PlayClickSound()
    {
        if (_audioManager != null)
        {
            _audioManager.PlayButtonClick();
        }
    }

    void OnDestroy()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(PlayClickSound);
        }
    }
}
