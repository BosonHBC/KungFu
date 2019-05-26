using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageBlink : MonoBehaviour
{
    public Graphic GraphicToBlink;
    public float BlinkInterval;
    [Range(0.0f, 1.0f)]
    public float MinAlpha;
    [Range(0.5f, 1.0f)]
    public float MaxAlpha;
    public bool BlinkOnStart = true;
    bool canBlink;
    
    // Start is called before the first frame update
    void Start()
    {
        if (GraphicToBlink == null)
            GraphicToBlink = GetComponent<Graphic>();
        GraphicToBlink.canvasRenderer.SetAlpha(MaxAlpha);
        
        canBlink = BlinkOnStart;
    }

    // Update is called once per frame
    void Update()
    {
        if(canBlink)
        {
            if (GraphicToBlink.canvasRenderer.GetAlpha() >= MaxAlpha)
            {
                GraphicToBlink.CrossFadeAlpha(MinAlpha, BlinkInterval, false);
            }
            else if (GraphicToBlink.canvasRenderer.GetAlpha() <= MinAlpha)
            {
                GraphicToBlink.CrossFadeAlpha(MaxAlpha, BlinkInterval, false);
            }
        }
    }

    public void Blink(bool i_blink)
    {
        if(canBlink != i_blink)
        {
            GraphicToBlink.canvasRenderer.SetAlpha(MaxAlpha);
            canBlink = i_blink;
        }
    }
}
