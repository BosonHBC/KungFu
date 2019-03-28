using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(MosicFader))]
public class UIDestroyer : MonoBehaviour
{
    MosicFader fader;
    ParticleSystem particle;
    public float delayToDeath;
    private bool bDestroying;
    private void Start()
    {
        fader = GetComponent<MosicFader>();
        particle = transform.Find("Particle").GetComponent<ParticleSystem>();
    }

    public void GoDie()
    {
        if (!bDestroying)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    Image _image = transform.GetChild(i).GetComponent<Image>();
                    if (_image != null)
                        transform.GetChild(i).GetComponent<Image>().CrossFadeAlpha(0, delayToDeath / 2, true);
                }
            }
            particle.Play();
            //UnityEditor.EditorApplication.isPaused = true;
            bDestroying = true;
            fader = GetComponent<MosicFader>();

            fader.FadeTo(0, delayToDeath, delegate { Destroy(gameObject); });
        }
    }
}
