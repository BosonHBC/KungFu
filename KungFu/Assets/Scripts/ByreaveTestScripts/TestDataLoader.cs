using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;

public class TestDataLoader : MonoBehaviour
{
    // currently will be only one data
    //private MusicData[] allMusicData;
    JSONNode musicData;
    private string gameDataFileName = "byreaveTest.json";

    private void Awake()
    {
        //just for testing, this is supposed to be in the loading scene
        LoadGameData();

    }
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

            //GameData loadedData = JsonUtility.FromJson<GameData>(dataJson);
            var loadedData = JSON.Parse(dataJson);
            musicData = loadedData["allMusicData"];
            Debug.Log(musicData[0]["name"]);
            //allMusicData = loadedData.allMusicData;
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }
    public JSONNode GetBeatDataByName(string name)
    {
        JSONNode retBeat = null;
        foreach (var music in musicData)
        {
            if(music.Value["name"] == name)
            {
                retBeat = music.Value["beatArray"];
                break;
            }
        }

        return retBeat;
    }
}
