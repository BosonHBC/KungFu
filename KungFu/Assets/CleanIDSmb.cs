using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanIDSmb : BaseSmb
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.SetFloat("AttackID_i", -1);
        animator.SetInteger("DefenseID_i", -1);
    }
}
