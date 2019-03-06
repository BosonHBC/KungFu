using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : BaseAnimController
{
    public int iPlayingFightAnimationID;
   // private bool bPreparing;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    void Update()
    {
        DebugAnimator();
    }
    void DebugAnimator()
    {
        // Reset to 0 to prevent keep playing one animation

        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown((KeyCode)(i + 49)))
            {
                iPlayingFightAnimationID = i + 1;
                anim.SetInteger("AttackID_i", iPlayingFightAnimationID);

            }
        }

    }

    public void GetDamage(bool _fromLeft)
    {
        anim.SetBool("KnockBackLeft_b", _fromLeft);
        anim.Play("KnockBack");
    }

    public void GuardSucceed()
    {
        anim.Play("Guard");
    }

}
