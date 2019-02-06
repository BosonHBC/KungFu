using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TestDataLoader : MonoBehaviour
{
    // currently will be only one data
    private MusicData[] allMusicData;
    private string gameDataFileName = "byreaveTest.json";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
