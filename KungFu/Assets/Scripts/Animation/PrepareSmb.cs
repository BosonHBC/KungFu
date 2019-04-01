using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareSmb : BaseSmb
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        BaseAnimController baseAnim =
animator.gameObject.GetComponent<BaseAnimController>();
        baseAnim.LerpFromPrepareToFight();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

    }

}
