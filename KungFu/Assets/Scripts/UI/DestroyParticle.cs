using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour
{

    ParticleSystem[] childParticles;
    // Start is called before the first frame update
    void Start()
    {
        childParticles = GetComponentsInChildren<ParticleSystem>();
    }

    public void PlayParticle(Color _color)
    {
        for (int i = 0; i < childParticles.Length; i++)
        {
            ParticleSystem.MainModule _main = childParticles[i].main;
            _main.startColor= _color;
            Debug.Log("Play particle");
            childParticles[i].Play();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
