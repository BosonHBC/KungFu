using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


public class BeatHitObject
{
    public float TimeToHit;
    public int comboCount;
    public BeatInfo BeatTime;
    //public BeatMode beatMode;
    public Dictionary<int, bool> MatchedButtons;
}
public class BeatGenerator : MonoBehaviour
{
    //new Data structure
    #region
    Dictionary<int, BeatInfo> beatData;
    JSONNode BeatArray;

    //current beat that is or will be played
    int currentBeatIndex = 0;
    #endregion
    Dictionary<int, KeyCode> buttonMapping;


    //Timer for beats
    public float beatTimer = 0.0f;
    private float fSongLengthInSec;
    public bool bCanPlay;

    //for input handling
    Queue<BeatHitObject> beatQueue;

    //Hint Generator
    HintGenerator hintGenerator;
    //Result Control
    ResultControl resultControl;
    //SFX
    SFXControl sfxControl;
    AudioSource songPlaySource;
    [SerializeField]
    AudioClip[] songs;
    EnemyAnimationControl enemyAnimCtrl;
    PlayerAnimController playerAnimCtrl;
    LevelLoader levelLoader;


    bool animPlayed = false;
    // Start is called before the first frame update
    void Start()
    {
        //animationData = MyGameInstance.instance.GetComponent<DataLoader>().GetAnimationInfos();
        beatData = MyGameInstance.instance.GetComponent<DataLoader>().GetBeatInfos();

        buttonMapping = new Dictionary<int, KeyCode>()
        {
            {0, KeyCode.Y },
            {1, KeyCode.Q },
            {2, KeyCode.P },
            {3, KeyCode.G },
            {4, KeyCode.Z },
            {5, KeyCode.M },
            {6, KeyCode.B }
        };

        beatQueue = new Queue<BeatHitObject>();
        levelLoader = FindObjectOfType<LevelLoader>();
        
    }

    public void SetData(Transform _enemy, HintGenerator _generator, ResultControl _control, string songName = "BattleGirl_H")
    {
        songPlaySource = Camera.main.GetComponent<AudioSource>();
        sfxControl = _enemy.GetComponent<SFXControl>();
        enemyAnimCtrl = _enemy.GetComponent<EnemyAnimationControl>();
        hintGenerator = _generator;
        resultControl = _control;
        BeatArray = MyGameInstance.instance.GetComponent<DataLoader>().GetBeatArrayByName(songName);
        playerAnimCtrl = FightingManager.instance.characters[0].GetComponent<PlayerAnimController>();
        // Debug
        fSongLengthInSec = MyGameInstance.instance.GetComponent<DataLoader>().GetLenghOfSongByName(songName);
    }

    public void StartGenerateBeat()
    {
        songPlaySource.clip = songs[MyGameInstance.instance.SongIndex];
        songPlaySource.Play();
        bCanPlay = true;
    }
    // Update is called once per frame
    void Update()
    {
        //song ends restart
        if (bCanPlay && BeatArray[currentBeatIndex]["BeatID"].AsInt == -1 && BeatArray[currentBeatIndex]["timeToHit"].AsInt <= beatTimer)
        {
            bCanPlay = false;
            //levelLoader.LoadScene("");
            //show result here.
            FightingManager.instance.FightOver(1);

            Debug.Log("Song ended");
        }

        // SetTimer for Time bar control
        TimeBarControl.instance.SetPercentage(beatTimer / fSongLengthInSec);

        if (bCanPlay)
        {
            if (BeatArray[currentBeatIndex]["BeatID"].AsInt != -1)
            {
                BeatInfo currentBeatInfo = beatData[BeatArray[currentBeatIndex]["BeatID"].AsInt];
                if (currentBeatInfo == null)
                    Debug.Log("Error when getting beat info");
                float TimeToHitThisBeat = BeatArray[currentBeatIndex]["timeToHit"].AsFloat;
                if (TimeToHitThisBeat - currentBeatInfo.PerfectStart <= beatTimer)
                {
                    if (!animPlayed)
                    {
                        //Enemy only has attack animations
                        if (currentBeatInfo.Mode == BeatMode.Defend)
                        {
                            enemyAnimCtrl.PlayAnim(currentBeatInfo.ButtonIDs[0]);
                        }
                        animPlayed = true;
                    }

                    if (TimeToHitThisBeat - currentBeatInfo.PerfectStart + currentBeatInfo.OKStart <= beatTimer)
                    {
                        //Debug.Log("BeatOut");
                        //create a map of matched buttons for miss check
                        var matchedButtons = new Dictionary<int, bool>();
                        foreach (int i in currentBeatInfo.ButtonIDs)
                        {
                            matchedButtons.Add(i, false);
                        }
                        beatQueue.Enqueue(
                            new BeatHitObject()
                            {
                                TimeToHit = TimeToHitThisBeat,
                                BeatTime = currentBeatInfo,
                                MatchedButtons = matchedButtons,
                            }
                        );

                        //Beat ends
                        StartCoroutine(beatEndInSecs(beatQueue.Peek(), currentBeatInfo.OKDuration));

                        //check for next beat
                        currentBeatIndex++;
                        animPlayed = false;
                    }
                }
            }

            checkInputFromKeyboard();
            checkInputFromArduino();

            beatTimer += Time.deltaTime;
        }

    }

