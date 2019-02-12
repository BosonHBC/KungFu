using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public GameObject [] Perspectives;

    int currentIndex = 0;
    bool switchCam = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SwitchPerspective();
    }

    void SwitchPerspective()
    {
        Perspectives[currentIndex].SetActive(false);
        if (++currentIndex == Perspectives.Length)
            currentIndex = 0;
        Perspectives[currentIndex].SetActive(true);
    }
}
