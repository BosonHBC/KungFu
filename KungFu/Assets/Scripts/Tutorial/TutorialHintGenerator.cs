using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class TutorialHintGenerator : HintGenerator
{
    //For beat timer
    TutorialBeatGenerator tutorialBeatGenerator;
    //Ring Indicator
    //RingIndicatorControl ringIndicator;
    // Start is called before the first frame update
    void Start()
    {
        animData = MyGameInstance.instance.GetComponent<DataLoader>().GetAnimationArrayByName("Kungfu");
        animationData = MyGameInstance.instance.GetComponent<DataLoader>().GetAnimationInfos();
        beatData = MyGameInstance.instance.GetComponent<DataLoader>().GetBeatInfos();
        tutorialBeatGenerator = FindObjectOfType<TutorialBeatGenerator>();
        hintsQueue = new Queue<GameObject>();
        //ringIndicator = GetComponent<RingIndicatorControl>();
    }
    private void Update()
    {
        if (tutorialBeatGenerator.bCanPlay)
        {
            //generate hints
            float hintTimer = tutorialBeatGenerator.beatTimer;
            if (animData[currentAnimationIndex]["AnimationID"].AsInt != -1)
            {
                AnimationInfo currentAnimInfo = animationData[animData[currentAnimationIndex]["AnimationID"].AsInt];
                BeatInfo currentBeatInfo = beatData[currentAnimInfo.BeatIDs[currentBeatIndex]];
                if (animData[currentAnimationIndex]["timeToHit"].AsFloat - currentBeatInfo.PerfectStart <= hintTimer + HintTimeBeforeHit)
                {
                    //get beat infos
                    currentBeatIndex++;
                    GenerateHint(currentBeatInfo, currentAnimInfo.Mode);
                    //StartCoroutine(ShowRingIndicatorInSecs(currentBeatInfo, HintTimeBeforeHit));
                    if (currentBeatIndex >= currentAnimInfo.BeatIDs.Length)
                    {
                        currentAnimationIndex++;
                        currentBeatIndex = 0;
                    }
                }
            }
        }
    }
    void GenerateHint(BeatInfo beatTiming, BeatMode beatMode)
    {
        GameObject tmpGO = Instantiate(HintObject);
        tmpGO.transform.SetParent(transform);
        tmpGO.transform.localPosition = Vector3.zero;
        tmpGO.transform.localEulerAngles = Vector3.zero;
        tmpGO.transform.localScale = Vector3.one;
        // Set new speed
        HintObjectSpeed = backgroundLength / (HintTimeBeforeHit + beatTiming.OKStart + beatTiming.OKDuration);
        tmpGO.GetComponent<HintTrackControl>().StartMoving(beatTiming, this, beatMode);
        hintsQueue.Enqueue(tmpGO);
        if (!hasAreaPlaced)
        {
            PlaceOKAndPerfect(tmpGO.GetComponent<HintTrackControl>());
            hasAreaPlaced = true;
        }
    }

    //Place the OK and Perfect Area
    void PlaceOKAndPerfect(HintTrackControl _htCtrl)
    {

        // Display Area
        okPointer.transform.parent.GetComponent<UIFader>().FadeIn(0.2f);
        // New Position x
        float _okPosX = -(HintTimeBeforeHit + _htCtrl.beatTiming.OKStart) * _htCtrl.moveSpeed + OKArea.rectTransform.sizeDelta.x / 2;
        // Move Area
        OKArea.GetComponent<UIMover>().SimpleLocalPositionMover(OKArea.rectTransform.localPosition, new Vector3(_okPosX, OKArea.rectTransform.localPosition.y, 0.0f), fUIMoveTime);

        // Move Pointer
        Vector3 _OKLocalPos = new Vector3(_okPosX, okPointer.localPosition.y, okPointer.localPosition.z);
        okPointer.GetComponent<UIMover>().SimpleLocalPositionMover(okPointer.localPosition, _OKLocalPos, fUIMoveTime);

        // okPointer.localPosition = new Vector3(OKArea.rectTransform.localPosition.x, okPointer.localPosition.y, okPointer.localPosition.z);
        //OKArea.rectTransform.sizeDelta = new Vector2(/*beatTiming.OKDuration * HintObjectSpeed*/5, 100.0f);
        if (_htCtrl.beatTiming.IsCombo)
        {
            OKArea.transform.GetChild(0).gameObject.SetActive(true);
            PerfectArea.rectTransform.sizeDelta = Vector2.zero;
        }
        else
        {
            OKArea.transform.GetChild(0).gameObject.SetActive(false);
            // New Position
            float _perfectPosX = -(HintTimeBeforeHit + _htCtrl.beatTiming.PerfectStart) * HintObjectSpeed + PerfectArea.rectTransform.sizeDelta.x / 2;
            // Move area
            PerfectArea.GetComponent<UIMover>().SimpleLocalPositionMover(PerfectArea.rectTransform.localPosition, new Vector3(_perfectPosX, PerfectArea.rectTransform.localPosition.y, 0.0f), fUIMoveTime);

            // Move Pointer
            Vector3 _perfectLocalPos = new Vector3(_perfectPosX, perfectPointer.localPosition.y, perfectPointer.localPosition.z);
            perfectPointer.GetComponent<UIMover>().SimpleLocalPositionMover(perfectPointer.localPosition, _perfectLocalPos, fUIMoveTime);
            //PerfectArea.rectTransform.sizeDelta = new Vector2(/*beatTiming.PerfectDuration * HintObjectSpeed*/5, 100.0f);
        }

    }

    public new void RemoveFirstHint()
    {
        if (hintsQueue.Count != 0)
        {
            GameObject tmpGO = hintsQueue.Peek();
            //ShowResultAt(hitResult, tmpGO.transform);
            hintsQueue.Dequeue();
            if (hintsQueue.Count != 0)
            {
                //Change the perfect and OK area to the most current one
                tmpGO = hintsQueue.Peek();
                HintTrackControl tmpHTC = tmpGO.GetComponent<HintTrackControl>();
                PlaceOKAndPerfect(tmpHTC);
            }
            else
            {
                hasAreaPlaced = false;
            }
        }
    }

    public new void MatchButton(int ButID)
    {
        if (hintsQueue.Count != 0)
        {
            GameObject tmpGO = hintsQueue.Peek();
            tmpGO.GetComponent<HintTrackControl>().MatchButton(ButID);
        }
    }

}
