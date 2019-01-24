using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class DataController : MonoBehaviour
{
    // currently will be only one data
    private MusicData[] allMusicData;
    private string gameDataFileName = "data.json";

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        //SceneManager.LoadScene("MainMenu");
        LoadGameData();
        SceneManager.LoadScene("MainMenu");
    }

    public MusicData GetCurrentMusicData()
    {
        return allMusicData[0];
    }

    private void LoadGameData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);
        if (File.Exists(filePath))
        {
            string dataJson = File.ReadAllText(filePath);

            GameData loadedData = JsonUtility.FromJson<GameData>(dataJson);

            allMusicData = loadedData.allMusicData;
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }
}
