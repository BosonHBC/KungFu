using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MenuControls : MonoBehaviour
{
    private AudioSource source;
    private bool canRegister;
    private GameObject input;
    private int currentSelectedID;


    public Button[] buttonObjects;
    public GameObject creditsScreen;
    public GameObject titleScreen;
    public int enterID;
    public int backID;
    public int upID;
    public int downID;
    public AudioClip gong;
    public AudioClip[] whoosh;

    // Start is called before the first frame update
    void Start()
    {
        creditsScreen.SetActive(false);
        canRegister = true;
        input = GameObject.Find("UduinoManager");
        source = gameObject.GetComponent<AudioSource>();
        //The starting button should be set as the first gameobject in the array and should be highlighted in the editor as well
        currentSelectedID = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Stops multiple input from registering
        if (canRegister)
        {
            //Select above button trigger with wrap around
            if (input.GetComponent<ArduinoInputScript>().buttons[upID] && !input.GetComponent<ArduinoInputScript>().buttons[downID])
            {
                source.PlayOneShot(whoosh[Random.Range(0,whoosh.Length)]);
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
            if (input.GetComponent<ArduinoInputScript>().buttons[enterID]  && !input.GetComponent<ArduinoInputScript>().buttons[downID] && !input.GetComponent<ArduinoInputScript>().buttons[upID])
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

    public void Quit()
    {
        Application.Quit();
    }
}
