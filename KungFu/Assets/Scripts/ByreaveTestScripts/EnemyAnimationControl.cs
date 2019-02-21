using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class EnemyAnimationControl : MonoBehaviour
{
    Animator enemyAnimator;
    JSONNode animData;
    Dictionary<int, string> animMapping;
    List<BeatAnimation> animationTimingData;
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
        animationTimingData = new List<BeatAnimation>();
        AddBeatTimingToList();
        AddSlowDownEvent();
    }

    public void PlayAnim(int AnimationID)
    {
        foreach(var anim in animData.Values)
        {
            if(anim["AnimationID"].AsInt == AnimationID)
            {
                enemyAnimator.Play(anim["AnimationName"].Value);
                return;
            }
        }
    }

    public void SlowDown(float time)
    {
        enemyAnimator.speed = 0.3f;
        StartCoroutine(StopSlowDown(time));
    }

    IEnumerator StopSlowDown(float time)
    {
        yield return new WaitForSeconds(time);
        enemyAnimator.speed = 1.0f;
    }

    void AddBeatTimingToList()
    {
        foreach(var data in animData)
        {
            animationTimingData.Add(new BeatAnimation(data.Value[0].AsInt, data.Value[1], data.Value[2].AsFloat, data.Value[3].AsFloat, data.Value[4].AsFloat, data.Value[5].AsFloat));
        }
    }
    void AddSlowDownEvent()
    {
        AnimationClip[] animationClips = enemyAnimator.runtimeAnimatorController.animationClips;
        foreach(AnimationClip ac in animationClips)
        {
            BeatAnimation timing = getBeatAnimationByName(ac.name);
            if(timing != null)
            {
                Debug.Log(ac.name);
                AnimationEvent animEvt = new AnimationEvent();
                animEvt.time = timing.OKStart;
                animEvt.floatParameter = timing.PerfectStart - timing.OKStart;
                animEvt.functionName = "SlowDown";
                ac.AddEvent(animEvt);
            }
        }
    }

    BeatAnimation getBeatAnimationByName(string name)
    {
        foreach(BeatAnimation ba in animationTimingData)
        {
            if (ba.Name == name)
                return ba;
        }
        return null;
    }
}
