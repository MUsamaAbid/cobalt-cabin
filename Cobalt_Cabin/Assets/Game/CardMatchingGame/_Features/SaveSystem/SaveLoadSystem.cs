using UnityEngine;

public class SaveLoadSystem
{
    private const string SAVE_KEY = "CardMatchGameSave";

    public void SaveGame(GameSaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData, true);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        
        Debug.Log($"[SaveLoadSystem] Game Saved! Level: {saveData.currentLevelIndex}, Score: {saveData.score}, Turns: {saveData.turnCount}, Matches: {saveData.matchesFound}/{saveData.cards?.Count ?? 0} cards");
    }

    public GameSaveData LoadGame()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
            
            Debug.Log($"[SaveLoadSystem] Game Loaded! Level: {saveData.currentLevelIndex}, Score: {saveData.score}, Turns: {saveData.turnCount}, Matches: {saveData.matchesFound}/{saveData.cards?.Count ?? 0} cards");
            return saveData;
        }
        
        Debug.Log("[SaveLoadSystem] No save data found.");
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
            Debug.Log("[SaveLoadSystem] Save data deleted.");
        }
        else
        {
            Debug.Log("[SaveLoadSystem] No save data to delete.");
        }
    }
}
