using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class EnemyAnimationControl : MonoBehaviour
{
    Animator enemyAnimator;
    JSONNode animData;
    Dictionary<int, string> animMapping;
    // Start is called before the first frame update
    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        animData = MyGameInstance.instance.GetComponent<TestDataLoader>().GetAnimationData();
        animMapping = new Dictionary<int, string>()
        {
            {0, "Hook"},
            {1, "Hammer" },
            {2, "Uppercut" },
            {3, "handssors" }

        };
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayAnim(int AnimationID)
    {
        foreach(var anim in animData.Values)
        {
            if(anim["AnimationID"].AsInt == AnimationID)
            {

                Debug.Log(AnimationID);
                Debug.Log(anim["AnimationName"].Value);
                enemyAnimator.Play(anim["AnimationName"].Value);
                return;
            }
        }
        Debug.Log("No such Animation!");
    }

    public HitResult CheckHit(int AnimationID, float ReactTime)
    {
        foreach(var anim in animData.Values)
        {
            if(anim["AnimationID"].AsInt == AnimationID)
            {
                if (ReactTime >= anim["PerfectStart"].AsFloat && ReactTime <= anim["PerfectStart"].AsFloat + anim["PerfectDuration"].AsFloat)
                    return HitResult.Perfect;
                else if (ReactTime >= anim["OKStart"].AsFloat && ReactTime <= anim["OKStart"].AsFloat + anim["OKDuration"].AsFloat)
                    return HitResult.Good;
                else
                    return HitResult.Miss;
            }
        }
        return HitResult.Miss;
    }
}
