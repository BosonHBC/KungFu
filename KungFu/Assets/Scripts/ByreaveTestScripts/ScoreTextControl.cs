using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScoreTextControl : MonoBehaviour
{
    public Text ScoreText;
    public Text MissText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScore(string text)
    {
        ScoreText.text = text;
    }

    public void SetMiss(string miss)
    {
        MissText.text = miss;
    }
}
