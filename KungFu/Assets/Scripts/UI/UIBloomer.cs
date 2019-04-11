using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class UIBloomer : MonoBehaviour
{
    Image _image;
    Outline _outline;
    [SerializeField] float[] fa_trans = { 0.1960784f, 1f };


    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
        _outline = GetComponent<Outline>();
        _outline.effectColor = _image.color;
        _outline.effectColor = new Color(_outline.effectColor.r, _outline.effectColor.g, _outline.effectColor.b, fa_trans[0]);
    }

    public void BloomTo(int _dest, float _fadeTime = 0.1f)
    {
        // Only start and end
        StopAllCoroutines();
        if(_outline && _image)
        {
            float _lerpData = _outline.effectColor.a;
            StartCoroutine(SimpleLerper(_lerpData, fa_trans[_dest], _fadeTime));
        }
        else
        {
            Debug.LogError("Null reference");
        }
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
            _outline.effectColor = new Color(_outline.effectColor.r, _outline.effectColor.g, _outline.effectColor.b, currentValue);

            if (_lerpPercentage >= 1) break;


            yield return new WaitForEndOfFrame();
        }
    }
}
