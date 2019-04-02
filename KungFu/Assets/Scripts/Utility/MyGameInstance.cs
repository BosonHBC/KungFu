using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum HitResult
{
    Perfect,
    Good,
    Miss,
    Mismatch,
    Combo
}
public class MyGameInstance : MonoBehaviour
{
    public static MyGameInstance instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    int scores = 0, misses = 0;
    bool[] buttonInput;

    private int iCombo;
    private int iMaxCombo;

    [Header("Score Releated")]
    [SerializeField] private int iPerfectScore = 1000;
    [SerializeField] private int iOkScore = 500;
    [SerializeField] private float fComboFactor = 30;
    [SerializeField] private Text comboText;


    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

    }
    private void Start()
    {
        iCombo = 0;
        if(comboText)
        comboText.text = "0";
    }
    public void Score(HitResult hr)
    {
        if (++iCombo >= iMaxCombo)
            iMaxCombo = iCombo;
        if (comboText)
        {
            comboText.text = iCombo.ToString();
            comboText.transform.parent.GetComponent<Animator>().Play("Pop");
        }
        //comboText.text = iCombo.ToString();
        switch (hr)
        {
            case HitResult.Perfect:
                scores += (int)(iCombo * fComboFactor * iPerfectScore);
                break;
            case HitResult.Good:
                scores += (int)(iCombo * fComboFactor * iOkScore);
                break;
        }
        scores++;

    }

    public void Miss(int number)
    {
        misses += number;
        iCombo = 0;
        if (comboText)
            comboText.text = iCombo.ToString();
    }

    public void SetArduinoInput(bool[] arduinoInput)
    {
        buttonInput = arduinoInput;
    }
    public bool[] GetArduinoInput()
    {
        return buttonInput;
    }

    public void RestartGame()
    {
        scores = 0;
        misses = 0;
        iCombo = 0;
     //   comboText.text = iCombo.ToString();
        SceneManager.LoadScene("ByreaveLoadingScene");
        Invoke("StartGame", 2.0f);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("ByreaveWhitebox");
    }

    public void SetScoreUI(Text _comboText)
    {
        comboText = _comboText;
        comboText.text = "0";
    }
}
