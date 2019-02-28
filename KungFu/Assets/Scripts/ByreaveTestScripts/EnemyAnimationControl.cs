using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class EnemyAnimationControl : MonoBehaviour
{
    Animator enemyAnimator;
    Dictionary<int, AnimationInfo> AnimationData;
    Dictionary<int, BeatInfo> BeatData;
    // Start is called before the first frame update
    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        AnimationData = MyGameInstance.instance.GetComponent<DataLoader>().GetAnimationInfos();
        BeatData = MyGameInstance.instance.GetComponent<DataLoader>().GetBeatInfos();
        //AddSlowDownEvent();
    }

    public void PlayAnim(int AnimationID)
    {
        //enemyAnimator.Play(AnimationData[AnimationID].AnimationName);
        enemyAnimator.SetInteger("AttackID_i", AnimationID + 1);
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

    //void AddBeatTimingToList()
    //{
    //    foreach(var data in animData)
    //    {
    //        animationTimingData.Add(new BeatAnimation(data.Value[0].AsInt, data.Value[1], data.Value[2].AsFloat, data.Value[3].AsFloat, data.Value[4].AsFloat, data.Value[5].AsFloat));
    //    }
    //}
    void AddSlowDownEvent()
    {
        AnimationClip[] animationClips = enemyAnimator.runtimeAnimatorController.animationClips;
        foreach(AnimationClip ac in animationClips)
        {
            AnimationInfo timing = getAnimationInfoByName(ac.name);
            if(timing != null)
            {
                foreach(var beatID in timing.BeatIDs)
                {
                    var beatInfo = BeatData[beatID];
                    AnimationEvent animEvt = new AnimationEvent
                    {
                        time = beatInfo.OKStart,
                        floatParameter = beatInfo.PerfectStart - beatInfo.OKStart,
                        functionName = "SlowDown"
                    };
                    ac.AddEvent(animEvt);
                }
                //Debug.Log(ac.name);
            }
            else
            {
                Debug.Log(ac.name);
            }
        }
    }

    public void AddSlowDownEvent(AnimationInfo animInfo)
    {
        AnimationClip[] animationClips = enemyAnimator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip ac in animationClips)
        {
            if(ac.name == animInfo.AnimationName)
            {
                ac.events = null;
                foreach(var beat in animInfo.BeatIDs)
                {
                    var beatInfo = BeatData[beat];
                    AnimationEvent animEvt = new AnimationEvent
                    {
                        time = beatInfo.OKStart,
                        floatParameter = beatInfo.PerfectStart - beatInfo.OKStart,
                        functionName = "SlowDown"
                    };
                    ac.AddEvent(animEvt);
                    //Debug.Log(ac.events.Length);
                }
                break;
            }
        }
    }

    AnimationInfo getAnimationInfoByName(string name)
    {
        foreach(var animInfo in AnimationData.Values)
        {
            if (animInfo.AnimationName == name)
                return animInfo;
        }
        return null;
    }
}
