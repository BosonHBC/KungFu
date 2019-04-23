using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCanvasControl : MonoBehaviour
{
    public enum MenuCanvas
    {
        MainMenu,
        CharacterSelect,
        SongSelect,
        Credits
    }
    TransitionControl CanvasMove;
    AudioSource audioSource;
    //0 Main, 1 Character Select, 2 Song Select, 3 Credits
    public int CurrentCanvas = 1;
    public AudioClip CanvasChange;
    public AudioClip OptionSwitch;
    public AudioClip GameStart;
    public delegate void ButtonAction(MenuCanvas currentCanvas);
    public static event ButtonAction OnCanvasChange;
    public static event ButtonAction OnSelectLeft;
    public static event ButtonAction OnSelectRight;
    public Text[] TopMenuText;

    float ArduinoCoolDownTimer = 0.0f;
    float ArduinoCoolDownTime = 0.5f;
    bool isInCoolDown = false;
    Dictionary<MenuCanvas, string[]> CanvasText;
    // Start is called before the first frame update
    void Start()
    {
        CanvasMove = FindObjectOfType<TransitionControl>();
        audioSource = GetComponent<AudioSource>();
        CanvasText = new Dictionary<MenuCanvas, string[]>()
        {
            {MenuCanvas.MainMenu, new string[] {"CREDITS", "CHARACTERS" } },
            {MenuCanvas.CharacterSelect, new string[] {"TUTORIAL", "SONGS" } },
            {MenuCanvas.SongSelect, new string[] {"CHARACTERS", "CREDITS" } },
            {MenuCanvas.Credits, new string[] {"SONGS", "TUTORIAL" } },
        };
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputFromKeyboard();
        CheckInputFromArduino();
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
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnSelectLeft?.Invoke((MenuCanvas)CurrentCanvas);
            audioSource.PlayOneShot(OptionSwitch);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            OnSelectRight?.Invoke((MenuCanvas)CurrentCanvas);
            audioSource.PlayOneShot(OptionSwitch);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.PlayOneShot(GameStart);
            Camera.main.GetComponent<AudioSource>().Pause();
            MyGameInstance.instance.StartGame();
        }
        if(isInCoolDown)
        {
            ArduinoCoolDownTimer += Time.deltaTime;
            if(ArduinoCoolDownTimer >= ArduinoCoolDownTime)
            {
                ArduinoCoolDownTimer = 0.0f;
                isInCoolDown = false;
            }
        }
    }

    void CheckInputFromArduino()
    {
        if(!isInCoolDown)
        {
            bool[] arduinoInput = MyGameInstance.instance.GetArduinoInput();
            for (int i = 0; i < arduinoInput.Length; ++i)
            {
                if (arduinoInput[i])
                {
                    isInCoolDown = true;
                    switch (i)
                    {
                        case 0:
                            audioSource.PlayOneShot(GameStart);
                            Camera.main.GetComponent<AudioSource>().Pause();
                            MyGameInstance.instance.StartGame();
                            break;
                        case 1:
                            PageLeft();
                            break;
                        case 3:
                            OnSelectLeft?.Invoke((MenuCanvas)CurrentCanvas);
                            audioSource.PlayOneShot(OptionSwitch);
                            break;
                        case 5:
                            OnSelectRight?.Invoke((MenuCanvas)CurrentCanvas);
                            audioSource.PlayOneShot(OptionSwitch);
                            break;
                        case 7:
                            PageRight();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
    void PageLeft()
    {
        if (CanvasMove.bMoving)
            return;
        CanvasMove.MoveMenu(-1);
        CurrentCanvas -= 1;
        if (CurrentCanvas < 0)
            CurrentCanvas = 3;
        audioSource.PlayOneShot(CanvasChange);
        TopMenuText[0].text = CanvasText[(MenuCanvas)CurrentCanvas][0];
        TopMenuText[1].text = CanvasText[(MenuCanvas)CurrentCanvas][1];
        OnCanvasChange?.Invoke((MenuCanvas)CurrentCanvas);
    }

    void PageRight()
    {
        if (CanvasMove.bMoving)
            return;
        CanvasMove.MoveMenu(1);
        CurrentCanvas = (CurrentCanvas + 1) % 4;

        audioSource.PlayOneShot(CanvasChange);
        TopMenuText[0].text = CanvasText[(MenuCanvas)CurrentCanvas][0];
        TopMenuText[1].text = CanvasText[(MenuCanvas)CurrentCanvas][1];
        OnCanvasChange?.Invoke((MenuCanvas)CurrentCanvas);
    }
}
