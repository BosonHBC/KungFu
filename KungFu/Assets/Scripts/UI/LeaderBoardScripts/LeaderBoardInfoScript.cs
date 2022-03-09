using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardInfoScript : MonoBehaviour
{

    public int songID;
    public GameObject[] scoreTextBoxes;

    // Start is called before the first frame update
    void Awake()
    {
        UpdateScore();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            Debug.Log("Cleared song: " + songID);
            HighScoreManager._instance.ClearLeaderBoard(songID);
        }
    }

}
