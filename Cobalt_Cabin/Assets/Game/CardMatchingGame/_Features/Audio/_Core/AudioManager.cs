using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioConfig audioConfig;
    
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        _audioSource.playOnAwake = false;
    }

    public void PlayCardFlip()
    {
        if (audioConfig != null && audioConfig.cardFlipSound != null)
        {
            float volume = audioConfig.sfxVolume * audioConfig.cardFlipVolume;
            _audioSource.PlayOneShot(audioConfig.cardFlipSound, volume);
        }
    }

    public void PlayMatch()
    {
        if (audioConfig != null && audioConfig.matchSound != null)
        {
            float volume = audioConfig.sfxVolume * audioConfig.matchVolume;
            _audioSource.PlayOneShot(audioConfig.matchSound, volume);
        }
    }

    public void PlayMismatch()
    {
        if (audioConfig != null && audioConfig.mismatchSound != null)
        {
            float volume = audioConfig.sfxVolume * audioConfig.mismatchVolume;
            _audioSource.PlayOneShot(audioConfig.mismatchSound, volume);
        }
    }

    public void PlayGameOver()
    {
        if (audioConfig != null && audioConfig.gameOverSound != null)
        {
            float volume = audioConfig.sfxVolume * audioConfig.gameOverVolume;
            _audioSource.PlayOneShot(audioConfig.gameOverSound, volume);
        }
    }

    public void PlayButtonClick()
    {
        if (audioConfig != null && audioConfig.buttonClickSound != null)
        {
            float volume = audioConfig.sfxVolume * audioConfig.buttonClickVolume;
            _audioSource.PlayOneShot(audioConfig.buttonClickSound, volume);
        }
    }

    public void SetMasterVolume(float volume)
    {
        if (audioConfig != null)
        {
            audioConfig.sfxVolume = Mathf.Clamp01(volume);
        }
    }
}
