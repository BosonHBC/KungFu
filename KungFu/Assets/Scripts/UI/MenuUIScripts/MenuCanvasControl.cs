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
    public UIImageBlink GameStartBlink;
    public UIImageBlink[] SwitchBlink;
    public UIImageBlink[] SelectBlink;
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
    bool [] MenuVisited;
    bool canStartGame = false;
    bool canSwitchBlink = true;
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
        MenuVisited = new bool[CanvasText.Count];

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
            if (canStartGame)
            {
                audioSource.PlayOneShot(GameStart);
                Camera.main.GetComponent<AudioSource>().Pause();
                MyGameInstance.instance.StartGame();
            }
            
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
                            if(canStartGame)
                            {
                                audioSource.PlayOneShot(GameStart);
                                Camera.main.GetComponent<AudioSource>().Pause();
                                MyGameInstance.instance.StartGame();
                            }
                            break;
                        case 1:
                            PageLeft();
                            break;
                        case 4:
                            OnSelectLeft?.Invoke((MenuCanvas)CurrentCanvas);
                            audioSource.PlayOneShot(OptionSwitch);
                            break;
                        case 5:
                            OnSelectRight?.Invoke((MenuCanvas)CurrentCanvas);
                            audioSource.PlayOneShot(OptionSwitch);
                            break;
                        case 2:
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
        //disable switch blink after first use
        if (canSwitchBlink)
        {
            SwitchMenuBlink(false);
            canSwitchBlink = false;
        }
        

        CanvasMove.MoveMenu(-1);
        CurrentCanvas -= 1;
        if (CurrentCanvas < 0)
            CurrentCanvas = 3;

        //can start game only in select character and song menu.
        if (CurrentCanvas == (int)MenuCanvas.CharacterSelect || CurrentCanvas == (int)MenuCanvas.SongSelect)
        {
            CanStartGame(true);
            SelectButtonBlink(true);
        }
        else
        {
            CanStartGame(false);
            SelectButtonBlink(false);
        }
        audioSource.PlayOneShot(CanvasChange);
        TopMenuText[0].text = CanvasText[(MenuCanvas)CurrentCanvas][0];
        TopMenuText[1].text = CanvasText[(MenuCanvas)CurrentCanvas][1];
        OnCanvasChange?.Invoke((MenuCanvas)CurrentCanvas);
    }

    void PageRight()
    {
        if (CanvasMove.bMoving)
            return;

        if(canSwitchBlink)
        {
            SwitchMenuBlink(false);
            canSwitchBlink = false;
        }
        CanvasMove.MoveMenu(1);
        CurrentCanvas = (CurrentCanvas + 1) % 4;
        //can start game only in select character and song menu.
        if (CurrentCanvas == (int)MenuCanvas.CharacterSelect || CurrentCanvas == (int)MenuCanvas.SongSelect)
        {
            CanStartGame(true);
            SelectButtonBlink(true);
        }
        else
        {
            CanStartGame(false);
            SelectButtonBlink(false);
        }

        //MenuVisited[CurrentCanvas] = true;
        //if (DataUtility.AllTrue(MenuVisited))
        //    CanStartGame(true);
        audioSource.PlayOneShot(CanvasChange);
        TopMenuText[0].text = CanvasText[(MenuCanvas)CurrentCanvas][0];
        TopMenuText[1].text = CanvasText[(MenuCanvas)CurrentCanvas][1];
        OnCanvasChange?.Invoke((MenuCanvas)CurrentCanvas);
    }

    void CanStartGame(bool i_start)
    {
        GameStartBlink.Blink(i_start);
        canStartGame = i_start;
    }



    void SwitchMenuBlink(bool blink)
    {
        foreach(var s in SwitchBlink)
        {
            s.Blink(blink);
        }
    }
    void SelectButtonBlink(bool blink)
    {
        foreach (var s in SelectBlink)
        {
            s.Blink(blink);
        }
    }
}
