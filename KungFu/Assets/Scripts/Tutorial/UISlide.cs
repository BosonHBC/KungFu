using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlide : MonoBehaviour
{
    public Vector2 SlideDistance;
    public float SlideTime;

    RectTransform rectTrans;
    Vector2 originalPos;

    private Vector2 SlideDestination;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        originalPos = rectTrans.anchoredPosition;
        SlideDestination = originalPos + SlideDistance;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 speed = (SlideDestination - originalPos) / SlideTime;

        //Reached Destination, start over
        if(Vector2.Distance(rectTrans.anchoredPosition, SlideDestination) <= 5.0f)
        {
            rectTrans.anchoredPosition = originalPos;
        }
        else
        {
            rectTrans.anchoredPosition += speed * Time.deltaTime;
        }
    }
}
