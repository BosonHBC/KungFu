using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using Uduino;

public class ArduinoInputScript : MonoBehaviour
{
    public static ArduinoInputScript instance;
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    internal bool[] buttons = new bool[11];
    private int NUMBUTTONS = 11;
    private string outputString = "00000000000";

    [SerializeField] bool bDebugPrintInput;

    SerialPort com7 = new SerialPort("COM7", 9600);

    // Use this for initialization
    void Start()
    {
        //opens the port and sets the read timeout. The timeout should match the write timeout in the arduino sketch to remove data stream lag.
        com7.Open();
        com7.ReadTimeout = 10;
    }

    // Update is called once per frame
    void Update()
    {
            //check if port is open and read the string from arduino.
            if (com7.IsOpen)
            {
                try
                {
                    outputString = com7.ReadLine();
                }
                catch (System.Exception)
                {
                    //Debug.Log("Port Closed");
                }
            }

        //Transfers the info sent by arduinos to the bool array of button inputs.
        stringToBoolArray(outputString);
    }

    //Saves button input
    void stringToBoolArray(string input)
    {
        //Arduino having issues not sending all 11 buttons sometimes(once every few seconds but sends info every 5 ms shouldnt have any issues in human timescales lol)
        if (input.Length < NUMBUTTONS)
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
        {
            Debug.Log(input + "\n");

        }
        else
        {
            //Saves input to GameInstance
            if (GameObject.Find("MyGameInstance") != null)
            {
                MyGameInstance.instance.SetArduinoInput(buttons);
            }      
            //only for use with original prototype
            else if (GameObject.Find("GameManager") != null)
            {
               GameManager.instance.SetUnoInput(buttons);
            }
        }


    }
    private void OnDisable()
    {
        com7.Close();
    }
}
