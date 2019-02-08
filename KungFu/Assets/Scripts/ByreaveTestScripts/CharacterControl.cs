﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public Camera FirstPersonCam, ThirdPersonCam;

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
        switchCam = !switchCam;
        FirstPersonCam.gameObject.SetActive(!switchCam);
        ThirdPersonCam.gameObject.SetActive(switchCam);
    }
}