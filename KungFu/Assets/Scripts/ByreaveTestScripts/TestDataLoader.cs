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
    JSONNode animationData;
    private string gameDataFileName = "AnimationTest.json";
    string animTimingFileName = "AnimationTiming.json";

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

        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }

        filePath = Path.Combine(Application.streamingAssetsPath, animTimingFileName);
        if (File.Exists(filePath))
        {
            string dataJson = File.ReadAllText(filePath);

            //GameData loadedData = JsonUtility.FromJson<GameData>(dataJson);
            var loadedData = JSON.Parse(dataJson);
            animationData = loadedData["Animations"];
        }
        else
        {
            Debug.LogError("Cannot load animation data!");
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

    public JSONNode GetAnimationData()
    {
        return animationData;
    }

    public BeatAnimation GetBeatAnimationDataByID(int ID)
    {
        JSONNode retBeat = null;
        foreach (var anim in animationData)
        {
            if (anim.Value["AnimationID"].AsInt == ID)
            {
                retBeat = anim.Value;
                break;
            }
        }
        if(retBeat != null)
            return new BeatAnimation(retBeat[0].AsInt, retBeat[1], retBeat[2].AsFloat, retBeat[3].AsFloat, retBeat[4].AsFloat, retBeat[5].AsFloat);
        else
        {
            Debug.Log("No Animation");
            return null;
        }
    }
    public JSONNode GetMusicData()
    {
        return musicData;
    }
}
