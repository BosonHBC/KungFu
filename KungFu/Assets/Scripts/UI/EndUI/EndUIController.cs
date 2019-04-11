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
    float[] gradeOfScore = { 0.33f, 0.50f, 0.67f };
    int iMaxScore;
    [SerializeField] Sprite[] gradeImgs;
    [SerializeField] Image fillImg;
    [SerializeField] Text comboText;
    [SerializeField] Image gradeImg;
    private float fillDuration = 1f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();


    }

    public void StartEndSession(int _numOfBeat, int _currentScore, int _PerfectScore, int _comboAward, int _maxCombo, int _comboAllScore)
    {
        GetComponent<CanvasGroup>().alpha = 1;

        // Calculate Max score
        iMaxScore = 0;
        for (int i = 0; i < _numOfBeat; i++)
        {
            iMaxScore += _PerfectScore + i * _comboAward;
        }
        Debug.Log("ComboScore: " + _comboAllScore);
        iMaxScore += _comboAllScore;
        Debug.Log("Max Score: " + iMaxScore + " currentScore: " + _currentScore);

        float percentage = _currentScore / (float)iMaxScore;

        fillEvent = new AnimationEvent();
        fillEvent.time = 140f / 60f;
        fillEvent.functionName = "FillImage";
        fillEvent.floatParameter = percentage;
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

    public void SetData(int _maxCombo, int _score, float _p1RemainHp, float _p2RemainHp)
    {

    }

    public void FillImage(float _percent = 0.8f)
    {
        Debug.Log("Fill Amount: " + _percent);
        StartCoroutine(ie_FillImage(_percent));
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
