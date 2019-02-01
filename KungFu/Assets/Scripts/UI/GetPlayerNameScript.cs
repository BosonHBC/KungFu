using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayerNameScript : MonoBehaviour {

    private string stringToEdit;

    public int enterID;
    public int prevID;
    public int nextID;

    private GameObject input;
    private int score;

    private void Update()
    {
        ReceiveInput();
    }

    private void OnEnable()
    {
        score = 0;
        stringToEdit = " HighScore!!! Please Enter Your Name";
    }

    private void ReceiveInput()
    {
        if (input.GetComponent<ArduinoInputScript>().buttons[enterID])
        {
            HighScoreManager._instance.SaveHighScore(stringToEdit, score);
            this.enabled = false;
        }
    }

    public void ReceiveScore(int score)
    {
        this.score = score;
    }

    void OnGUI()
    {
        stringToEdit = GUI.TextField(new Rect(Screen.width/2-365, Screen.height/2+165, 730, 50), stringToEdit, 50);
        GUI.skin.textField.fontSize = 40;
    }
}
