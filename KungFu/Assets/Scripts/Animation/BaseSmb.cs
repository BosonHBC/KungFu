﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSmb : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerAnimController _animCtrl = animator.gameObject.GetComponent<PlayerAnimController>();
        if (_animCtrl != null)   // it is not a enemy
        {
            _animCtrl.iPlayingFightAnimationID = -1;
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
