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
    JSONNode beatData;
    int currentHintIndex = 0;
    //Ring Indicator
    RingIndicatorControl ringIndicator;
    // Start is called before the first frame update
    void Start()
    {
        beatData = MyGameInstance.instance.GetComponent<TestDataLoader>().GetBeatDataByName("Kungfu");
        beatGenerator = FindObjectOfType<BeatGenerator>();
        hintsQueue = new Queue<GameObject>();
        ringIndicator = GetComponent<RingIndicatorControl>();
    }
    private void Update()
    {
        //generate hints
        float hintTimer = beatGenerator.beatTimer;
        if (beatData[currentHintIndex]["timeToHit"].AsFloat <= hintTimer + HintTimeBeforeHit)
        {
            int[] activeButtons = DataUtility.GetIntArrayFromJSONNode(beatData[currentHintIndex]["buttonID"]);
            if (activeButtons == null)
                return;
            BeatAnimation ba = MyGameInstance.instance.GetComponent<TestDataLoader>().GetBeatAnimationDataByID(beatData[currentHintIndex]["AnimationID"].AsInt);
            GenerateHint(ba, activeButtons);
            StartCoroutine(ShowRingIndicatorInSecs(ba, HintTimeBeforeHit));
            currentHintIndex++;
        }
    }
    void GenerateHint(BeatAnimation beatTiming, int[] ButtonIDs)
    {
        GameObject tmpGO = Instantiate(HintObject, transform.position, Quaternion.identity, transform);
        tmpGO.GetComponent<HintTrackControl>().StartMoving(beatTiming, ButtonIDs, this);
        hintsQueue.Enqueue(tmpGO);
        if(!hasAreaPlaced)
        {
            PlaceOKAndPerfect(beatTiming);
            hasAreaPlaced = true;
        }
    }

    //Place the OK and Perfect Area
    void PlaceOKAndPerfect(BeatAnimation beatTiming)
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

    IEnumerator ShowRingIndicatorInSecs(BeatAnimation ba, float secs)
    {
        yield return new WaitForSeconds(secs);
        ringIndicator.ShowRingIndicator(ba);
    }
}
