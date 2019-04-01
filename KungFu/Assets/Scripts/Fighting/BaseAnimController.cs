using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseAnimController : MonoBehaviour
{
    protected Animator anim;
    protected bool bDashing;
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
        StartCoroutine(LerpToNumber(0, 0, 1, 0.5f));
    }
    public void GetDamage(bool _fromLeft)
    {
        anim.SetBool("KnockBackLeft_b", _fromLeft);
        
        anim.Play("KnockBack");
    }
    public void DashVertically(float _vert, float _dashSpeed, float _dashDuration, UnityAction _onFinishDash = null)
    {
        if (!bDashing)
        {
            bDashing = true;
            StartCoroutine(LerpToNumber(1, anim.GetFloat("vert"), _vert, fDashLerpTime / 2));
            anim.SetFloat("fMoveSpeed", _dashSpeed);
            StartCoroutine(onFinishDash("vert", _dashDuration, _onFinishDash));
        }
    }
    public void DashHorizontally(float _hori, float _dashSpeed, float _dashDuration, UnityAction _onFinishDash = null)
    {
        if (!bDashing)
        {
            bDashing = true;
            StartCoroutine(LerpToNumber(2, anim.GetFloat("hori"), _hori, fDashLerpTime / 2));
            anim.SetFloat("fMoveSpeed", _dashSpeed);
            StartCoroutine(onFinishDash("hori", _dashDuration, _onFinishDash));
        }
    }

    public void StopDash(float _StopTime = 0.3f)
    {
        StopAllCoroutines();
        StartCoroutine(ieStopDash(_StopTime));
    }
    IEnumerator ieStopDash(float _StopTime = 0.3f)
    {
        StartCoroutine(LerpToNumber(1, anim.GetFloat("vert"), 0, _StopTime));
        StartCoroutine(LerpToNumber(2, anim.GetFloat("hori"), 0, _StopTime));
        yield return new WaitForSeconds(fDashLerpTime);
        anim.SetFloat("fMoveSpeed", 1);
        bDashing = false;
    }
    private float fDashLerpTime = 0.3f;
    IEnumerator onFinishDash(string _dir, float _dashDuration, UnityAction _onFinishDash = null)
    {
        yield return new WaitForSeconds(_dashDuration - fDashLerpTime);

        StartCoroutine(LerpToNumber(_dir == "hori"?2:1, anim.GetFloat(_dir), 0, fDashLerpTime));
        yield return new WaitForSeconds(fDashLerpTime);
        anim.SetFloat("fMoveSpeed", 1);
        if (_onFinishDash != null)
            _onFinishDash.Invoke();
        bDashing = false;
    }

    IEnumerator LerpToNumber(int _id, float _start, float _end, float _fadeTime)
    {
        float _timeStartFade = Time.time;
        float _timeSinceStart = Time.time - _timeStartFade;
        float _lerpPercentage = _timeSinceStart / _fadeTime;

        while (true)
        {
            _timeSinceStart = Time.time - _timeStartFade;
            _lerpPercentage = _timeSinceStart / _fadeTime;

            float currentValue = Mathf.Lerp(_start, _end, _lerpPercentage);
            switch (_id)
            {
                case 0:
                    anim.SetFloat("StandToFight_f", currentValue);
                    break;
                case 1:
                    anim.SetFloat("vert", currentValue);
                    break;
                case 2:
                    anim.SetFloat("hori", currentValue);
                    break;
            }

            if (_lerpPercentage >= 1) break;

            yield return new WaitForEndOfFrame();
        }
    }
}
