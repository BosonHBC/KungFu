using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongPannel : MonoBehaviour
{
    private Animator pannelAnim;
   [SerializeField] private Animator difficultyAnim;
    // Start is called before the first frame update
    public bool bSelecting;
    private int iDiffculty;
    public int iSongID;

    [Header("UI Component")]
    [SerializeField] private string sPath;
    [SerializeField] private Text charNameText;
    [SerializeField] private Image charImage;
    [SerializeField] private Image gradeIamge;
    [SerializeField] private Text songNameText;
    [SerializeField] private Text songDurationText;
    [SerializeField] private Text hSNameText;
    [SerializeField] private Text hSScoreText;

    void Start()
    {
        pannelAnim = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetData(string _characterName, string _songName, float _duration)
    {
        charNameText.text = _characterName;
        charImage.sprite = Resources.Load<Sprite>(sPath + "SongSelectedMenu/Character" + iSongID);
        songNameText.text = _songName;
        songDurationText.text = string.Format("{0}:{1:00}", (int)_duration / 60, (int)_duration % 60);

        List<Scores> scoreList = HighScoreManager._instance.GetHighScore(_songName);
        if (scoreList != null)
        {
            if (scoreList.Count > 0)
            {
                string tempName = "";
                for (int i = 0; i < 3; i++)
                {
                    tempName += scoreList[0].name.Substring(i,1) + ".";
                }
                tempName = tempName.Substring(0, 5);
                hSNameText.text = tempName;
                int score = scoreList[0].score;
                hSScoreText.text = score.ToString();

                if(score>= 90000)
                {
                    gradeIamge.sprite = Resources.Load<Sprite>(sPath +"S");
                }
                else if(score >= 80000 && score < 90000)
                {
                    gradeIamge.sprite = Resources.Load<Sprite>(sPath + "A");
                }
                else
                {
                    gradeIamge.sprite = Resources.Load<Sprite>(sPath + "B");
                }

            }
        }
        else
            Debug.LogError("Can not find High score with song " + _songName); 
    }

    public void ExpandPannel() {
        bSelecting = true;
        pannelAnim.Play("Expand");
    }

    public void FoldPannel()
    {
        bSelecting = false;
        pannelAnim.Play("Fold");

    }

    public void SwitchDifficult(int _difficultty)
    {
        if (bSelecting)
        {
            switch (_difficultty)
            {
                case 0:
                    {
                        if(iDiffculty == 1)
                        {
                            iDiffculty = 0;
                            difficultyAnim.Play("SwitchToEasy");
                        }
                        break;
                    }
                case 1:
                    {
                        if(iDiffculty == 0)
                        {
                            iDiffculty = 1;
                            difficultyAnim.Play("SwitchToHard");
                        }
                        break;
                    }
                default:
                    break;
            }
        }
   
    }
}
