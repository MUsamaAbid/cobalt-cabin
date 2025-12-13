using UnityEngine;

public class SaveDebugHelper : MonoBehaviour
{
    [Header("Debug Controls")]
    [Tooltip("Press this key to delete all saves and restart")]
    public KeyCode deleteAllSavesKey = KeyCode.F9;
    
    [Tooltip("Press this key to delete current game save only")]
    public KeyCode deleteGameSaveKey = KeyCode.F8;

    void Update()
    {
        if (Input.GetKeyDown(deleteAllSavesKey))
        {
            DeleteAllSaves();
        }

        if (Input.GetKeyDown(deleteGameSaveKey))
        {
            DeleteGameSave();
        }
    }

    void DeleteAllSaves()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[SaveDebugHelper] All PlayerPrefs deleted! Reloading scene...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    void DeleteGameSave()
    {
        SaveLoadSystem saveSystem = new SaveLoadSystem();
        saveSystem.DeleteSave();
        Debug.Log("[SaveDebugHelper] Game save deleted! Reloading scene...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 14;
        style.normal.textColor = Color.yellow;
        
        GUI.Label(new Rect(10, 10, 400, 60), 
            $"[Debug] Press {deleteAllSavesKey} to delete ALL saves\n" +
            $"[Debug] Press {deleteGameSaveKey} to delete game save only", 
            style);
    }
}
