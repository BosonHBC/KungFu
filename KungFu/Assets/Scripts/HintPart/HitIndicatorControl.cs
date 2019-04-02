using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitIndicatorControl : MonoBehaviour
{
    [SerializeField]
    private GameObject [] ChildBodyParts;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool[] arduinoInput = MyGameInstance.instance.GetArduinoInput();
        if(arduinoInput!= null)
        {
            for (int i = 0; i < ChildBodyParts.Length; ++i)
            {
                ChildBodyParts[i].SetActive(arduinoInput[i]);
            }
        }

    }
}
