using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{

    private Animator anim;

    private int iPlayingFightAnimationID;
    public bool bPreparing;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        DebugAnimator();
    }
    void DebugAnimator()
    {
        // Reset to 0 to prevent keep playing one animation
        iPlayingFightAnimationID = -1;
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown((KeyCode)(i + 49)))
            {
                iPlayingFightAnimationID = i + 1;
            }
        }
        PlayPrepareFight();
        anim.SetInteger("AttackID_i", iPlayingFightAnimationID);

    }

    void PlayPrepareFight()
    {
        if (bPreparing)
        {
            bPreparing = false;
            iPlayingFightAnimationID = 0;
            anim.SetFloat("StandToFight_f", 1f);
        }

    }

}
