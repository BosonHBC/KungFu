using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataController : MonoBehaviour
{
    // currently will be only one data
    public MusicData[] allMusicData;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public MusicData GetCurrentMusicData()
    {
        return allMusicData[0];
    }

    void Update()
    {
        
    }
}
