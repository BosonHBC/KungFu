using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIFader))]
public class UIDestroyer : MonoBehaviour
{
    UIFader fader;
    public float delayToDeath;
    private bool bDestroying;
    public void GoDie()
    {
        if (!bDestroying)
        {
            bDestroying = true;
            fader = GetComponent<UIFader>();
            fader.FadeOut(delayToDeath, delegate { Destroy(gameObject); });
        }
        
    }
}
