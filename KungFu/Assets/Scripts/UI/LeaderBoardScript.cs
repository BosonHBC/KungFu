using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardScript : MonoBehaviour
{
    public GameObject[] scoreTextBoxes;
    public GameObject songText;
    public bool resetScores;
    public string songToReset;
    public string[] songs;
    public int prevID;
    public int nextID;

    private int currSong;
    private bool canRegister;
    private GameObject input;
    private float inputDelay;
    private float delayTime;

    // Start is called before the first frame update
    void Start()
    {
        input = GameObject.Find("UduinoManager");
        inputDelay = 0;
        delayTime = 0.5f;
        currSong = 0;
        canRegister = true;

        if(resetScores)
        {
        HighScoreManager._instance.ClearLeaderBoard(songToReset);
        }

        //HighScoreManager._instance.SaveHighScore("      ", 0, songs[currSong]);
    }

    private void OnEnable()
    {
        songs = new string[] { "XiaoHong", "Test"};
    }

    // Update is called once per frame
    void Update()
    {
        ReceiveInput();

        if (inputDelay < delayTime)
        {
            inputDelay += Time.deltaTime;
        }
        else
        {
            inputDelay = 0;
            canRegister = true;
        }
    }

    void ReceiveInput()
    {
        if (canRegister)
        {
            
            if (input.GetComponent<ArduinoInputScript>().buttons[nextID])
            {
                if (currSong < songs.Length - 1)
                {
                    currSong++;
                }
                else
                {
                    currSong = 0;
                }

                UpdateScore();
                canRegister = false;
            }

            if (input.GetComponent<ArduinoInputScript>().buttons[prevID])
            {

                if (currSong > 0)
                {
                    currSong--;
                }
                else
                {
                    currSong = songs.Length - 1;
                }

                UpdateScore();
                canRegister = false;
            }
            
        }
    }

        public void UpdateScore()
    {
        songText.GetComponent<Text>().text = songs[currSong];
        int i = 0;
        foreach (Scores highScore in HighScoreManager._instance.GetHighScore(songs[currSong]))
        {
            scoreTextBoxes[i].GetComponent<Text>().text = (i + 1) + ": " + highScore.name + " - " + highScore.score;
            i++;
        }
    }
}
