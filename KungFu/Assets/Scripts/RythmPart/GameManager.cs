using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private DataController _dataController;
    private MusicData currentMusicData;

    private bool bIsMusicActive;
    private float fCurrentTime;
    private BeatData[] beatData;
    private AudioClip musicClip;

    private int score;
    private int currentComb;
    private int highestComb;

    // Start is called before the first frame update
    void Start()
    {
        _dataController = FindObjectOfType<DataController>();
        currentMusicData = _dataController.GetCurrentMusicData();
        beatData = currentMusicData.beatArray;
        fCurrentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (bIsMusicActive)
        {
            fCurrentTime += Time.deltaTime;
            if(fCurrentTime > currentMusicData.lengthInSeconds)
            {
                // Game Over
                bIsMusicActive = false;
                
            }
        }
    }

    public void StartGame()
    {
        bIsMusicActive = true;
    }
}
