using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using Uduino;

public class Button : MonoBehaviour {

    internal bool button01;
    internal bool button02;
    internal bool button03;
    internal bool button04;
    internal bool button05;
    internal bool button06;
    internal bool button07;
    internal bool button08;
    internal bool button09;
    internal bool button10;
    internal bool button11;
    internal bool button12;


    SerialPort sp = new SerialPort("COM3",9600); 

	// Use this for initialization
	void Start () {
        sp.Open();
        sp.ReadTimeout = 1;
	}

    // Update is called once per frame
    void Update()
    {
        if(sp.IsOpen)
        {
            try
            {
                Controls(sp.ReadByte());
            }
            catch(System.Exception)
            {
                
            }
        }

    }

    private void Controls(int numButton)
    {
        if(numButton == 1)
        {
            button01 = true;
        }
        else
        {
            button01 = false;
        }

        if (numButton == 2)
        {
            button02 = true;
        }
        else
        {
            button02 = false;
        }

        if (numButton == 3)
        {
            button03 = true;
        }
        else
        {
            button03 = false;
        }

        if (numButton == 4)
        {
            button04 = true;
        }
        else
        {
            button04 = false;
        }

        if (numButton == 5)
        {
            button05 = true;
        }
        else
        {
            button05 = false;
        }

        if (numButton == 6)
        {
            button06 = true;
        }
        else
        {
            button06 = false;
        }

        if (numButton == 7)
        {
            button07 = true;
        }
        else
        {
            button07 = false;
        }

        if (numButton == 8)
        {
            button08 = true;
        }
        else
        {
            button08 = false;
        }

        if (numButton == 9)
        {
            button09 = true;
        }
        else
        {
            button09 = false;
        }

        if (numButton == 10)
        {
            button10 = true;
        }
        else
        {
            button10 = false;
        }

        if (numButton == 11)
        {
            button11 = true;
        }
        else
        {
            button11 = false;
        }

        if (numButton == 12)
        {
            button12 = true;
        }
        else
        {
            button12 = false;
        }

        Debug.Log(numButton);
    }
}
