using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MosicFader : MonoBehaviour
{
    private Material m_Mosic;
    private Image m_renderer;
    // Start is called before the first frame update
    void Start()
    {
        m_renderer = GetComponent<Image>();
        m_Mosic = m_renderer.material;
        m_Mosic.SetFloat("_FadeOut", 1);
        m_renderer.material = m_Mosic;
    }

    public void FadeTo(float _result, float _time, UnityAction _onFihishFade = null)
    {
        StartCoroutine(FadeMosic(m_Mosic.GetFloat("_FadeOut"),_result, _time, _onFihishFade));
    }

    IEnumerator FadeMosic(float _start, float _end, float _fadeTime = 0.5f, UnityAction _onFihishFade = null)
    {
        float _timeStartFade = Time.time;
        float _timeSinceStart = Time.time - _timeStartFade;
        float _lerpPercentage = _timeSinceStart / _fadeTime;

        while (true)
        {
            _timeSinceStart = Time.time - _timeStartFade;
            _lerpPercentage = _timeSinceStart / _fadeTime;

            float currentValue = Mathf.Lerp(_start, _end, _lerpPercentage);
            m_Mosic.SetFloat("_FadeOut", currentValue);
            m_renderer.material = m_Mosic;

            if (_lerpPercentage >= 1) break;


            yield return new WaitForEndOfFrame();
        }
        if (_onFihishFade!=null)
            _onFihishFade.Invoke();

    }

    private void OnDestroy()
    {
        m_Mosic.SetFloat("_FadeOut", 1);
        m_renderer.material = m_Mosic;
    }
}