    //there is a match hit
    void matchButton(int buttonID, BeatHitObject beatHitObject)
    {
        //if (beatHitObject.BeatTime.Mode == BeatMode.Attack)
        //{
        //    HitResult hr = GetResultFromInput();

        //    if (hr != HitResult.Miss)
        //    {
        //        FightingManager.instance.FM_Score(hr, buttonID);
        //        resultControl.ShowResult(hr);
        //    }
        //    else
        //        Debug.Log("No beats in the queue!");//FightingManager.instance.FM_Miss(1);

        //    hintGenerator.DirectlyRemoveFirstHint();
        //    if (beatQueue.Count > 0)
        //        beatQueue.Dequeue();
        //}
        //else
        //{
        if (beatHitObject.MatchedButtons.ContainsKey(buttonID))
        {
            //if is not already matched
            if (!beatHitObject.MatchedButtons[buttonID])
            {
                //sfxControl.PlayRandomMatchSFX();
                HitResult hr = GetResultFromInput();

                hintGenerator.MatchButton(buttonID);
                //player animation goes here
                //indicatorControl.MatchButton(buttonID);
                //we can calculate the reacting time to give different scores (as a parameter to Score() function) here
                if (hr != HitResult.Miss)
                    FightingManager.instance.FM_Score(hr, buttonID);
                else
                {
                    Debug.Log("Match Button Bugged");
                    //FightingManager.instance.FM_Miss(1);
                }
                //Get JointID
                int index = DataUtility.IntArrayIndex(beatHitObject.BeatTime.ButtonIDs, buttonID);
                if (index == -1)
                    Debug.LogError("Button ID not in beat button id array");
                //indicatorControl.ShowResultAt(buttonID, hr);
                resultControl.ShowResult(hr, (hr == HitResult.Miss) ? 0 : beatHitObject.BeatTime.JointIDs[index]);
                beatHitObject.MatchedButtons[buttonID] = true;

                //if all buttons are hit, dequeue
                if (DataUtility.DictionaryAllTrue(beatHitObject.MatchedButtons))
                {
                    if (beatQueue.Count > 0)
                        beatQueue.Dequeue();
                }
            }
        }
        else
            Debug.Log("No such button in matchedButtons!!!!!");
        //}
    }

    void missCheck(BeatHitObject beatHitObject)
    {
        //dequeue
        if (beatQueue.Count != 0 && beatHitObject == beatQueue.Peek())
        {
            if (beatHitObject.BeatTime.IsCombo)
            {
                resultControl.ShowCombo(beatHitObject.comboCount);
            }
            else
            {
                foreach (var matched in beatHitObject.MatchedButtons)
                {
                    if (!matched.Value)
                    {
                        float[] knockbackDir = { beatHitObject.BeatTime.KnockBack_H, beatHitObject.BeatTime.KnockBack_V };
                        FightingManager.instance.FM_Miss(1, knockbackDir);
                        //show miss image
                        resultControl.ShowResult(HitResult.Miss);
                        sfxControl.PlayRandomImpactSFX();
                    }
                }
            }
            beatQueue.Dequeue();
        }
    }

    void mismatch(BeatHitObject beatHitObject, int keyIndex)
    {

        //player animation goes here
        //need to add transition from defense to knockback
        sfxControl.PlayRandomMismatchSFX();
        if (beatHitObject.BeatTime.Mode == BeatMode.Defend)
            playerAnimCtrl.PlayGuardAnimation(keyIndex);
        else
            playerAnimCtrl.PlayPlayerAttackAnimation(keyIndex);
        //hintGenerator.DirectlyRemoveFirstHint();
        //if (beatQueue.Count > 0)
        //    beatQueue.Dequeue();
        //resultControl.ShowResult(HitResult.Mismatch);
        //FightingManager.instance.FM_Miss(beatHitObject.MatchedButtons.Count);
    }

    //end of this beat, reset all, can be improved
    //Improved a little, need to add the check if the next beat comes before this beat ends
    IEnumerator beatEndInSecs(BeatHitObject beatHit, float delay)
    {
        yield return new WaitForSeconds(delay);
        //indicatorControl.DeactivateButtons(ButtonIDs);
        missCheck(beatHit);
        beatHit.MatchedButtons.Clear();
        //FightingManager.instance.SetFightMode(FightingManager.FightMode.Wait);
    }



