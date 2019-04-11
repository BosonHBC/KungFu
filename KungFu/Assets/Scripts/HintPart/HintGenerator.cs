using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class HintGenerator : MonoBehaviour
{
    public float HintTimeBeforeHit = 3.0f;
    [HideInInspector]
    public float HintObjectSpeed = 400.0f;
    public GameObject HintObject;
    public GameObject ComboHintObject;

    [SerializeField]
    protected Image PerfectArea;
    protected Queue<GameObject> hintsQueue;
    protected bool hasAreaPlaced = false;

    //For beat timer
    BeatGenerator beatGenerator;
    #region
    protected int currentBeatIndex;
    protected JSONNode beatArray;
    protected Dictionary<int, BeatInfo> beatData;
    #endregion
    protected int currentHintIndex = 0;
    //Ring Indicator
    RingIndicatorControl ringIndicator;
    [SerializeField] protected float backgroundLength = 810;
    [HideInInspector]
    public UIBloomer blommer;
   // [SerializeField] protected RectTransform perfectPointer;

    // Start is called before the first frame update
    void Start()
    {
        //beatData = MyGameInstance.instance.GetComponent<DataLoader>().GetBeatInfos();
        //beatGenerator = FindObjectOfType<BeatGenerator>();
        hintsQueue = new Queue<GameObject>();
        currentBeatIndex = 0;
        blommer = PerfectArea.transform.GetComponentInChildren<UIBloomer>();
    }
    public void SetData(BeatGenerator _bg, string songName = "BattleGirl_H")
    {
        beatArray = MyGameInstance.instance.GetComponent<DataLoader>().GetBeatArrayByName(songName);
        beatData = MyGameInstance.instance.GetComponent<DataLoader>().GetBeatInfos();
        beatGenerator = _bg;
        ringIndicator = GetComponent<RingIndicatorControl>();
    }


    private void Update()
    {
        if (beatGenerator.bCanPlay)
        {
            //generate hints
            float hintTimer = beatGenerator.beatTimer;
            if (beatArray[currentBeatIndex]["BeatID"].AsInt != -1)
            {
                //get beat infos
                BeatInfo currentBeatInfo = beatData[beatArray[currentBeatIndex]["BeatID"].AsInt];
                if (beatArray[currentBeatIndex]["timeToHit"].AsFloat - currentBeatInfo.PerfectStart <= hintTimer + HintTimeBeforeHit)
                {
                    if (currentBeatInfo.Mode == BeatMode.Defend)
                    {
                        FightingManager.instance.SetFightMode(FightingManager.FightMode.Defense);
                    }
                    else
                        FightingManager.instance.SetFightMode(FightingManager.FightMode.Offense);
                    GenerateHint(currentBeatInfo);
                    if (currentBeatInfo.Mode == BeatMode.Defend)
                        StartCoroutine(ShowRingIndicatorInSecs(currentBeatInfo, HintTimeBeforeHit + currentBeatInfo.OKStart));
                    currentBeatIndex++;
                }
            }
        }
    }
    void GenerateHint(BeatInfo beatTiming)
    {
        // Dash before hit, some thing wrong
        if (FightingManager.instance.fightMode == FightingManager.FightMode.Defense)
            FightingManager.instance.characters[1].GetComponent<Character>().DashToOpponent(1, HintTimeBeforeHit);
        else if (FightingManager.instance.fightMode == FightingManager.FightMode.Offense)
            FightingManager.instance.characters[0].GetComponent<Character>().DashToOpponent(1, HintTimeBeforeHit);
        GameObject tmpGO;
        if (beatTiming.IsCombo)
            tmpGO = Instantiate(ComboHintObject);
        else
            tmpGO = Instantiate(HintObject);
        tmpGO.transform.SetParent(transform);
        tmpGO.transform.localPosition = Vector3.zero;
        tmpGO.transform.localEulerAngles = Vector3.zero;
        tmpGO.transform.localScale = Vector3.one;
       // UnityEditor.EditorApplication.isPaused = true;
        // Set new speed
        HintObjectSpeed = Mathf.Abs( PerfectArea.rectTransform.localPosition.x) / (HintTimeBeforeHit + beatTiming.PerfectStart + beatTiming.PerfectDuration/2f);
        tmpGO.GetComponent<HintTrackControl>().StartMoving(beatTiming, this);
        hintsQueue.Enqueue(tmpGO);
        if (!hasAreaPlaced)
        {
            //PlaceOKAndPerfect(tmpGO.GetComponent<HintTrackControl>());
            hasAreaPlaced = true;
        }
    }

    [SerializeField] protected float fUIMoveTime = 0.1f;
    //Place the OK and Perfect Area
    void PlaceOKAndPerfect(HintTrackControl _htCtrl)
    {
        // okPointer.localPosition = new Vector3(OKArea.rectTransform.localPosition.x, okPointer.localPosition.y, okPointer.localPosition.z);
        //OKArea.rectTransform.sizeDelta = new Vector2(/*beatTiming.OKDuration * HintObjectSpeed*/5, 100.0f);
        if (_htCtrl.beatTiming.IsCombo)
        {
            //OKArea.transform.GetChild(0).gameObject.SetActive(true);
           // perfectPointer.localScale = Vector2.zero;
            PerfectArea.rectTransform.localScale = Vector2.zero;
        }
        else
        {
            //OKArea.transform.GetChild(0).gameObject.SetActive(false);
          //  perfectPointer.localScale = Vector2.one;
            PerfectArea.rectTransform.localScale = Vector2.one;
            // New Position with width offset;
            float _perfectPosX = -(HintTimeBeforeHit + _htCtrl.beatTiming.PerfectStart + _htCtrl.beatTiming.PerfectDuration/2f) * HintObjectSpeed;
            Debug.Log("End Position of  Solid Hint: " + _perfectPosX);
            // Move area
            PerfectArea.GetComponent<UIMover>().SimpleLocalPositionMover(
                PerfectArea.rectTransform.localPosition,
                new Vector3(
                    _perfectPosX,
                    PerfectArea.rectTransform.localPosition.y,
                    0.0f),
                fUIMoveTime);
            // Move Pointer
            //Vector3 _perfectLocalPos = new Vector3(_perfectPosX + perfectPointer.sizeDelta.x / 2, perfectPointer.localPosition.y, perfectPointer.localPosition.z);
            //perfectPointer.GetComponent<UIMover>().SimpleLocalPositionMover(perfectPointer.localPosition, _perfectLocalPos, fUIMoveTime);
            //PerfectArea.rectTransform.sizeDelta = new Vector2(/*beatTiming.PerfectDuration * HintObjectSpeed*/5, 100.0f);
        }

    }

    public void RemoveFirstHint()
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
                //HintTrackControl tmpHTC = tmpGO.GetComponent<HintTrackControl>();
                //PlaceOKAndPerfect(tmpHTC);
            }
            else
            {
                hasAreaPlaced = false;
            }
        }
    }

    public void DirectlyRemoveFirstHint()
    {
        if (hintsQueue.Count != 0)
        {
            GameObject tmpGO = hintsQueue.Peek();
            tmpGO.GetComponent<HintTrackControl>().isMoving = false;
            tmpGO.GetComponent<UIDestroyer>().GoDie();
            //ShowResultAt(hitResult, tmpGO.transform);
            hintsQueue.Dequeue();
            if (hintsQueue.Count != 0)
            {
                //Change the perfect and OK area to the most current one
                tmpGO = hintsQueue.Peek();
               // HintTrackControl tmpHTC = tmpGO.GetComponent<HintTrackControl>();
               // PlaceOKAndPerfect(tmpHTC);
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
