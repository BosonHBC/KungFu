using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareSmb : BaseSmb
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        // PlayerAnimController _pAnimCtrl = animator.gameObject.GetComponent<PlayerAnimController>();
        animator.SetFloat("StandToFight_f", 1f);

    }
}
