using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyAnimationControl : BaseAnimController
{

    Dictionary<int, AnimationInfo> AnimationData;
    Dictionary<int, BeatInfo> BeatData;
    // Start is called before the first frame update
   protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        AnimationData = MyGameInstance.instance.GetComponent<DataLoader>().GetAnimationInfos();
        BeatData = MyGameInstance.instance.GetComponent<DataLoader>().GetBeatInfos();
        //AddSlowDownEvent();
    }

    public void PlayAnim(int AnimationID)
    {
        float _animId = (AnimationID) / 10f;
        //enemyAnimator.Play(AnimationData[AnimationID].AnimationName);
        anim.SetFloat("AttackID_i", _animId);
        anim.SetFloat("Attack_Anim_ID", _animId);
    }
    
    public void SlowDown(float time)
    {
        anim.speed = 0.3f;
        StartCoroutine(StopSlowDown(time));
    }

    IEnumerator StopSlowDown(float time)
    {
        yield return new WaitForSeconds(time);
        anim.speed = 1.0f;
    }

    public void PlayHitAnimation()
    {
        anim.Play("KnockBack");
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
        AnimationClip[] animationClips = anim.runtimeAnimatorController.animationClips;
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
        AnimationClip[] animationClips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip ac in animationClips)
        {
            if(ac.name == animInfo.AnimationName)
            {
                ac.events = null;
                foreach(var beat in animInfo.BeatIDs)
                {
                    var beatInfo = BeatData[beat];
                    Debug.Log(beatInfo.BeatID);
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
