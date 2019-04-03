using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    // Start is called before the first frame update
   protected override void Start()
    {
        anim = GetComponent<EnemyAnimationControl>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

   

    }


}
