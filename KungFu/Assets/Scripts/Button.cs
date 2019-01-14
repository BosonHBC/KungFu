using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;
public class Button : MonoBehaviour {

    [SerializeField] int ReadPin;
    bool bBoardConnected = false;
	// Use this for initialization
	void Start () {
        UduinoManager.Instance.pinMode(ReadPin, PinMode.Input_pullup);
	}
	
	// Update is called once per frame
	void Update () {
        int buttonValue = bBoardConnected?UduinoManager.Instance.digitalRead(ReadPin):-1;

        if (buttonValue == 0)
        {
            Debug.Log("Down");
        }
        else if(buttonValue == 1)
        {
            Debug.Log("Up");
        }

    }

    public void SetConnection(bool _connected)
    {
        bBoardConnected = _connected;

    }
}
