using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;

public class DataLoader : MonoBehaviour
{
    // currently will be only one data
    //private MusicData[] allMusicData;
    JSONNode musicData;
    JSONNode animationData;
    bool bLoaded;
    //new data structure
    #region
    //string animationInfoFilename = "AnimationData.json";
    string beatInfoFilename = "BeatData_new.json";
    string songInfoFilename = "NewMusicData.json";

    JSONNode allMusicData;
    Dictionary<int, AnimationInfo> AnimationData;
    Dictionary<int, BeatInfo> BeatData;

    void LoadDataToObjects()
    {
        BeatData = new Dictionary<int, BeatInfo>();
        //load beat data
        string filePath = Path.Combine(Application.streamingAssetsPath, beatInfoFilename);
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
        DontDestroyOnLoad(gameObject);
        if (!bLoaded)
        {
            bLoaded = true;
            LoadDataToObjects();
        }
    }

    public JSONNode GetAnimationData()
    {
        return animationData;
    }

    public JSONNode GetMusicData()
    {
        return allMusicData;
    }

    public JSONNode GetBeatArrayByName(string name)
    {
        foreach (var song in allMusicData.Values)
        {
            if (song["name"] == name)
            {
                return song["animArray"];
            }
        }
        return null;
    }

    public int GetBeatNumByName(string name)
    {
        foreach (var song in allMusicData.Values)
        {
            if (song["name"] == name)
            {
                return song["numOfAnim"];
            }
        }
        return 0;
    }
}
