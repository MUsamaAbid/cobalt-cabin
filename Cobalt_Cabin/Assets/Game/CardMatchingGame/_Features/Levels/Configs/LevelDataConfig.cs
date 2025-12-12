using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Level Data", menuName = "CardMatch/Level Data", order = 0)]
public class LevelDataConfig : ScriptableObject
{
      public int columns = 1;
      public int rows = 1;
    
      [Tooltip("These are the only card types that will be involved in the distribution of cards")]
      public List<CardType> cardTypes;

      [Header("Retraining level")]
      [Tooltip("Works only if the 'check' is set to 'true'")]
      public bool restrainedLevel = false;
      public int maxTurnCount = 1;
}
