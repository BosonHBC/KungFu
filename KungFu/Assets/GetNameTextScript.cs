using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetNameTextScript : MonoBehaviour
{
    private Text scoreManagerNameText;
    private bool highScoreAvailable;
    public Text nameText;
    public Text scoreText;
    // Start is called before the first frame update

    private void Start()
    {
        highScoreAvailable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (highScoreAvailable)
        {
            nameText.text = scoreManagerNameText.text;
        }
    }

    public void SetTextRef(Text nameText, string scoreText)
    {
        scoreManagerNameText = nameText;
        this.scoreText.text = scoreText;
        highScoreAvailable = true;
    }
}
