using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EndUIController : MonoBehaviour
{
    public static EndUIController instance;
    private void Awake()
    {
        if (instance == null || instance != this)
        {
            instance = this;
        }
    }

    Animator anim;
    AnimationClip clip;
    AnimationEvent fillEvent;
    float[] gradeOfScore = { 0.25f, 0.50f, 0.75f };
    int iMaxScore;
    [SerializeField] Sprite[] gradeImgs;
    [SerializeField] Image fillImg;
    [SerializeField] Text comboText;
    [SerializeField] Image gradeImg;
    private float fillDuration = 1f;
    private float percentage;

    [Header("Detail Score Field")]
    [SerializeField] private Text tx_PerfectCount;
    [SerializeField] private Text tx_OkCount;
    [SerializeField] private Text tx_MissCount;
    [SerializeField] private Text tx_ScoreCount;
    public int finalScore;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        percentage = 0;
        finalScore = 0;
    }

    public void StartEndSession(int _numOfBeat, int _currentScore, int _PerfectScore, int _comboAward, int _maxCombo, int _comboAllScore)
    {
        GetComponent<CanvasGroup>().alpha = 1;

        // Calculate Max score
        int comboAwardScore = 0;
        for (int i = 0; i < _numOfBeat; i++)
        {
            comboAwardScore += i * _comboAward;
        }
        Debug.Log("Combo Award Score: " + comboAwardScore);
        iMaxScore = _numOfBeat * _PerfectScore + comboAwardScore;
        Debug.Log("ComboScore: " + _comboAllScore);
        //iMaxScore += _comboAllScore;
        finalScore = (_currentScore); //+ _comboAllScore);
        Debug.Log("Max Score: " + iMaxScore + " currentScore: " + finalScore);

        percentage = finalScore / (float)iMaxScore;

        fillEvent = new AnimationEvent();
        fillEvent.time = 140f / 60f;
        fillEvent.functionName = "FillImage";
        // fillEvent.floatParameter = percentage;
        clip = anim.runtimeAnimatorController.animationClips[0];
        if (clip.events.Length > 0)
        {
            Debug.Log("Override event, float: " + fillEvent.floatParameter);
            clip.events[0] = fillEvent;
        }
        else
        {
            Debug.Log("Create event, float: " + fillEvent.floatParameter);
            clip.AddEvent(fillEvent);
        }

        anim.Play("EndUI");

        // SetData
        comboText.text = _maxCombo.ToString();
        if (percentage < gradeOfScore[0])
        {
            gradeImg.sprite = gradeImgs[0];
        }
        else if (percentage >= gradeOfScore[0] && percentage < gradeOfScore[1])
        {
            gradeImg.sprite = gradeImgs[1];
        }
        else if (percentage >= gradeOfScore[1] && percentage < gradeOfScore[2])
        {
            gradeImg.sprite = gradeImgs[2];
        }
        else if (percentage >= gradeOfScore[2])
        {
            gradeImg.sprite = gradeImgs[3];
        }
    }

    public void SetData(int _score, int _perfectCount, int _OkCount, int _missCount)
    {
        tx_PerfectCount.text = _perfectCount.ToString();
        tx_OkCount.text = _OkCount.ToString();
        tx_MissCount.text = _missCount.ToString();
        tx_ScoreCount.text = finalScore.ToString();
    }

    public void FillImage()
    {
        Debug.Log("Fill Amount: " + percentage);
        StartCoroutine(ie_FillImage(percentage));
    }

    IEnumerator ie_FillImage(float _end)
    {
        float _timeStartFade = Time.time;
        float _timeSinceStart = Time.time - _timeStartFade;
        float _lerpPercentage = _timeSinceStart / fillDuration;

        while (true)
        {
            _timeSinceStart = Time.time - _timeStartFade;
            _lerpPercentage = _timeSinceStart / fillDuration;

            float currentValue = Mathf.Lerp(0, _end, _lerpPercentage);
            fillImg.fillAmount = currentValue;

            if (_lerpPercentage >= 1) break;


            yield return new WaitForEndOfFrame();
        }

    }
}
