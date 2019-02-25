using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class HintGenerator : MonoBehaviour
{
    public float HintTimeBeforeHit = 3.0f;
    public float HintObjectSpeed = 200.0f;
    public GameObject HintObject;
    [SerializeField]
    private Image OKArea;
    [SerializeField]
    private Image PerfectArea;
    Queue<GameObject> hintsQueue;
    bool hasAreaPlaced = false;

    //For beat timer
    BeatGenerator beatGenerator;
    #region
    int currentAnimationIndex;
    int currentBeatIndex;
    JSONNode animData;
    Dictionary<int, AnimationInfo> animationData;
    Dictionary<int, BeatInfo> beatData;
    #endregion
    int currentHintIndex = 0;
    //Ring Indicator
    RingIndicatorControl ringIndicator;
    // Start is called before the first frame update
    void Start()
    {
        animData = MyGameInstance.instance.GetComponent<TestDataLoader>().GetAnimationArrayByName("Kungfu");
        animationData = MyGameInstance.instance.GetComponent<TestDataLoader>().GetAnimationInfos();
        beatData = MyGameInstance.instance.GetComponent<TestDataLoader>().GetBeatInfos();
        beatGenerator = FindObjectOfType<BeatGenerator>();
        hintsQueue = new Queue<GameObject>();
        ringIndicator = GetComponent<RingIndicatorControl>();
    }
    private void Update()
    {
        //generate hints
        float hintTimer = beatGenerator.beatTimer;
        AnimationInfo currentAnimInfo = animationData[animData[currentAnimationIndex]["AnimationID"].AsInt];
        BeatInfo currentBeatInfo = beatData[currentAnimInfo.BeatIDs[currentBeatIndex]];
        if (currentBeatInfo.OKStart + animData[currentAnimationIndex]["timeToHit"].AsFloat <= hintTimer + HintTimeBeforeHit)
        {
            //get beat infos
            currentBeatIndex++;
            GenerateHint(currentBeatInfo);
            StartCoroutine(ShowRingIndicatorInSecs(currentBeatInfo, HintTimeBeforeHit));
            if (currentBeatIndex >= currentAnimInfo.BeatIDs.Length)
            {
                currentAnimationIndex++;
                currentBeatIndex = 0;
            }
        }
    }
    void GenerateHint(BeatInfo beatTiming)
    {
        GameObject tmpGO = Instantiate(HintObject, transform.position, Quaternion.identity, transform);
        tmpGO.GetComponent<HintTrackControl>().StartMoving(beatTiming, this);
        hintsQueue.Enqueue(tmpGO);
        if(!hasAreaPlaced)
        {
            PlaceOKAndPerfect(beatTiming);
            hasAreaPlaced = true;
        }
    }

    //Place the OK and Perfect Area
    void PlaceOKAndPerfect(BeatInfo beatTiming)
    {
        OKArea.rectTransform.localPosition = new Vector3(-(HintTimeBeforeHit + beatTiming.OKStart) * HintObjectSpeed, 0.0f, 0.0f);
        OKArea.rectTransform.sizeDelta = new Vector2(beatTiming.OKDuration * HintObjectSpeed, 100.0f);
        PerfectArea.rectTransform.localPosition = new Vector3(-(HintTimeBeforeHit + beatTiming.PerfectStart) * HintObjectSpeed, 0.0f, 0.0f);
        PerfectArea.rectTransform.sizeDelta = new Vector2(beatTiming.PerfectDuration * HintObjectSpeed, 100.0f);
    }

    public void RemoveFirstHint()
    {
        if(hintsQueue.Count != 0)
        {
            GameObject tmpGO = hintsQueue.Peek();
            //ShowResultAt(hitResult, tmpGO.transform);
            hintsQueue.Dequeue();
            if(hintsQueue.Count != 0)
            {
                //Change the perfect and OK area to the most current one
                tmpGO = hintsQueue.Peek();
                HintTrackControl tmpHTC = tmpGO.GetComponent<HintTrackControl>();
                PlaceOKAndPerfect(tmpHTC.GetBeatTiming());
            }
            else
            {
                hasAreaPlaced = false;
            }
        }
    }

    public void MatchButton(int ButID)
    {
        if (hintsQueue.Count != 0)
        {
            GameObject tmpGO = hintsQueue.Peek();
            tmpGO.GetComponent<HintTrackControl>().MatchButton(ButID);
        }
    }

    IEnumerator ShowRingIndicatorInSecs(BeatInfo ba, float secs)
    {
        yield return new WaitForSeconds(secs);
        ringIndicator.ShowRingIndicator(ba);
    }
}
