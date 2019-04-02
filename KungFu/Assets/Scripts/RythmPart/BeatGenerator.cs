﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


public class BeatHitObject
{
    public float TimeToHit;
    public int comboCount;
    public BeatInfo BeatTime;
    public BeatMode beatMode;
    public Dictionary<int, bool> MatchedButtons;
}
public class BeatGenerator : MonoBehaviour
{
    //new Data structure
    #region
    Dictionary<int, AnimationInfo> animationData;
    Dictionary<int, BeatInfo> beatData;
    JSONNode AnimationArray;

    //current animation that is or will be played
    int currentAnimationIndex = 0;
    int currentBeatIndex = 0;
    #endregion
    Dictionary<int, KeyCode> buttonMapping;
    //Timer for beats
    public float beatTimer = 0.0f;
    public bool bCanPlay;

    //for input handling
    Queue<BeatHitObject> beatQueue;

    //Hint Generator
    HintGenerator hintGenerator;
    //Result Control
    ResultControl resultControl;
    AudioSource songPlaySource;
    EnemyAnimationControl enemyAnimCtrl;
    PlayerAnimController playerAnimCtrl;
    LevelLoader levelLoader;
    //SFX Control
    //SFXControl sfxControl;

    bool animEvtsAdded = false;
    bool animPlayed = false;
    // Start is called before the first frame update
    void Start()
    {
        animationData = MyGameInstance.instance.GetComponent<DataLoader>().GetAnimationInfos();
        beatData = MyGameInstance.instance.GetComponent<DataLoader>().GetBeatInfos();

        buttonMapping = new Dictionary<int, KeyCode>()
        {
            {0, KeyCode.Y },
            {1, KeyCode.Q },
            {2, KeyCode.S },
            {3, KeyCode.E },
            {4, KeyCode.N },
            {5, KeyCode.U },
            {6, KeyCode.K },
            {7, KeyCode.O },
            {8, KeyCode.G },
            {9, KeyCode.B },
            {10, KeyCode.V }
        };
        beatQueue = new Queue<BeatHitObject>();
        levelLoader = FindObjectOfType<LevelLoader>();
        //indicatorControl = Indicator.GetComponent<IndicatorControl>();
        //hintGenerator = FindObjectOfType<HintGenerator>();
        //resultControl = FindObjectOfType<ResultControl>();
        //sfxControl = GetComponent<SFXControl>();
    }

    public void SetData(Transform _enemy, HintGenerator _generator, ResultControl _control, string songName = "BattleGirl")
    {
        songPlaySource = _enemy.GetComponent<AudioSource>();
        enemyAnimCtrl = _enemy.GetComponent<EnemyAnimationControl>();
        hintGenerator = _generator;
        resultControl = _control;
        AnimationArray = MyGameInstance.instance.GetComponent<DataLoader>().GetAnimationArrayByName(songName);
        playerAnimCtrl = FightingManager.instance.characters[0].GetComponent<PlayerAnimController>();
        // Debug
        StartGenerateBeat();
    }

    public void StartGenerateBeat()
    {
        songPlaySource.Play();
        bCanPlay = true;
    }
    // Update is called once per frame
    void Update()
    {
        //song ends restart
        if (AnimationArray[currentAnimationIndex]["AnimationID"].AsInt == -1 && bCanPlay)
        {
            bCanPlay = false;
            //levelLoader.LoadScene("");
            //show result here.
            Debug.Log("Song ended");
        }
        if (bCanPlay)
        {
            AnimationInfo currentAnimInfo = animationData[AnimationArray[currentAnimationIndex]["AnimationID"].AsInt];
            BeatInfo currentBeatInfo = beatData[currentAnimInfo.BeatIDs[currentBeatIndex]];
            //beatData[currentAnimInfo.BeatIDs[currentBeatIndex]].BeatID = 200;
            // Debug.Log(currentBeatInfo.BeatID);
            if (currentBeatInfo == null)
                Debug.Log("Error when getting beat info");
            if (AnimationArray[currentAnimationIndex]["timeToHit"].AsFloat - currentBeatInfo.PerfectStart <= beatTimer)
            {
                if (!animEvtsAdded)
                {
                    //enemyAnimCtrl.AddSlowDownEvent(currentAnimInfo);
                    animEvtsAdded = true;
                }
                if (!animPlayed)
                {
                    //Enemy only has attack animations
                    if (currentAnimInfo.Mode == BeatMode.Defend)
                    {
                        enemyAnimCtrl.PlayAnim(currentAnimInfo.AnimationID);
                    }
                    animPlayed = true;
                    //Debug.Log(currentAnimInfo.Mode);
                }

                if (AnimationArray[currentAnimationIndex]["timeToHit"].AsFloat - currentBeatInfo.PerfectStart + currentBeatInfo.OKStart <= beatTimer)
                {
                    //UnityEditor.EditorApplication.isPaused = true;
                    //create a map of matched buttons for miss check
                    var matchedButtons = new Dictionary<int, bool>();
                    foreach (int i in currentBeatInfo.ButtonIDs)
                    {
                        matchedButtons.Add(i, false);
                    }
                    beatQueue.Enqueue(
                        new BeatHitObject()
                        {
                            TimeToHit = AnimationArray[currentAnimationIndex]["timeToHit"].AsFloat,
                            BeatTime = currentBeatInfo,
                            MatchedButtons = matchedButtons,
                            beatMode = currentAnimInfo.Mode
                        }
                    );

                    //Beat ends
                    StartCoroutine(beatEndInSecs(beatQueue.Peek(), currentBeatInfo.OKDuration));

                    currentBeatIndex++;
                    if (currentBeatIndex >= currentAnimInfo.BeatIDs.Length)
                    {
                        currentAnimationIndex++;
                        currentBeatIndex = 0;
                        animEvtsAdded = false;
                        animPlayed = false;
                    }
                }
            }
            checkInputFromKeyboard();
            //checkInputFromArduino();

            beatTimer += Time.deltaTime;
        }

    }

