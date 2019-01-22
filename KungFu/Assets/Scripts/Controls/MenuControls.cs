using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MenuControls : MonoBehaviour
{
    private bool canRegister;
    private GameObject input;
    private int currentSelectedID;
    public Button[] buttonObjects;
    public int enterID;
    public int backID;
    public int upID;
    public int downID;

    // Start is called before the first frame update
    void Start()
    {
        canRegister = true;
        input = GameObject.Find("UduinoManager");

        //The starting button should be set as the first gameobject in the array and should be highlighted in the editor as well
        currentSelectedID = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Stops multiple input from registering
        if (canRegister)
        {
            //Select above button trigger
            if (input.GetComponent<ArduinoInputScript>().buttons[upID] && !input.GetComponent<ArduinoInputScript>().buttons[downID])
            {
                if (currentSelectedID != 0)
                {
                    currentSelectedID--;
                    buttonObjects[currentSelectedID].Select();
                    canRegister = false;
                }
            }

            //Select below button trigger
            if (input.GetComponent<ArduinoInputScript>().buttons[downID] && !input.GetComponent<ArduinoInputScript>().buttons[upID])
            {
                if (currentSelectedID != buttonObjects.Length - 1)
                {
                    currentSelectedID++;
                    buttonObjects[currentSelectedID].Select();
                    canRegister = false;
                }
            }

            //Submit trigger
            if (input.GetComponent<ArduinoInputScript>().buttons[enterID]  && !input.GetComponent<ArduinoInputScript>().buttons[downID] && !input.GetComponent<ArduinoInputScript>().buttons[upID])
            {
                buttonObjects[currentSelectedID].onClick.Invoke();
            }

            //Close menu trigger
            if (input.GetComponent<ArduinoInputScript>().buttons[backID] && !input.GetComponent<ArduinoInputScript>().buttons[downID] && !input.GetComponent<ArduinoInputScript>().buttons[upID])
            {
                this.gameObject.SetActive(false);
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
