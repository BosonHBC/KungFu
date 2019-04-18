using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCanvasControl : MonoBehaviour
{
    TransitionControl CanvasMove;
    AudioSource audioSource;
    //0 Main, 1 Song Select, 2 Character Select, 3 Credits
    public int CurrentCanvas = 0;
    public AudioClip CanvasChange;
    public AudioClip GameStart;
    public delegate void ButtonAction(int currentCanvasID);
    public static event ButtonAction OnCanvasChange;
    public static event ButtonAction OnSelectLeft;
    public static event ButtonAction OnSelectRight;
    // Start is called before the first frame update
    void Start()
    {
        CanvasMove = FindObjectOfType<TransitionControl>();
        audioSource = GetComponent<AudioSource>();
        OnCanvasChange += canvastest1;
        OnCanvasChange += canvastest2;


    }

    void canvastest1(int i)
    {
        if(i == 1)
            Debug.Log(i);
    }
    void canvastest2(int i)
    {
        if(i == 2)
            Debug.Log(i);
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
            PageRight();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PageLeft();   
        }
    }

    void CheckInputFromArduino()
    {
        bool[] arduinoInput = MyGameInstance.instance.GetArduinoInput();
        for(int i = 0; i < arduinoInput.Length; ++ i)
        {
            switch(i)
            {
                case 0:
                    MyGameInstance.instance.StartGame();
                    break;
                case 1:
                    PageLeft();
                    break;
                case 3:
                    OnSelectLeft?.Invoke(CurrentCanvas);
                    break;
                case 5:
                    OnSelectRight?.Invoke(CurrentCanvas);
                    break;
                case 7:
                    PageRight();
                    break;
                default:
                    break;
            }
        }
    }
    void PageLeft()
    {
        CanvasMove.MoveMenu(1);
        CurrentCanvas = (CurrentCanvas + 1) % 4;
        audioSource.PlayOneShot(CanvasChange);
        OnCanvasChange?.Invoke(CurrentCanvas);
    }

    void PageRight()
    {
        CanvasMove.MoveMenu(-1);
        CurrentCanvas -= 1;
        if (CurrentCanvas < 0)
            CurrentCanvas = 3;
        audioSource.PlayOneShot(CanvasChange);

        OnCanvasChange?.Invoke(CurrentCanvas);
    }
}
