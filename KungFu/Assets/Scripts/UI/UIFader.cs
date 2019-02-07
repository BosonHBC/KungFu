using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFader : MonoBehaviour
{
    private CanvasGroup uiGroup;
    // Start is called before the first frame update
    void Start()
    {
        uiGroup = GetComponent<CanvasGroup>();
    }
    public void FadeIn()
    {
        StartCoroutine(FadeCanvasGroup(uiGroup.alpha, 1));
    }
    public void FadeOut()
    {
        StartCoroutine(FadeCanvasGroup(uiGroup.alpha, 0));
    }
    public void FadeIn(float _t)
    {
        StartCoroutine(FadeCanvasGroup(uiGroup.alpha, 1, _t));
    }
    public void FadeOut(float _t)
    {
        StartCoroutine(FadeCanvasGroup(uiGroup.alpha, 0, _t));
    }

    IEnumerator FadeCanvasGroup(float _start, float _end, float _fadeTime = 0.5f)
    {
        float _timeStartFade = Time.time;
        float _timeSinceStart = Time.time - _timeStartFade;
        float _lerpPercentage = _timeSinceStart/_fadeTime;

        while (true)
        {
            _timeSinceStart = Time.time - _timeStartFade;
            _lerpPercentage = _timeSinceStart / _fadeTime;

            float currentValue = Mathf.Lerp(_start, _end, _lerpPercentage);
            uiGroup.alpha = currentValue;

            if (_lerpPercentage >= 1) break;


            yield return new WaitForEndOfFrame();
        }

    }
}
