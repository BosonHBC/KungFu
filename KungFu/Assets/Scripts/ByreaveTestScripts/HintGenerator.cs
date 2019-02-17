using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintGenerator : MonoBehaviour
{
    public float HintTimeBeforeHit = 3.0f;
    public float HintObjectSpeed = 200.0f;
    public GameObject HintObject;
    public GameObject ResultImageShow;
    [SerializeField]
    private Image OKArea;
    [SerializeField]
    private Image PerfectArea;
    Queue<GameObject> hintsQueue;

    bool hasAreaPlaced = false;
    // Start is called before the first frame update
    void Start()
    {
        hintsQueue = new Queue<GameObject>();
    }

    public void NewHint(BeatAnimation beatTiming, int[] ButtonIDs)
    {
        GameObject tmpGO = Instantiate(HintObject, transform.position, Quaternion.identity, transform);
        tmpGO.GetComponent<HintTrackControl>().StartMoving(HintObjectSpeed, beatTiming, ButtonIDs, HintTimeBeforeHit);
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
        OKArea.rectTransform.sizeDelta = new Vector2(beatTiming.OKDuration * 100.0f, 100.0f);
        PerfectArea.rectTransform.localPosition = new Vector3(-(HintTimeBeforeHit + beatTiming.PerfectStart) * HintObjectSpeed, 0.0f, 0.0f);
        PerfectArea.rectTransform.sizeDelta = new Vector2(beatTiming.PerfectDuration * 100.0f, 100.0f);
    }

    public void RemoveFirstHint()
    {
        if(hintsQueue.Count != 0)
        {
            GameObject tmpGO = hintsQueue.Peek();
            //ShowResultAt(hitResult, tmpGO.transform);
            hintsQueue.Dequeue();
            tmpGO.GetComponent<HintTrackControl>().RemoveDDR();
            if(hintsQueue.Count != 0)
            {
                //Change the perfect and OK area to the most current one
                tmpGO = hintsQueue.Peek();
                HintTrackControl tmpHTC = tmpGO.GetComponent<HintTrackControl>();
                PlaceOKAndPerfect(tmpHTC.beatTiming);
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

    void ShowResultAt(HitResult hitResult, Transform locTrans)
    {
        GameObject ri = Instantiate(ResultImageShow, locTrans.position, transform.rotation);
        //scale a bit
        ri.transform.localScale = ri.transform.localScale / 2;
        ri.GetComponent<ResultImageControl>().ShowResult(hitResult);
    }

    public void ShowResultAtFirstHint(HitResult hr)
    {
        if (hintsQueue.Count != 0)
        {
            GameObject tmpGO = hintsQueue.Peek();
            ShowResultAt(hr, tmpGO.transform);
        }
    }
}
