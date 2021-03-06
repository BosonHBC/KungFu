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
    private bool isLarger;
    private string[] songs = {"BattleGirl_H","Sailor_H"};
    private const int LeaderboardLength = 5;

    public static HighScoreManager _instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new GameObject("HighScoreManager").AddComponent<HighScoreManager>();
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

        HighScoreManager._instance.SaveHighScore("", 0, songs[0]);
        HighScoreManager._instance.SaveHighScore("", 0, songs[1]);

        isLarger = false;
    }

    public void SaveHighScore(string name, int score, string song)
    {
        List<Scores> HighScores = new List<Scores>();

        int i = 1;
        while (i <= LeaderboardLength && PlayerPrefs.HasKey(song + i + "score"))
        {
            Scores temp = new Scores();
            temp.score = PlayerPrefs.GetInt(song + i + "score");
            temp.name = PlayerPrefs.GetString(song + i + "name");
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
            PlayerPrefs.SetString(song + i + "name", HighScores[i - 1].name);
            PlayerPrefs.SetInt(song + i + "score", HighScores[i - 1].score);
            i++;
        }

    }

    public List<Scores> GetHighScore(int songID)
    {
        List<Scores> HighScores = new List<Scores>();

        int i = 1;
        while (i <= LeaderboardLength && PlayerPrefs.HasKey(songs[songID] + i + "score"))
        {
            Scores temp = new Scores();
            temp.score = PlayerPrefs.GetInt(songs[songID] + i + "score");
            temp.name = PlayerPrefs.GetString(songs[songID] + i + "name");
            HighScores.Add(temp);
            i++;
        }

        return HighScores;
    }

    public void ClearLeaderBoard(int songID)
    {

        List<Scores> HighScores = GetHighScore(songID);

        for (int i = 1; i <= HighScores.Count; i++)
        {
            PlayerPrefs.DeleteKey(songs[songID] + i + "name");
            PlayerPrefs.DeleteKey(songs[songID] + i + "score");
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }

    public void CheckIfHighScore(int songID, string playerName, int score)
    {
        foreach (Scores highScore in GetHighScore(songID))
        {
             if (score >= highScore.score)
            {
              isLarger = true;
              break;
            }
        }

        if (isLarger)
        {
            HighScoreManager._instance.SaveHighScore(playerName, score, songs[songID]);
        }
    }
}


public class Scores
{
    public int score;
    public string name;
}

