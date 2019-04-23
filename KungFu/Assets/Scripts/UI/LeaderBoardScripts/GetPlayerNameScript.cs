using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayerNameScript : MonoBehaviour {
    private int enterID = 0;
    private int backID = 4;
    private int prevID = 3;
    private int nextID = 5;
    private GameObject input;
    private int score;
    private string stringToEdit;
    private char[] chars;
    private int charCounter;
    private int currentChar;
    private char[] letters;
    private bool canRegister;
    private float inputDelay;
    private float delayTime;
    private string song;

    private void Awake()
    {
        inputDelay= 0;
        delayTime = 0.5f;
        currentChar = 0;
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
        ReceiveInput();
        UpdateText();

        if(inputDelay < delayTime)
        {
            inputDelay += Time.deltaTime;
        }
        else
        {
        inputDelay = 0;
        canRegister = true;
        }
    }

    private void ReceiveInput()
    {
        //Stops multiple input from registering
        if (canRegister)
        {
            //Select above button trigger with wrap around
            if (input.GetComponent<ArduinoInputScript>().buttons[prevID] && !input.GetComponent<ArduinoInputScript>().buttons[nextID])
            {
                canRegister = false;
                if (currentChar > 0)
                {
                    currentChar--;
                }
                else
                {
                    currentChar = letters.Length - 1;
                }

                chars[charCounter] = letters[currentChar];                
            }

            //Select below button trigger with wrap around
            if (input.GetComponent<ArduinoInputScript>().buttons[nextID] && !input.GetComponent<ArduinoInputScript>().buttons[prevID])
            {
                canRegister = false;
                if (currentChar < letters.Length - 1)
                {
                    currentChar++;
                }
                else
                {
                    currentChar = 0;
                }

                chars[charCounter] = letters[currentChar];
            }
           
            if (input.GetComponent<ArduinoInputScript>().buttons[enterID] && charCounter < 3)
            {
                canRegister = false;
                charCounter++;
                currentChar = 0;
            }

            if (input.GetComponent<ArduinoInputScript>().buttons[backID] && charCounter != 0)
            {
                canRegister = false;
                charCounter--;
                currentChar = 0;
            }

            if (input.GetComponent<ArduinoInputScript>().buttons[enterID] && charCounter >= 3)
            {
                HighScoreManager._instance.SaveHighScore(stringToEdit, score, song);
                //gameObject.GetComponent<HighScoreManager>().DisableText();
                canRegister = false;
                this.enabled = false;
            }
        }
    }

    public void ReceiveScore(int score, string song)
    {
        this.score = score;
        this.song = song;
    }

    private void UpdateText()
    {
        stringToEdit = "" + chars[0] + " - " + chars[1] + " - "  + chars[2];
        //gameObject.GetComponent<HighScoreManager>().SetText(stringToEdit);
    }

    private void CreateCharArray()
    {
        letters = new char[] {'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','!','@','#','$','&','*'};
    }
}
