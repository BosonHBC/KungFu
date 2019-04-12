using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXControl : MonoBehaviour
{
    public AudioClip[] ImpactSFXs;

    public void PlayRandomImpactSFX()
    {
        GetComponent<AudioSource>().PlayOneShot(ImpactSFXs[Random.Range(0, ImpactSFXs.Length)]);
    }
}
