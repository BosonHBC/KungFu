using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardInfoScript : MonoBehaviour
{

    public int songID;
    public GameObject[] scoreTextBoxes;
    public bool resetSongScores;

    // Start is called before the first frame update
    void Awake()
    {
        UpdateScore();

        if(resetSongScores)
        {
            HighScoreManager._instance.ClearLeaderBoard(songID);
        }
    }
    public void UpdateScore()
    {
        int i = 0;
        foreach (Scores highScore in HighScoreManager._instance.GetHighScore(songID))
        {
            scoreTextBoxes[i].GetComponent<Text>().text = highScore.name + " - " + highScore.score;
            i++;
        }
    }

}
