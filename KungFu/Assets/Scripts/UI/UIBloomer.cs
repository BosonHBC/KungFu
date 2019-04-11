using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBloomer : MonoBehaviour
{
    Image _image;
    [SerializeField] float[] fa_trans = { 0, 0.8f };


    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, fa_trans[0]);
    }

    public void BloomTo(int _dest, float _fadeTime = 0.1f)
    {
        // Only start and end
        StopAllCoroutines();
        if( _image)
        {
            float _lerpData = _image.color.a;
            StartCoroutine(SimpleLerper(_lerpData, fa_trans[_dest], _fadeTime));
        }
        else
        {
            Debug.LogError("Null reference");
        }
    }

    public void Blink(float _time, Color _color)
    {
        StartCoroutine(ie_Blink(_time, _color));
    }

    IEnumerator ie_Blink(float _time, Color _color)
    {
        //BloomTo(1, _time);
        _image.color = new Color(_color.r, _color.g, _color.b, fa_trans[1]);
        yield return new WaitForSeconds(_time);
        //BloomTo(0, _time);
        _image.color = new Color(_color.r, _color.g, _color.b, fa_trans[0]);
    }

    IEnumerator SimpleLerper(float _start, float _end, float _fadeTime = 0.5f)
    {
        float _timeStartFade = Time.time;
        float _timeSinceStart = Time.time - _timeStartFade;
        float _lerpPercentage = _timeSinceStart / _fadeTime;

        while (true)
        {
            _timeSinceStart = Time.time - _timeStartFade;
            _lerpPercentage = _timeSinceStart / _fadeTime;

            float currentValue = Mathf.Lerp(_start, _end, _lerpPercentage);
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, currentValue);

            if (_lerpPercentage >= 1) break;

            yield return new WaitForEndOfFrame();
        }
    }
}
