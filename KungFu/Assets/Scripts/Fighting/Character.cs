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
    Animator iconGetRed;

    protected bool bFaceToOpponent = true;

    [SerializeField] private float fDashSpeed;
    [SerializeField] private float fDashThreshold;
    private float fDistToOpponent;
    private bool bDashing;
    AttackJointID[] attackJoints;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        anim = GetComponent<BaseAnimController>();
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        FaceToOpponent();
        if (bDashing)
        {
            fDistToOpponent = Vector3.Distance(transform.position, trOppoent.position);
            if (fDistToOpponent <= fDashThreshold)
            {
                bDashing = false;
                anim.StopDash(0.2f);
            }
        }
    }

    public virtual void GetDamage(float _dmg, float[] _attackDir)
    {
        float _afterDmg = fCurrentHp - _dmg;
        anim.GetDamage(_attackDir);
        StartCoroutine(GetDamage(fCurrentHp, _afterDmg<0?0:_afterDmg));
        if (fCurrentHp <= 0)
        {
            FightingManager.instance.FightOver(iCharID);
        }
    }
    public virtual void GameOver(bool _win)
    {
        anim.PlayEndAnim(_win);
    }
    public virtual void  ExecuteOpponent()
    {

    }

    public void SetData(Image _img, Transform _trOpponent, int _icharID)
    {
        hpFillBar = _img;
        iconGetRed = hpFillBar.transform.parent.parent.parent.Find("Icon").GetComponent<Animator>();
        fCurrentHp = fMaxHp;
        bFaceToOpponent = true;
        iCharID = _icharID;
        name = "P" + iCharID + "_" + sCharName;
        trOppoent = _trOpponent;

        attackJoints = transform.GetComponentsInChildren<AttackJointID>();
        //Debug.Log(name + " " + attackJoints.Length);
        System.Array.Sort(attackJoints, delegate (AttackJointID _a1, AttackJointID _a2) {
            return _a1.iJointID.CompareTo(_a2.iJointID);
        });
    }
    
    public Transform GetJointPositionByJointID(int _id)
    {
        return attackJoints[_id].transform;
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
        iconGetRed.Play("IconGetRed");
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

    public void DashToOpponent(int vert, float _time)
    {
        fDistToOpponent = Vector3.Distance(transform.position, trOppoent.position);

        if (fDistToOpponent <= fDashThreshold)
            return;
        bDashing = true;

        anim.DashVertically(vert, fDashSpeed, 5, delegate { bDashing = false; });
    }
}
