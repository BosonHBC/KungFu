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


    //new data structure
    #region
    string animationInfoFilename = "AnimationData.json";
    string beatInfoFilename = "BeatData.json";
    string songInfoFilename = "NewMusicData.json";

    JSONNode allMusicData;
    Dictionary<int, AnimationInfo> AnimationData;
    Dictionary<int, BeatInfo> BeatData;

    void LoadDataToObjects()
    {
        AnimationData = new Dictionary<int, AnimationInfo>();
        BeatData = new Dictionary<int, BeatInfo>();

        //load animation data
        string filePath = Path.Combine(Application.streamingAssetsPath, animationInfoFilename);
        if (File.Exists(filePath))
        {
            string dataJson = File.ReadAllText(filePath);

            var loadedData = JSON.Parse(dataJson);
            foreach(var anim in loadedData["Animations"].Values)
            {
                AnimationData.Add(anim["AnimationID"], new AnimationInfo(anim));
            }
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
        //load beat data
        filePath = Path.Combine(Application.streamingAssetsPath, beatInfoFilename);
        if (File.Exists(filePath))
        {
            string dataJson = File.ReadAllText(filePath);

            var loadedData = JSON.Parse(dataJson);
            foreach (var beat in loadedData["BeatData"].Values)
            {
                BeatData.Add(beat["BeatID"], new BeatInfo(beat));
            }
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
        //load songs data
        filePath = Path.Combine(Application.streamingAssetsPath, songInfoFilename);
        if (File.Exists(filePath))
        {
            string dataJson = File.ReadAllText(filePath);

            var loadedData = JSON.Parse(dataJson);
            allMusicData = loadedData["allMusicData"];
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }

    public Dictionary<int, AnimationInfo> GetAnimationInfos()
    {
        return AnimationData;
    }
    public Dictionary<int, BeatInfo> GetBeatInfos()
    {
        return BeatData;
    }

    #endregion

    private void Awake()
    {
        //just for testing, this is supposed to be in the loading scene
        LoadGameData();
        LoadDataToObjects();
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

    public BeatTiming GetBeatAnimationDataByID(int ID)
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
            return new BeatTiming(retBeat[2].AsFloat, retBeat[3].AsFloat, retBeat[4].AsFloat, retBeat[5].AsFloat);
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

    public JSONNode GetAnimationArrayByName(string name)
    {
        foreach(var song in allMusicData.Values)
        {
            if(song["name"] == name)
            {
                return song["animArray"];
            }
        }
        return null;
    }
}
