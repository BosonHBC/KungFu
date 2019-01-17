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
        if (canRegister)
        {
            if (input.GetComponent<ArduinoInputScript>().buttons[upID] && !input.GetComponent<ArduinoInputScript>().buttons[downID])
            {
                if (currentSelectedID != 0)
                {
                    currentSelectedID--;
                    buttonObjects[currentSelectedID].Select();
                    canRegister = false;
                }
            }

            if (input.GetComponent<ArduinoInputScript>().buttons[downID] && !input.GetComponent<ArduinoInputScript>().buttons[upID])
            {
                if (currentSelectedID != buttonObjects.Length - 1)
                {
                    currentSelectedID++;
                    buttonObjects[currentSelectedID].Select();
                    canRegister = false;
                }
            }
        }
        else
        {
            if (!input.GetComponent<ArduinoInputScript>().buttons[upID] && !input.GetComponent<ArduinoInputScript>().buttons[downID])
            {
                canRegister = true;
            }
        }
    }
}
