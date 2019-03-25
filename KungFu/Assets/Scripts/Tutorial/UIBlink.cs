using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBlink : MonoBehaviour
{
    public float BlinkInteval = 1.0f;
    public float MaxAlpha = 0.8f;
    public float MinAlpha = 0.1f;
    bool canBlink = false;
    Image thisImage;
    private void Awake()
    {
        thisImage = GetComponent<Image>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartBlink();
    }

    // Update is called once per frame
    void Update()
    {
        if(canBlink)
        {
            if (thisImage.canvasRenderer.GetAlpha() >= MaxAlpha)
                thisImage.CrossFadeAlpha(MinAlpha, BlinkInteval, false);
            else if (thisImage.canvasRenderer.GetAlpha() <= MinAlpha)
                thisImage.CrossFadeAlpha(MaxAlpha, BlinkInteval, false);
        }
    }

    void StartBlink()
    {
        canBlink = true;
    }
}
