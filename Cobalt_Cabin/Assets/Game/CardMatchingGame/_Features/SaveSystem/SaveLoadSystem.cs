using UnityEngine;

public class SaveLoadSystem
{
    private const string SAVE_KEY = "CardMatchGameSave";

    public void SaveGame(GameSaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData, true);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        
        Debug.Log($"Game Saved! Score: {saveData.score}, Turns: {saveData.turnCount}, Matches: {saveData.matchesFound}");
    }

    public GameSaveData LoadGame()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
            
            Debug.Log($"Game Loaded! Score: {saveData.score}, Turns: {saveData.turnCount}, Matches: {saveData.matchesFound}");
            return saveData;
        }
        
        Debug.Log("No save data found.");
        return null;
    }

    public bool HasSaveData()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    public void DeleteSave()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
            Debug.Log("Save data deleted.");
        }
    }
}
