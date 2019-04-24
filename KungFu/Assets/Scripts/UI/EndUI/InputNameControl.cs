using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputNameControl : MonoBehaviour
{
    private GameObject input;
    private int charCounter = 1;
    public bool bCanShoose;
    private int iMaxIndex = 3;
    private int iCurrentIndex;
    private int[] initials = { 0, 0, 0 };
    char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    private int enterID = 10;
    private int prevID = 0;
    private int nextID = 4;

    private bool canRegister;
    private bool canEnterName;
    private float inputDelay;
    private float delayTime;

    Text[] childs;

    private void Awake()
    {
        inputDelay = 0;
        delayTime = .75f;
        canRegister = true;
        canEnterName = true;
    }

        // Start is called before the first frame update
        void Start()
    {
        input = GameObject.Find("UduinoManager");
        childs = new Text[] {
            transform.GetChild(1).GetChild(2).GetComponent<Text >(),
            transform.GetChild(2).GetChild(2).GetComponent<Text >(),
            transform.GetChild(3).GetChild(2).GetComponent<Text >()
        };
    }

    // Update is called once per frame
    void Update()
    {
        DebugTest();
        ControllerInputs();

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
    private void DebugTest()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            NextCharacter();

        if (Input.GetKeyDown(KeyCode.UpArrow))
            PreviousCharacter();

        if (Input.GetKeyDown(KeyCode.RightArrow))
            NextIndex();

        if (Input.GetKeyDown(KeyCode.Return) && canEnterName)
        {
            string playerName = "" + alpha[initials[0]] + alpha[initials[1]] + alpha[initials[2]];

            if (playerName.Equals("ASS") || playerName.Equals("AZZ"))
            {
                playerName = "***";
            }

            HighScoreManager._instance.CheckIfHighScore(MyGameInstance.instance.SongIndex, playerName, EndUIController.instance.finalScore);
            charCounter = 1;
            canEnterName = false;
        }
    }

    public void NextIndex()
    {
        if (bCanShoose)
        {
            iCurrentIndex++;
            if (iCurrentIndex < iMaxIndex)
            {
                childs[iCurrentIndex - 1].transform.parent.GetChild(0).gameObject.SetActive(false);
                childs[iCurrentIndex - 1].transform.parent.GetChild(1).gameObject.SetActive(false);

                childs[iCurrentIndex].transform.parent.GetChild(0).gameObject.SetActive(true);
                childs[iCurrentIndex].transform.parent.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                bCanShoose = false;
                childs[iMaxIndex - 1].transform.parent.GetChild(0).gameObject.SetActive(false);
                childs[iMaxIndex - 1].transform.parent.GetChild(1).gameObject.SetActive(false);
            }
            charCounter++;
        }
       
    }

    public void NextCharacter()
    {
        if (bCanShoose && iCurrentIndex < iMaxIndex)
        {
            initials[iCurrentIndex]++;
            if (initials[iCurrentIndex] > 25)
                initials[iCurrentIndex] = 0;

            childs[iCurrentIndex].text = alpha[initials[iCurrentIndex]].ToString() + ".";
        }
    }
    public void PreviousCharacter()
    {
        if (bCanShoose && iCurrentIndex < iMaxIndex)
        {
            initials[iCurrentIndex]--;
            if (initials[iCurrentIndex] < 0)
                initials[iCurrentIndex] = 25;

            childs[iCurrentIndex].text = alpha[initials[iCurrentIndex]].ToString() + ".";
        }
    }

    public void ControllerInputs()
    {
        if(charCounter == 4)
        {
            canEnterName = true;
        }

        if (canRegister)
        {
            if (input.GetComponent<ArduinoInputScript>().buttons[nextID])
            {
                NextCharacter();
                canRegister = false;
            }

            if (input.GetComponent<ArduinoInputScript>().buttons[prevID])
            {
                PreviousCharacter();
                canRegister = false;
            }

            if (input.GetComponent<ArduinoInputScript>().buttons[enterID] && !canEnterName)
            {
                NextIndex();
                canRegister = false;
            }

            if (input.GetComponent<ArduinoInputScript>().buttons[enterID] && canEnterName)
            {
                string playerName = "" + alpha[initials[0]] + alpha[initials[1]] + alpha[initials[2]];

                if (playerName.Equals("ASS") || playerName.Equals("AZZ"))
                {
                    playerName = "***";
                }

                HighScoreManager._instance.CheckIfHighScore(MyGameInstance.instance.SongIndex, playerName, EndUIController.instance.finalScore);
                charCounter = 1;
                canRegister = false;
                canEnterName = false;
            }
        }
    }
}
