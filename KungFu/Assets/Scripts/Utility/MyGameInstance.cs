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
    [SerializeField] private int iOkScore = 800;
    [SerializeField] private int fComboAward;
    [SerializeField] private int iComboFightScore = 500;
    [SerializeField] private ComboController comboText;
    [SerializeField] private Text scoreText;
    DataLoader loader;
    private int iComboFightCount;
    private int perfectCount;
    private int okCount;

    //Menu Select Data
    //[HideInInspector]
    public int PlayerCharacterIndex = 0;
    //[HideInInspector]
    public int SongIndex = 0;
    //Awake is always called before any Start functions
    void Awake()
    {
        //PlayerCharacterIndex = 1;
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

        loader = GetComponent<DataLoader>();
    }
    public void Score(HitResult hr)
    {
        //comboText.text = iCombo.ToString();
        switch (hr)
        {
            case HitResult.Perfect:
                if (++iCombo >= iMaxCombo)
                    iMaxCombo = iCombo;
                scores += (int)(iCombo * fComboAward + iPerfectScore);
                perfectCount++;
                break;
            case HitResult.Good:
                if (++iCombo >= iMaxCombo)
                    iMaxCombo = iCombo;
                scores += (int)(iCombo * fComboAward + iOkScore);
                okCount++;
                break;
            case HitResult.Combo:
                iComboFightCount++;
                break;
        }
        if (comboText)
        {
            comboText.ChangeCombo(iCombo);

            scoreText.text = scores.ToString();
            scoreText.transform.parent.GetComponent<Animator>().Play("Pop");

        }
    }

    public void Miss(int number)
    {
        misses += number;

        iCombo = 0;
        comboText.ChangeCombo(iCombo);

    }

    public void SetArduinoInput(bool[] arduinoInput)
    {
        buttonInput = arduinoInput;
    }
    public bool[] GetArduinoInput()
    {
        if (buttonInput == null)
            return new bool[7] { false, false, false, false, false, false, false };
        return buttonInput;
    }

    public void RestartGame()
    {
        scores = 0;
        misses = 0;
        iCombo = 0;
        iMaxCombo = 0;

        perfectCount = 0;
        okCount = 0;
        PlayerCharacterIndex = 0;
        SongIndex = 0;

    }
    public void StartGame()
    {
        LevelLoader.instance.LoadScene("FightingScene_0");
    }

    public void SetScoreUI(ComboController _comboText, Text _scoreText)
    {
        comboText = _comboText;

        scoreText = _scoreText;
        scoreText.text = "0";
    }

    public void CalcualteScore()
    {
        Debug.Log("Max Combo: " + iMaxCombo);
        int overallComboFightScore = iComboFightScore * iComboFightCount;
        EndUIController.instance.StartEndSession(loader.GetBeatNumByName("BattleGirl_H"), scores, iPerfectScore, fComboAward, iMaxCombo, overallComboFightScore);
        EndUIController.instance.SetData(scores, perfectCount, okCount, misses);
    }
}
