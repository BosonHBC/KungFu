﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        if (instance != this || instance == null)
            instance = this;
    }

    // public
    public bool bIsGameOver;
    public float fReactTime;


    // private
    private DataController _dataController;
    private MusicData currentMusicData;
    [SerializeField]
    private GameObject hitObjePrefab;

    private bool bIsMusicActive;
    private float fCurrentTime;
    private int iCurrentBeat = -1;
    private bool[] UnoInput;
    private bool bInBeat;


    private BeatData[] beatData;
    private AudioClip musicClip;
    [SerializeField] private AudioSource audioSource;

    private int score;
    private int currentComb;
    private int highestComb;


    // Start is called before the first frame update
    void Start()
    {
        _dataController = FindObjectOfType<DataController>();
        currentMusicData = _dataController.GetCurrentMusicData();
        musicClip = Resources.Load<AudioClip>("BGM/" + currentMusicData.name);
        if (!musicClip)
        {
            Debug.LogError("No certain music is found!");
            return;
        }
        audioSource.clip = musicClip;

        beatData = currentMusicData.beatArray;
        fCurrentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (bIsMusicActive)
        {
            fCurrentTime += Time.deltaTime;

            CheckBeat();

            if (fCurrentTime > currentMusicData.lengthInSeconds)
            {
                // Game Over
                bIsMusicActive = false;
                bIsGameOver = true;
            }
        }
    }

    public void StartGame()
    {
        bIsMusicActive = true;
        audioSource.Play();
        Debug.Log("Game Start!");

        UnoInput = new bool[11];
    }

    // get the current time which beat it is in.
    void CheckBeat()
    {

        for (int i = 0; i < beatData.Length - 1; i++)
        {
            if (Mathf.Abs((beatData[i].timeToHit - 1) - fCurrentTime) <= 0.01f)
            {
                iCurrentBeat = i;
                int repeatHit = 0;

                Debug.Log("Start beat: " + iCurrentBeat);
                // go through the hit array
                for (int j = 0; j < currentMusicData.hitArray.Length; j++)
                {
                    // compare the beatID with the current beat id
                    if (currentMusicData.hitArray[j].beatID == iCurrentBeat)
                    {
                        // Instantiate hit object
                        GameObject go = Instantiate(hitObjePrefab, UIController.instance.transform.GetChild(2));
                        go.transform.localPosition += repeatHit * Vector3.up;
                        repeatHit++;
                        go.GetComponent<HitObjet>().SetButtonID(currentMusicData.hitArray[j].buttonID);
                    }

                }
                break;
            }
            else
            {
                iCurrentBeat = -1;
            }
                
        }



    }

    public void DebugInput(int _buttonID)
    {
        UnoInput[_buttonID] = true;

        StartCoroutine(falseButton(_buttonID));
    }
    IEnumerator falseButton(int _buttonID)
    {
        yield return new WaitForSeconds(0.1f);
        UnoInput[_buttonID] = false;
    }


    public void SetUnoInput(bool[] _input)
    {
        UnoInput = _input;
    }
    public bool GetUnoInput(int _buttonID)
    {
        return UnoInput[_buttonID];
    }
    public bool[] GetUnoInputs()
    {
        return UnoInput;
    }
}
