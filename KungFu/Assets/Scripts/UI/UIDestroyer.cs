using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(MosicFader))]
public class UIDestroyer : MonoBehaviour
{
    MosicFader fader;
    DestroyParticle particle;
    public float delayToDeath;
    [SerializeField] Color _color;
    private bool bDestroying;
    private void Start()
    {
        fader = GetComponent<MosicFader>();
        particle = GetComponentInChildren<DestroyParticle>();

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
            if (particle != null)
                particle.PlayParticle(_color);
            //UnityEditor.EditorApplication.isPaused = true;
            bDestroying = true;
            fader = GetComponent<MosicFader>();

            if (fader != null)
                fader.FadeTo(0, delayToDeath, delegate { Destroy(gameObject); });
        }
    }
}
