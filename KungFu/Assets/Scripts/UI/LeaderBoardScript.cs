using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardScript : MonoBehaviour
{
    public GameObject[] scoreTextBoxes;
    public bool resetScores;

    // Start is called before the first frame update
    void Start()
    {
        if(resetScores)
        {
        HighScoreManager._instance.ClearLeaderBoard();
        }

        HighScoreManager._instance.SaveHighScore("      ", 0);
    }


    public void UpdateScore()
    {
        int i = 0;
        foreach (Scores highScore in HighScoreManager._instance.GetHighScore())
        {
            scoreTextBoxes[i].GetComponent<Text>().text = (i + 1) + ": " + highScore.name + " - " + highScore.score;
            i++;
        }
    }
}
