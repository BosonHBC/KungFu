using System.Collections;
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
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        if(canvas != null)
            canvas.gameObject.GetComponent<ScoreTextControl>().SetScore("Score: " + scores.ToString());
    }

    public void Miss(int number)
    {
        misses += number;
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        //if (canvas != null)
        //    canvas.gameObject.GetComponent<ScoreTextControl>().SetMiss("Miss: " + misses.ToString());
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
        SceneManager.LoadScene("ByreaveLoadingScene");
        Invoke("StartGame", 2.0f);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("ByreaveWhitebox");
    }
}
