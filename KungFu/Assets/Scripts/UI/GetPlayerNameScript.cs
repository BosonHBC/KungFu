using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayerNameScript : MonoBehaviour {

    internal string stringToEdit;
    private AudioSource audioSource;
    public AudioClip highScoreSound;

    private void Awake()
    {     
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        stringToEdit = " HighScore!!! Please Enter Your Name";
        audioSource.PlayOneShot(highScoreSound);
    }

    void OnGUI()
    {
        stringToEdit = GUI.TextField(new Rect(Screen.width/2-365, Screen.height/2+165, 730, 50), stringToEdit, 50);
        GUI.skin.textField.fontSize = 40;
    }
}
