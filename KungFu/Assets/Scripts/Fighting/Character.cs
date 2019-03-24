using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("CharacterProperty")]
    [SerializeField]
    protected string sCharName;
    public int iCharID;
    protected Transform trOppoent;
    [SerializeField] protected float fMaxHp;
    [SerializeField] protected float fCurrentHp;
    protected BaseAnimController anim;
    private Image hpFillBar;


    protected bool bFaceToOpponent = true;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        anim = GetComponent<BaseAnimController>();
        //SetData(null);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        FaceToOpponent();
    }

    public virtual void GetDamage(float _dmg, bool _fromLeft)
    {
        float _afterDmg = fCurrentHp - _dmg;
        anim.GetDamage(_fromLeft);
        StartCoroutine(GetDamage(fCurrentHp, _afterDmg<0?0:_afterDmg));
        if (fCurrentHp <= 0)
        {
            Die();
        }
    }
    public virtual void Die()
    {

    }

    public void SetData(Image _img, Transform _trOpponent)
    {
        hpFillBar = _img;
        fCurrentHp = fMaxHp;
        bFaceToOpponent = true;
        name = "P" + iCharID + "_" + sCharName;
        trOppoent = _trOpponent;

    }

    void FaceToOpponent()
    {
        if (bFaceToOpponent)
        {
            transform.LookAt(trOppoent.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }

    IEnumerator GetDamage(float _CurrentHp, float _AfterDmgHp, float _fadeTime = 0.2f)
    {

        float _timeStartFade = Time.time;
        float _timeSinceStart = Time.time - _timeStartFade;
        float _lerpPercentage = _timeSinceStart / _fadeTime;

        while (true)
        {
            _timeSinceStart = Time.time - _timeStartFade;
            _lerpPercentage = _timeSinceStart / _fadeTime;
            float currentValue = Mathf.Lerp(_CurrentHp, _AfterDmgHp, _lerpPercentage);
            fCurrentHp= currentValue;
            hpFillBar.fillAmount = fCurrentHp / fMaxHp;
            if (_lerpPercentage >= 1) break;
            yield return new WaitForEndOfFrame();
        }

    }
}
