using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeImage : MonoBehaviour
{
    public float fFadeRate;
    private Image Img;
    private float fTargetAlpha;
    // Start is called before the first frame update
    void Start()
    {
        Img = GetComponent<Image>();
        Fade(0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        Color curColor = Img.color;
        float alphaDiff = Mathf.Abs(fTargetAlpha - curColor.a);
        if (alphaDiff > 0.2f)
        {
            transform.position += 0.5f*Vector3.up*Time.deltaTime;
            curColor.a = Mathf.Lerp(curColor.a, fTargetAlpha, fFadeRate * Time.deltaTime);
           Img.color = curColor;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Fade(float _alpha)
    {
        fTargetAlpha = _alpha;
    }
}
