using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class HintGenerator : MonoBehaviour
{
    public float HintTimeBeforeHit = 3.0f;
    [HideInInspector]
    public float HintObjectSpeed = 200.0f;
    public GameObject HintObject;
    public GameObject ComboHintObject;
    [SerializeField]
    protected Image OKArea;
    [SerializeField]
    protected Image PerfectArea;
    protected Queue<GameObject> hintsQueue;
    protected bool hasAreaPlaced = false;

    //For beat timer
    BeatGenerator beatGenerator;
    #region
    protected int currentAnimationIndex;
    protected int currentBeatIndex;
    protected JSONNode animData;
    protected Dictionary<int, AnimationInfo> animationData;
    protected Dictionary<int, BeatInfo> beatData;
    #endregion
    protected int currentHintIndex = 0;
    //Ring Indicator
    RingIndicatorControl ringIndicator;
    [SerializeField] protected float backgroundLength = 810;

    [SerializeField] protected RectTransform perfectPointer;
    [SerializeField] protected RectTransform okPointer;
    // Start is called before the first frame update
    void Start()
    {
        animData = MyGameInstance.instance.GetComponent<DataLoader>().GetAnimationArrayByName("BattleGirl");
        animationData = MyGameInstance.instance.GetComponent<DataLoader>().GetAnimationInfos();
        beatData = MyGameInstance.instance.GetComponent<DataLoader>().GetBeatInfos();
        beatGenerator = FindObjectOfType<BeatGenerator>();
        hintsQueue = new Queue<GameObject>();
        ringIndicator = GetComponent<RingIndicatorControl>();
    }
    private void Update()
    {
        if (beatGenerator.bCanPlay)
        {
            //generate hints
            float hintTimer = beatGenerator.beatTimer;
            if (animData[currentAnimationIndex]["AnimationID"].AsInt != -1)
            {
                AnimationInfo currentAnimInfo = animationData[animData[currentAnimationIndex]["AnimationID"].AsInt];
                //get beat infos
                BeatInfo currentBeatInfo = beatData[currentAnimInfo.BeatIDs[currentBeatIndex]];
                if (animData[currentAnimationIndex]["timeToHit"].AsFloat - currentBeatInfo.PerfectStart <= hintTimer + HintTimeBeforeHit)
                {
                    currentBeatIndex++;
                    if (currentAnimInfo.Mode == BeatMode.Defend)
                    {
                        FightingManager.instance.SetFightMode(FightingManager.FightMode.Defense);
                    }
                    else
                        FightingManager.instance.SetFightMode(FightingManager.FightMode.Offense);
                    GenerateHint(currentBeatInfo, currentAnimInfo.Mode);
                    if (currentAnimInfo.Mode == BeatMode.Defend)
                        StartCoroutine(ShowRingIndicatorInSecs(currentBeatInfo, HintTimeBeforeHit + currentBeatInfo.OKStart));
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

    [SerializeField] protected float fUIMoveTime = 0.1f;
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
            //OKArea.transform.GetChild(0).gameObject.SetActive(true);
            perfectPointer.localScale = Vector2.zero;
            PerfectArea.rectTransform.localScale = Vector2.zero;
        }
        else
        {
            //OKArea.transform.GetChild(0).gameObject.SetActive(false);
            perfectPointer.localScale = Vector2.one;
            PerfectArea.rectTransform.localScale = Vector2.one;
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
                HintTrackControl tmpHTC = tmpGO.GetComponent<HintTrackControl>();
                PlaceOKAndPerfect(tmpHTC);
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
                HintTrackControl tmpHTC = tmpGO.GetComponent<HintTrackControl>();
                PlaceOKAndPerfect(tmpHTC);
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
