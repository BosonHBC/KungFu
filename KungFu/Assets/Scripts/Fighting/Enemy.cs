using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [Header("Enemy")]
    [SerializeField] private float fDashSpeed;
    [SerializeField] private float fDashThreshold;
    private float fDistToOpponent;
    private bool bDashing;
    // Start is called before the first frame update
   protected override void Start()
    {
        base.Start();
        anim = GetComponent<EnemyAnimationControl>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void DashToPlayer(int vert, float _time)
    {
        fDistToOpponent = Vector3.Distance(transform.position, trOppoent.position);
        Debug.Log("Dist: " + fDistToOpponent);
        if (fDistToOpponent <= fDashThreshold)
            return;
        bDashing = true;
        // 6.571 is the average speed of the animation, checked in the fbx file. // 8.16f
        float _t = (fDistToOpponent - fDashThreshold) / 6.571f;
        Debug.Log("Start Dash, dash duration: " + _t);

        anim.DashVertically(vert, fDashSpeed, _t, delegate { bDashing = false; });
    }
}
