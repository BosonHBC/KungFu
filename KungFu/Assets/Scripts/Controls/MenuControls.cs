using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MenuControls : MonoBehaviour
{
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
        input = GameObject.Find("UdinoManager");

        //The starting button should be set as the first gameobject in the array and should be highlighted in the editor as well
        currentSelectedID = 0;
    }

    // Update is called once per frame
    void Update()
    {
       if(input.GetComponent<ArduinoInputScript>().buttons[upID])
        {
            if(currentSelectedID != 0)
            {
                currentSelectedID--;
                buttonObjects[currentSelectedID].Select();
            }
        }
    }
}
