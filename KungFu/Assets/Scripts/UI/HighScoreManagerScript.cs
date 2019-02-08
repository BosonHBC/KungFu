﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// High score manager.
/// Local highScore manager for LeaderboardLength number of entries
/// 
/// this is a singleton class.  to access these functions, use HighScoreManager._instance object.
/// eg: HighScoreManager._instance.SaveHighScore("meh",1232);
/// No need to attach this to any game object, thought it would create errors attaching.
/// </summary>

public class HighScoreManager : MonoBehaviour
{

    private static HighScoreManager m_instance;
    private static Text text;
    private bool isLarger;
    private const int LeaderboardLength = 5;

    public static HighScoreManager _instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new GameObject("HighScoreManager").AddComponent<HighScoreManager>();
                m_instance.gameObject.AddComponent<GetPlayerNameScript>().enabled = false;
                text = m_instance.gameObject.AddComponent<Text>();
                text.fontSize = 40;
            }
            return m_instance;
        }
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        isLarger = false;
    }

    public void SaveHighScore(string name, int score)
    {
        List<Scores> HighScores = new List<Scores>();

        int i = 1;
        while (i <= LeaderboardLength && PlayerPrefs.HasKey("HighScore" + i + "score"))
        {
            Scores temp = new Scores();
            temp.score = PlayerPrefs.GetInt("HighScore" + i + "score");
            temp.name = PlayerPrefs.GetString("HighScore" + i + "name");
            HighScores.Add(temp);
            i++;
        }
        if (HighScores.Count == 0)
        {
            Scores _temp = new Scores();
            _temp.name = name;
            _temp.score = score;
            HighScores.Add(_temp);
        }
        else
        {
            for (i = 1; i <= HighScores.Count && i <= LeaderboardLength; i++)
            {
                if (score > HighScores[i - 1].score)
                {
                    Scores _temp = new Scores();
                    _temp.name = name;
                    _temp.score = score;
                    HighScores.Insert(i - 1, _temp);
                    break;
                }
                if (i == HighScores.Count && i < LeaderboardLength)
                {
                    Scores _temp = new Scores();
                    _temp.name = name;
                    _temp.score = score;
                    HighScores.Add(_temp);
                    break;
                }
            }
        }

        i = 1;
        while (i <= LeaderboardLength && i <= HighScores.Count)
        {
            PlayerPrefs.SetString("HighScore" + i + "name", HighScores[i - 1].name);
            PlayerPrefs.SetInt("HighScore" + i + "score", HighScores[i - 1].score);
            i++;
        }

    }

    public List<Scores> GetHighScore()
    {
        List<Scores> HighScores = new List<Scores>();

        int i = 1;
        while (i <= LeaderboardLength && PlayerPrefs.HasKey("HighScore" + i + "score"))
        {
            Scores temp = new Scores();
            temp.score = PlayerPrefs.GetInt("HighScore" + i + "score");
            temp.name = PlayerPrefs.GetString("HighScore" + i + "name");
            HighScores.Add(temp);
            i++;
        }

        return HighScores;
    }

    public void ClearLeaderBoard()
    {

        List<Scores> HighScores = GetHighScore();

        for (int i = 1; i <= HighScores.Count; i++)
        {
            PlayerPrefs.DeleteKey("HighScore" + i + "name");
            PlayerPrefs.DeleteKey("HighScore" + i + "score");
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }

    public void SetText(string name)
    {
        text.text = name;
    }

    public void DisableText()
    {
        text.enabled = false;
    }

    public void CheckIfHighScore(int score)
    {
        foreach (Scores highScore in GetHighScore())
        {
             if (score >= highScore.score)
            {
              isLarger = true;
              break;
            }
        }

        if (isLarger)
        {
            GameObject.Find("Canvas").GetComponent<GetNameTextScript>().SetTextRef(text, score.ToString());
            gameObject.GetComponent<GetPlayerNameScript>().enabled = true;
            gameObject.GetComponent<GetPlayerNameScript>().ReceiveScore(score);
        }
    }
}


public class Scores
{
    public int score;
    public string name;
}