    void checkInputFromKeyboard()
    {
        if (beatQueue.Count != 0)
        {
            var butInfo = beatQueue.Peek();
            //Debug.Log(beatTimer);
            foreach (var k in buttonMapping)
            {
                if (beatTimer >= butInfo.TimeToHit - butInfo.BeatTime.PerfectStart + butInfo.BeatTime.OKStart && beatTimer <= butInfo.TimeToHit - butInfo.BeatTime.PerfectStart + butInfo.BeatTime.OKStart + butInfo.BeatTime.OKDuration)
                {
                    if (butInfo.BeatTime.IsCombo)
                    {
                        if (Input.GetKeyDown(k.Value))
                        {
                            butInfo.comboCount++;
                            sfxControl.PlayRandomImpactSFX();
                            resultControl.ShowCombo(butInfo.comboCount);
                            hintGenerator.GetCurrentComboHTC().ChangeColor();
                            //playerAnimCtrl.PlayPlayerAttackAnimation(-2);
                            playerAnimCtrl.PlayComboAnimation(butInfo.BeatTime.OKDuration + butInfo.BeatTime.OKStart);
                            FightingManager.instance.FM_Score(HitResult.Combo, 0, true);
                        }
                    }
                    else
                    {
                        //the buttons to be pressed in this beat
                        if (DataUtility.HasElement(butInfo.BeatTime.ButtonIDs, k.Key))
                        {
                            if (Input.GetKeyDown(k.Value))
                            {
                                sfxControl.PlayRandomImpactSFX();
                                matchButton(k.Key, butInfo);
                            }
                        }
                        //other buttons
                        else
                        {
                            if (Input.GetKeyDown(k.Value))
                            {
                                mismatch(butInfo, k.Key);
                            }
                        }
                    }
                }
            }
        }

    }

    void checkInputFromArduino()
    {
        if (beatQueue.Count != 0)
        {
            bool[] arduinoInput = MyGameInstance.instance.GetArduinoInput();
            var butInfo = beatQueue.Peek();
            if (beatTimer >= butInfo.TimeToHit - butInfo.BeatTime.PerfectStart + butInfo.BeatTime.OKStart && beatTimer <= butInfo.TimeToHit - butInfo.BeatTime.PerfectStart + butInfo.BeatTime.OKStart + butInfo.BeatTime.OKDuration)
            {
                for (int i = 0; i < arduinoInput.Length; ++i)
                {
                    ////More arduino inputs
                    //if (i >= buttonMapping.Count)
                    //    continue;
                    if (butInfo.BeatTime.Mode == BeatMode.Attack)
                    {
                        if (butInfo.BeatTime.IsCombo && arduinoInput[i])
                        {
                            butInfo.comboCount++;
                            resultControl.ShowCombo(butInfo.comboCount);
                            sfxControl.PlayRandomImpactSFX();

                            hintGenerator.GetCurrentComboHTC().ChangeColor();

                            //playerAnimCtrl.PlayPlayerAttackAnimation(-2);
                            playerAnimCtrl.PlayComboAnimation(butInfo.BeatTime.OKDuration + butInfo.BeatTime.OKStart);
                            FightingManager.instance.FM_Score(HitResult.Combo, 0, true);
                        }
                        else if (arduinoInput[i] && DataUtility.HasElement(butInfo.BeatTime.ButtonIDs, i))
                        {
                            sfxControl.PlayRandomImpactSFX();
                            matchButton(i, butInfo);
                        }
                    }
                    else
                    {
                        if (DataUtility.HasElement(butInfo.BeatTime.ButtonIDs, i))
                        {
                            if (arduinoInput[i])
                            {
                                sfxControl.PlayRandomImpactSFX();
                                matchButton(i, butInfo);
                            }
                        }
                        //other buttons
                        else
                        {
                            if (arduinoInput[i])
                            {
                                mismatch(butInfo, i);
                            }
                        }
                    }
                }
            }
        }
    }
    //void ToggleIndicator()
    //{
    //    showIndicator = !showIndicator;
    //    Indicator.transform.localScale = showIndicator ? Vector3.one : Vector3.zero;
    //}



    HitResult GetResultFromInput()
    {
        if (beatQueue.Count != 0)
        {
            BeatInfo currentBeatInfo = beatQueue.Peek().BeatTime;
            float ReactTime = beatTimer - beatQueue.Peek().TimeToHit + currentBeatInfo.PerfectStart;
            if (ReactTime >= currentBeatInfo.PerfectStart && ReactTime <= currentBeatInfo.PerfectStart + currentBeatInfo.PerfectDuration)
                return HitResult.Perfect;
            else if (ReactTime >= currentBeatInfo.OKStart && ReactTime <= currentBeatInfo.OKStart + currentBeatInfo.OKDuration)
                return HitResult.Good;
            else
                return HitResult.Miss;
        }
        return HitResult.Miss;
    }
}
