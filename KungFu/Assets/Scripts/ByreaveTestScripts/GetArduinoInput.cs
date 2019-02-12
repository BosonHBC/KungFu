using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using Uduino;

public class GetArduinoInput : MonoBehaviour
{
    private int NUMBUTTONS = 11;
    bool[] buttons;
    private string outputString;

    [SerializeField] bool bDebugPrintInput;

    SerialPort sp = new SerialPort("COM3", 9600);

    // Use this for initialization
    void Start()
    {
        //opens the port and sets the read timeout. The timeout should match the write timeout in the arduino sketch to remove data stream lag.
        sp.Open();
        sp.ReadTimeout = 5;
        buttons = new bool[NUMBUTTONS];
    }

    // Update is called once per frame
    void Update()
    {
        //check if port is open and read the string from arduino.
        if (sp.IsOpen)
        {
            try
            {
                outputString = sp.ReadLine();
            }
            catch (System.Exception)
            {
                //Debug.Log("Port Closed");
            }
        }
        //Transfers the info sent by arduino to the bool array of button inputs.
        stringToBoolArray(outputString);
    }

    //Saves button input
    void stringToBoolArray(string input)
    {
        //Arduino having issues not sending all 11 buttons sometimes(once every few seconds but sends info every 5 ms shouldnt have any issues in human timescales lol)
        if (input == null || input.Length < NUMBUTTONS)
        {
            input = "00000000000";
        }

        for (int i = 0; i < NUMBUTTONS; i++)
        {
            if (input.ToCharArray()[i] == '1')
            {
                buttons[i] = true;
                if (bDebugPrintInput)
                    Debug.Log("Pressing Button:" + i);
            }
            else
            {
                buttons[i] = false;
            }
        }
        if (bDebugPrintInput)
            Debug.Log(input + "\n");

        //for (int i = 0; i < buttons.Length; i++)
        //{
        //    if (buttons[i])
        //        Debug.Log("Button " + i + "is pressed");
        //}
        MyGameInstance.instance.SetArduinoInput(buttons);
    }
}
