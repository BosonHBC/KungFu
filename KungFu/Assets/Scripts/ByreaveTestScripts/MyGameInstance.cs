using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyGameInstance : MonoBehaviour
{
    public static MyGameInstance instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    public Text ScoreText;
    public Text MissText;
    int scores = 0, misses = 0;
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
}