    //there is a match hit
    void matchButton(int buttonID, BeatHitObject beatHitObject)
    {
        if (beatHitObject.beatMode == BeatMode.Attack)
        {
            HitResult hr = GetResultFromInput();

            if (hr != HitResult.Miss)
                FightingManager.instance.FM_Score(hr, beatHitObject.BeatTime.BeatID);
            else
                FightingManager.instance.FM_Miss(1);

            //need to change
            resultControl.ShowResult(hr);
            hintGenerator.DirectlyRemoveFirstHint();
            if (beatQueue.Count > 0)
                beatQueue.Dequeue();
        }
        else
        {
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
                        FightingManager.instance.FM_Score(hr);
                    else
                    {
                        Debug.Log("asdsa");
                        FightingManager.instance.FM_Miss(1);
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
                        beatQueue.Dequeue();
                    }
                }
            }
            else
                Debug.Log("No such button in matchedButtons!!!!!");
        }
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
                        FightingManager.instance.FM_Miss(1);
                        //show miss image
                        resultControl.ShowResult(HitResult.Miss);
                        //sfxControl.PlayRandomMissSFX();
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
        playerAnimCtrl.PlayGuardAnimation(beatHitObject.BeatTime.BeatID);
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
                    if (butInfo.beatMode == BeatMode.Attack)
                    {
                        if (butInfo.BeatTime.IsCombo && Input.GetKeyDown(k.Value))
                        {
                            butInfo.comboCount++;
                            resultControl.ShowCombo(butInfo.comboCount);
                            //playerAnimCtrl.PlayPlayerAttackAnimation(-2);
                            playerAnimCtrl.PlayComboAnimation(butInfo.BeatTime.OKDuration + butInfo.BeatTime.OKStart);
                            FightingManager.instance.FM_Score(HitResult.Good, 0, true);
                        }
                        else if (Input.GetKeyDown(k.Value))
                        {
                            matchButton(k.Key, butInfo);
                        }
                    }
                    else
                    {
                        //the buttons to be pressed in this beat
                        if (DataUtility.HasElement(butInfo.BeatTime.ButtonIDs, k.Key))
                        {
                            if (Input.GetKeyDown(k.Value))
                            {
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
                    if(butInfo.beatMode == BeatMode.Attack)
                    {
                        if(butInfo.BeatTime.IsCombo && arduinoInput[i])
                        {
                            butInfo.comboCount++;
                            resultControl.ShowCombo(butInfo.comboCount);
                            //playerAnimCtrl.PlayPlayerAttackAnimation(-2);
                            playerAnimCtrl.PlayComboAnimation(butInfo.BeatTime.OKDuration + butInfo.BeatTime.OKStart);
                            FightingManager.instance.FM_Score(HitResult.Good, 0, true);
                        }
                        else if(arduinoInput[i])
                        {
                            matchButton(i, butInfo);
                        }
                    }
                    else
                    {
                        if (DataUtility.HasElement(butInfo.BeatTime.ButtonIDs, i))
                        {
                            if (arduinoInput[i])
                            {
                                matchButton(i, butInfo);
                            }
                        }
                        //other buttons
                        else
                        {
                            if(arduinoInput[i])
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