using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXControl : MonoBehaviour
{
    public AudioClip[] MissSFXs;
    public AudioClip[] MatchSFXs;

    public void PlayRandomMissSFX()
    {
        GetComponent<AudioSource>().PlayOneShot(MissSFXs[Random.Range(0, MissSFXs.Length)]);
    }

    public void PlayRandomMatchSFX()
    {
        GetComponent<AudioSource>().PlayOneShot(MatchSFXs[Random.Range(0, MatchSFXs.Length)]);
    }
}
