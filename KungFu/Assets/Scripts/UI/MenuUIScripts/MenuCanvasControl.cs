using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCanvasControl : MonoBehaviour
{
    TransitionControl CanvasMove;
    //0 Main, 1 Song Select, 2 Character Select, 3 Credits
    public int CurrentCanvas = 0;
    // Start is called before the first frame update
    void Start()
    {
        CanvasMove = FindObjectOfType<TransitionControl>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputFromKeyboard();
    }
    void CheckInputFromKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CanvasMove.MoveMenu(-1);
            CurrentCanvas -= 1;
            if (CurrentCanvas < 0)
                CurrentCanvas = 3;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CanvasMove.MoveMenu(1);
            CurrentCanvas = (CurrentCanvas + 1) % 4;
        }
    }
}
