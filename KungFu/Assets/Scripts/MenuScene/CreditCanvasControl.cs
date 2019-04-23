using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditCanvasControl : MonoBehaviour
{
    private void OnEnable()
    {
        MenuCanvasControl.OnCanvasChange += OnCreditCanvasEnter;
    }
    void OnCreditCanvasEnter(MenuCanvasControl.MenuCanvas canvasIndex)
    {
        //if(canvasIndex == MenuCanvasControl.MenuCanvas.Credits)
        //{
        //    GetComponent<Animator>().Play("CreditMove");
        //}
    }
    private void OnDisable()
    {
        MenuCanvasControl.OnCanvasChange -= OnCreditCanvasEnter;
    }
}
