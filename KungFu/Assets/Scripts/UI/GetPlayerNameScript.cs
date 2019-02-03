using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayerNameScript : MonoBehaviour {
    public int enterID;
    public int backID;
    public int prevID;
    public int nextID;

    private GameObject input;
    private int score;
    private string stringToEdit;
    private char[] chars;
    private int charCounter;
    private int currentChar;
    private char[] letters;
    private bool canRegister;

    private void Awake()
    {
        charCounter = 0;
        input = GameObject.Find("UduinoManager");
        stringToEdit = "";
        score = 0;
        chars = new char[3] {' ',' ',' '};
        canRegister = true;
        CreateCharArray();
    }

    private void Update()
    {
        canRegister = true;
        UpdateText();
        ReceiveInput();
    }

    private void ReceiveInput()
    {
        //Stops multiple input from registering
        if (canRegister)
        {
            //Select above button trigger with wrap around
            if (input.GetComponent<ArduinoInputScript>().buttons[prevID] && !input.GetComponent<ArduinoInputScript>().buttons[nextID])
            {
                if (charCounter != 0)
                {
                    currentChar--;
                }
                else
                {
                    currentChar = letters.Length - 1;
                }
                chars[charCounter] = letters[currentChar];
                charCounter++;
                canRegister = false;
            }

            //Select below button trigger with wrap around
            if (input.GetComponent<ArduinoInputScript>().buttons[nextID] && !input.GetComponent<ArduinoInputScript>().buttons[prevID])
            {
                if (charCounter != letters.Length - 1)
                {
                    currentChar++;
                }
                else
                {
                    currentChar = 0;
                }
                chars[charCounter] = letters[currentChar];
                charCounter++;
                canRegister = false;
            }

            if (input.GetComponent<ArduinoInputScript>().buttons[enterID] && charCounter == 3)
            {
                HighScoreManager._instance.SaveHighScore(stringToEdit, score);
                gameObject.GetComponent<HighScoreManager>().DisableText();
                canRegister = false;
                this.enabled = false;
            }

            if (input.GetComponent<ArduinoInputScript>().buttons[backID] && charCounter != 0)
            {
                charCounter--;
                currentChar = 0;
            }
        }
    }

    public void ReceiveScore(int score)
    {
        this.score = score;
    }

    private void UpdateText()
    {
        stringToEdit = "" + chars[0] + " - " + chars[1] + " - "  + chars[2];
        gameObject.GetComponent<HighScoreManager>().SetText(stringToEdit);
    }

    private void CreateCharArray()
    {
        letters = new char[] {'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','!','@','#','$','&','*'};
    }
}
