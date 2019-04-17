using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageBlink : MonoBehaviour
{
    public Image ImageToBlink;
    public float BlinkInterval;
    [Range(0.0f, 1.0f)]
    public float MinAlpha;
    [Range(0.5f, 1.0f)]
    public float MaxAlpha;
    // Start is called before the first frame update
    void Start()
    {
        ImageToBlink.canvasRenderer.SetAlpha(MaxAlpha);
    }

    // Update is called once per frame
    void Update()
    {
        if(ImageToBlink.canvasRenderer.GetAlpha() >= MaxAlpha)
        {
            ImageToBlink.CrossFadeAlpha(MinAlpha, BlinkInterval, false);
        }
        else if(ImageToBlink.canvasRenderer.GetAlpha() <= MinAlpha)
        {
            ImageToBlink.CrossFadeAlpha(MaxAlpha, BlinkInterval, false);
        }
    }
}
