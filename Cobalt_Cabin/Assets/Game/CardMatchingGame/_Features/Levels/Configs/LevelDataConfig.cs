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
}
