using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Predictor : MonoBehaviour
{
    bool[] UnoInput;
    List<GameObject> highLights = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            highLights.Add(transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UnoInput = MyGameInstance.instance.GetArduinoInput();

        for (int i = 0; i < UnoInput.Length; i++)
        {
            highLights[i].gameObject.SetActive(UnoInput[i]);
        }
    }
}
