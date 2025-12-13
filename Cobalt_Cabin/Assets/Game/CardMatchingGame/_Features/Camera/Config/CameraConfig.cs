using UnityEngine;

[CreateAssetMenu(fileName = "Camera Config", menuName = "CardMatch/Camera Config", order = 11)]
public class CameraConfig : ScriptableObject
{
    [Header("Padding/Offset")]
    [Tooltip("Extra space around the grid (in world units)")]
    public float topPadding = 1f;
    public float bottomPadding = 1f;
    public float leftPadding = 1f;
    public float rightPadding = 1f;

    [Header("Card Dimensions")]
    [Tooltip("Width of a single card in world units")]
    public float cardWidth = 2f;
    
    [Tooltip("Height of a single card in world units")]
    public float cardHeight = 2.5f;

    [Header("Grid Spacing")]
    [Tooltip("Spacing between cards (should match CardSystemController spacing)")]
    public float cardSpacing = 2.5f;
}
