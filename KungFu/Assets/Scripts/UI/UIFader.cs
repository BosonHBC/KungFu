using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIFader : MonoBehaviour
{
    private CanvasGroup uiGroup;
    private UnityAction onFadeOut;
    private UnityAction onFadeIn;
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
    public void FadeIn(float _t, UnityAction _onFadeIn)
    {
        onFadeIn = _onFadeIn;
        StartCoroutine(FadeCanvasGroup(uiGroup.alpha, 1, _t));
    }
    public void FadeOut(float _t, UnityAction _onFadeOut)
    {
        onFadeOut = _onFadeOut;
        StartCoroutine(FadeCanvasGroup(uiGroup.alpha, 0, _t));
    }

    IEnumerator FadeCanvasGroup(float _start, float _end, float _fadeTime = 0.5f)
    {
        float _timeStartFade = Time.time;
        float _timeSinceStart = Time.time - _timeStartFade;
        float _lerpPercentage = _timeSinceStart / _fadeTime;

        while (true)
        {
            _timeSinceStart = Time.time - _timeStartFade;
            _lerpPercentage = _timeSinceStart / _fadeTime;

            float currentValue = Mathf.Lerp(_start, _end, _lerpPercentage);
            uiGroup.alpha = currentValue;

            if (_lerpPercentage >= 1) break;


            yield return new WaitForEndOfFrame();
        }

        if (_end == 0)
        {
            // fade out
            if (onFadeOut != null)
            {
              //  Debug.Log("On Fade out");
                onFadeOut.Invoke();
            }
                
        }
        else if (_end == 1)
        {
            // fade out
            if (onFadeIn != null)
            {
                Debug.Log("On Fade In");
                onFadeIn.Invoke();
            }
                
        }
    }
}
