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
    private HpBarControl hpCtrl;
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
                //UnityEditor.EditorApplication.isPaused = true;
                bDashing = false;
                anim.StopDash(0.2f);
            }
        }
    }

    public virtual void GetDamage(float _dmg, float[] _attackDir)
    {
        
        anim.GetDamage(_attackDir);
        hpCtrl.GetDamage(fCurrentHp,_dmg);
        fCurrentHp -= _dmg;
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

    public void SetData(HpBarControl _hpCtrl, Transform _trOpponent, int _icharID)
    {
        hpCtrl = _hpCtrl;
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

   

    public void DashToOpponent(int vert, float _time)
    {
        fDistToOpponent = Vector3.Distance(transform.position, trOppoent.position);

        if (fDistToOpponent <= fDashThreshold)
            return;
        bDashing = true;

        anim.DashVertically(vert, fDashSpeed, 1.5f, delegate { bDashing = false; });
    }
}
