﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MenuControls : MonoBehaviour
{
    private AudioSource source;
    private bool canRegister;
    private GameObject input;
    private int currentSelectedID;
    private Vector3 startButtonPos;


    public Button[] buttonObjects;
    public GameObject creditsScreen;
    public GameObject titleScreen;
    public int enterID;
    public int backID;
    public int upID;
    public int downID;
    public AudioClip gong;
    public AudioClip[] whoosh;
    public float buttonSize;
    public float maxButtonSize;
    public float expandRate;

    // Start is called before the first frame update
    void Start()
    {
        creditsScreen.SetActive(false);
        canRegister = true;
        input = GameObject.Find("UduinoManager");
        source = gameObject.GetComponent<AudioSource>();
        //The starting button should be set as the first gameobject in the array and should be highlighted in the editor as well
        currentSelectedID = 0;
        startButtonPos = buttonObjects[currentSelectedID].image.rectTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ReceiveInput();
        GrowButton();
        MakeSpace();
    }

    private void GrowButton()
    {
        if (buttonObjects[currentSelectedID].image.rectTransform.localScale.x < maxButtonSize)
        {
            buttonObjects[currentSelectedID].image.rectTransform.localScale += new Vector3(expandRate, expandRate, 1f);
        }
    }

    private void MakeSpace()
    {
        for (int i = 0; i < buttonObjects.Length; i++)
        {
            if (currentSelectedID != i) {
                Vector3 pos = buttonObjects[i].image.rectTransform.position;

                if (buttonObjects[i].image.rectTransform.position.x < buttonObjects[currentSelectedID].image.rectTransform.position.x)
                {
                    pos = Vector3.MoveTowards(pos, new Vector3(startButtonPos.x - (100 * i + 50), startButtonPos.y, 1f), 5f);
                }
                else
                {
                    pos = Vector3.MoveTowards(pos, new Vector3(startButtonPos.x + (100 * i + 50), startButtonPos.y, 1f), 5f);
                }

                buttonObjects[i].image.rectTransform.position = pos;
            }
        }
    }

    private void PutBack()
    {
        for (int i = 0; i < buttonObjects.Length; i++)
        {
            buttonObjects[i].image.rectTransform.position = new Vector3(startButtonPos.x + (100*i), startButtonPos.y, 0f);
        }
    }

    private void ShrinkButton()
    {
        buttonObjects[currentSelectedID].image.rectTransform.localScale = new Vector3(buttonSize, buttonSize, 0f);
    }

    public void DisplayCredits()
    {
        creditsScreen.SetActive(true);
        titleScreen.SetActive(false);
    }

    public void DisplayScore()
    {
        //To Do
        //titleScreen.SetActive(false);
    }

    //Quit with delay
    public void Quit()
    {
        Invoke("ShutDown",2.0f);
    }
    private void ShutDown()
    {
        Application.Quit();
    }

    private void ReceiveInput()
        {
        //Stops multiple input from registering
        if (canRegister)
        {
            //Select above button trigger with wrap around
            if (input.GetComponent<ArduinoInputScript>().buttons[upID] && !input.GetComponent<ArduinoInputScript>().buttons[downID])
            {
                source.PlayOneShot(whoosh[Random.Range(0, whoosh.Length)]);

                ShrinkButton();
                PutBack();

                if (currentSelectedID != 0)
                {
                    currentSelectedID--;
                }
                else
                {
                    currentSelectedID = buttonObjects.Length - 1;
                }
                buttonObjects[currentSelectedID].Select();
                canRegister = false;
            }

            //Select below button trigger with wrap around
            if (input.GetComponent<ArduinoInputScript>().buttons[downID] && !input.GetComponent<ArduinoInputScript>().buttons[upID])
            {
                source.PlayOneShot(whoosh[Random.Range(0, whoosh.Length)]);

                ShrinkButton();
                PutBack();

                if (currentSelectedID != buttonObjects.Length - 1)
                {
                    currentSelectedID++;
                }
                else
                {
                    currentSelectedID = 0;
                }
                buttonObjects[currentSelectedID].Select();
                canRegister = false;
            }

            //Submit trigger
            if (input.GetComponent<ArduinoInputScript>().buttons[enterID] && !input.GetComponent<ArduinoInputScript>().buttons[downID] && !input.GetComponent<ArduinoInputScript>().buttons[upID])
            {
                source.PlayOneShot(gong);
                buttonObjects[currentSelectedID].onClick.Invoke();
            }

            //Close menu trigger
            if (input.GetComponent<ArduinoInputScript>().buttons[backID] && !input.GetComponent<ArduinoInputScript>().buttons[downID] && !input.GetComponent<ArduinoInputScript>().buttons[upID] && !titleScreen.activeSelf)
            {
                source.PlayOneShot(gong);
                creditsScreen.SetActive(false);
                titleScreen.SetActive(true);
            }
        }
        else
        {
            //Allows input to register after no buttons are pressed down.
            if (!input.GetComponent<ArduinoInputScript>().buttons[upID] && !input.GetComponent<ArduinoInputScript>().buttons[downID])
            {
                canRegister = true;
            }
        }
    }
}
