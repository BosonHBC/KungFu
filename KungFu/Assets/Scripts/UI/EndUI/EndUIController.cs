using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EndUIController : MonoBehaviour
{
    Animator anim;
    AnimationClip clip;
    AnimationEvent fillEvent;
    float[] gradeOfScore = { 0.4f, 0.6f, 0.8f };
    int iMaxScore;
    [SerializeField] Sprite[] gradeImgs;
    [SerializeField] Image fillImg;
    private float fillDuration = 1f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        fillEvent = new AnimationEvent();
        fillEvent.time = 140f / 60f;
        fillEvent.functionName = "FillImage";
        fillEvent.floatParameter = 0.8f;
        clip = anim.runtimeAnimatorController.animationClips[0];
        clip.AddEvent(fillEvent);

        
        anim.Play("EndUI");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartEndSession(int _numOfBeat, int _PerfectScore, int _comboAward)
    {
        anim.Play("EndUI");
        
        // Calculate Max score
        iMaxScore = 0;
        for (int i = 0; i < _numOfBeat; i++)
        {
            iMaxScore += _PerfectScore + i * _comboAward;
        }
    }

    public void SetData(int _maxCombo, int _score,  float _p1RemainHp, float _p2RemainHp)
    {
        float percentage = _score / iMaxScore;
        fillEvent.floatParameter = percentage;
    }

    public void FillImage(float _percent = 0.8f)
    {
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
