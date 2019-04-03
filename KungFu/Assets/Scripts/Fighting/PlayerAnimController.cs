using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : BaseAnimController
{

    private bool bCombing;
   // private bool bPreparing;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    void Update()
    {
        //DebugAnimator();
    }
    void DebugAnimator()
    {
        // Reset to 0 to prevent keep playing one animation
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown((KeyCode)(i + 48)))
            {
                anim.SetFloat("Attack_Anim_ID", (i / 10f));
                anim.SetFloat("AttackID_i", (i / 10f));
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                anim.SetFloat("Attack_Anim_ID",0f);
                anim.SetFloat("AttackID_i", (i / 10f));
            }

        }

    }
    public void PlayPlayerAttackAnimation(float _AttackID)
    {
        //Debug.Log("Anim ID: " + _AttackID);
        anim.SetFloat("AttackID_i", _AttackID);
        anim.SetFloat("Attack_Anim_ID", _AttackID);
    }

    public void PlayGuardAnimation(int _releativeAttackID = 0)
    {
        //anim.Play("Guard");
        anim.SetInteger("DefenseID_i", _releativeAttackID + 1);
    }

    public void PlayComboAnimation(float _combTime)
    {
        if (!bCombing)
        {
            anim.SetFloat("AttackID_i", -2);
            bCombing = true;
            StartCoroutine(finishCombo(_combTime));
        }

    }
    IEnumerator finishCombo(float _combTime)
    {
        yield return new WaitForSeconds(_combTime);
        bCombing = false;
    }

}
