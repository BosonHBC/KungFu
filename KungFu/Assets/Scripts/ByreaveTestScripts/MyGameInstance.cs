﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum HitResult
{
    Perfect,
    Good,
    Miss
}
public class MyGameInstance : MonoBehaviour
{
    public static MyGameInstance instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    public Text ScoreText;
    public Text MissText;
    public GameObject ResultImageShow;
    int scores = 0, misses = 0;
    bool [] buttonInput;
    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

    }

    public void Score()
    {
        scores++;
        ScoreText.text = "Score: " + scores.ToString();
    }

    public void Miss(int number)
    {
        misses += number;
        MissText.text = "Miss: " + misses.ToString();
    }

    public void ShowResultAt(Transform locTrans, HitResult hitResult)
    {
        GameObject ri = Instantiate(ResultImageShow, locTrans.position, Quaternion.identity);
        ri.GetComponent<ResultImageControl>().ShowResult(hitResult);
    }

    public void SetArduinoInput(bool[] arduinoInput)
    {
        buttonInput = arduinoInput;
    }
    public bool[] GetArduinoInput()
    {
        return buttonInput;
    }

    public void RestartGame()
    {
        scores = 0;
        misses = 0;
        MissText.text = "Miss: " + misses.ToString();
        ScoreText.text = "Score: " + scores.ToString();
        SceneManager.LoadScene("ByreaveLoadingScene");
        Invoke("StartGame", 2.0f);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("ByreaveWhitebox");
    }
}
