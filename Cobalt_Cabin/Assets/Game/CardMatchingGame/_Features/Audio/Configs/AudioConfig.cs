using UnityEngine;

[CreateAssetMenu(fileName = "Audio Config", menuName = "CardMatch/Audio Config", order = 10)]
public class AudioConfig : ScriptableObject
{
    [Header("Card Sounds")]
    [Tooltip("Sound played when a card is flipped")]
    public AudioClip cardFlipSound;

    [Header("Match Sounds")]
    [Tooltip("Sound played when cards match successfully")]
    public AudioClip matchSound;

    [Tooltip("Sound played when cards don't match")]
    public AudioClip mismatchSound;

    [Header("Game Sounds")]
    [Tooltip("Sound played when level is completed")]
    public AudioClip gameOverSound;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    [Range(0f, 1f)]
    public float cardFlipVolume = 0.7f;

    [Range(0f, 1f)]
    public float matchVolume = 0.8f;

    [Range(0f, 1f)]
    public float mismatchVolume = 0.6f;

    [Range(0f, 1f)]
    public float gameOverVolume = 1f;
}
