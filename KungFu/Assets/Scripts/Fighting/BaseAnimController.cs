using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimController : MonoBehaviour
{
    protected Animator anim;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void PlayPrepareFight()
    {
        anim.SetInteger("AttackID_i", 0);
    }
    public void LerpFromPrepareToFight()
    {
        StartCoroutine(LerpToNumber(0, 1, 0.5f));
    }

    IEnumerator LerpToNumber(float _start, float _end, float _fadeTime)
    {
        float _timeStartFade = Time.time;
        float _timeSinceStart = Time.time - _timeStartFade;
        float _lerpPercentage = _timeSinceStart / _fadeTime;

        while (true)
        {
            _timeSinceStart = Time.time - _timeStartFade;
            _lerpPercentage = _timeSinceStart / _fadeTime;

            float currentValue = Mathf.Lerp(_start, _end, _lerpPercentage);
            anim.SetFloat("StandToFight_f", currentValue);

            if (_lerpPercentage >= 1) break;

            yield return new WaitForEndOfFrame();
        }
    }
}
