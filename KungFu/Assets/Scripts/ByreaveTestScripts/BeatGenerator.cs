using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


public class BeatHitObject
{
    public float TimeToHit;
    public BeatInfo BeatTime;
    public Dictionary<int, bool> MatchedButtons;
    public int comboCount;
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
    //SFX Control
    //SFXControl sfxControl;

    bool animEvtsAdded = false;
    // Start is called before the first frame update
    void Start()
    {
        //BeatData = MyGameInstance.instance.GetComponent<TestDataLoader>().GetBeatDataByName("Kungfu");
        AnimationArray = MyGameInstance.instance.GetComponent<DataLoader>().GetAnimationArrayByName("Kungfu");
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
            {7, KeyCode.O }
        };
        beatQueue = new Queue<BeatHitObject>();
        //indicatorControl = Indicator.GetComponent<IndicatorControl>();
        hintGenerator = FindObjectOfType<HintGenerator>();
        resultControl = FindObjectOfType<ResultControl>();
        //sfxControl = GetComponent<SFXControl>();
    }

    public void SetData(Transform _enemy, HintGenerator _generator, ResultControl _control)
    {
        songPlaySource = _enemy.GetComponent<AudioSource>();
        enemyAnimCtrl = _enemy.GetComponent<EnemyAnimationControl>();
        hintGenerator = _generator;
        resultControl = _control;
        
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
        if (AnimationArray[currentAnimationIndex]["AnimationID"].AsInt == -1)
        {
            bCanPlay = false;
            Debug.Log("Song ended");
        }
        AnimationInfo currentAnimInfo = animationData[AnimationArray[currentAnimationIndex]["AnimationID"].AsInt];
        BeatInfo currentBeatInfo = beatData[currentAnimInfo.BeatIDs[currentBeatIndex]];
        if (currentBeatInfo == null)
            Debug.Log("Error when getting beat info");
        if (AnimationArray[currentAnimationIndex]["timeToHit"].AsFloat - currentBeatInfo.OKStart + currentBeatInfo.PerfectStart <= beatTimer)
        {
            if (!animEvtsAdded)
            {
                enemyAnimCtrl.AddSlowDownEvent(currentAnimInfo);
                animEvtsAdded = true;
            }
            enemyAnimCtrl.PlayAnim(currentAnimInfo.AnimationID);

            if (currentBeatInfo.OKStart + AnimationArray[currentAnimationIndex]["timeToHit"].AsFloat <= beatTimer)
            {
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
                        MatchedButtons = matchedButtons
                    }
                );

                //Beat ends
                StartCoroutine(beatEndInSecs(matchedButtons, currentBeatInfo.OKStart + currentBeatInfo.OKDuration));

                currentBeatIndex++;
                if (currentBeatIndex >= currentAnimInfo.BeatIDs.Length)
                {
                    currentAnimationIndex++;
                    currentBeatIndex = 0;
                    animEvtsAdded = false;
                }
            }
        }
        checkInputFromKeyboard();
        //checkInputFromArduino(activeButtons, isInBeat);
        if (bCanPlay)
            beatTimer += Time.deltaTime;
    }

    //there is a match hit
    void matchButton(int buttonID, Dictionary<int, bool> matchedButtons)
    {
        if (matchedButtons.ContainsKey(buttonID))
        {
            //if is not already matched
            if (!matchedButtons[buttonID])
            {
                //sfxControl.PlayRandomMatchSFX();
                HitResult hr = GetResultFromInput();

                hintGenerator.MatchButton(buttonID);
                //indicatorControl.MatchButton(buttonID);
                //we can calculate the reacting time to give different scores (as a parameter to Score() function) here
                if (hr != HitResult.Miss)
                    MyGameInstance.instance.Score();
                else
                    MyGameInstance.instance.Miss(1);
                //indicatorControl.ShowResultAt(buttonID, hr);
                resultControl.ShowResult(hr);
                matchedButtons[buttonID] = true;

                //if all buttons are hit, dequeue
                if (DataUtility.DictionaryAllTrue(matchedButtons))
                {
                    beatQueue.Dequeue();
                }
            }
        }
        else
            Debug.Log("No such button in matchedButtons!!!!!");
    }

    void missCheck(Dictionary<int, bool> matchedButtons)
    {

        //dequeue
        if (beatQueue.Count != 0 && matchedButtons == beatQueue.Peek().MatchedButtons)
        {
            if (beatQueue.Peek().BeatTime.IsCombo)
            {
                resultControl.ShowCombo(beatQueue.Peek().comboCount);
            }
            else
            {
                foreach (var matched in matchedButtons)
                {
                    if (!matched.Value)
                    {
                        MyGameInstance.instance.Miss(1);
                        //show miss image
                        resultControl.ShowResult(HitResult.Miss);
                        //MyGameInstance.instance.ShowResultAt(ChildBodyParts[matched.Key].transform, HitResult.Miss);
                        //sfxControl.PlayRandomMissSFX();
                    }
                }
            }
            beatQueue.Dequeue();
        }
    }

    //end of this beat, reset all, can be improved
    //Improved a little, need to add the check if the next beat comes before this beat ends
    IEnumerator beatEndInSecs(Dictionary<int, bool> matchedButtons, float delay)
    {
        yield return new WaitForSeconds(delay);
        //indicatorControl.DeactivateButtons(ButtonIDs);
        missCheck(matchedButtons);
        matchedButtons.Clear();
    }



    void checkInputFromKeyboard()
    {
        if (beatQueue.Count != 0)
        {
            var butInfo = beatQueue.Peek();
            if (beatTimer >= butInfo.TimeToHit + butInfo.BeatTime.OKStart && beatTimer <= butInfo.TimeToHit + butInfo.BeatTime.OKDuration)
            {
                foreach (var k in buttonMapping)
                {
                    //the buttons to be pressed in this beat
                    if (DataUtility.HasElement(butInfo.BeatTime.ButtonIDs, k.Key))
                    {
                        if (Input.GetKeyDown(k.Value))
                        {
                            if (butInfo.BeatTime.IsCombo)
                            {
                                butInfo.comboCount++;
                                resultControl.ShowCombo(beatQueue.Peek().comboCount);
                            }
                            else
                                matchButton(k.Key, butInfo.MatchedButtons);
                        }
                    }
                    //other buttons
                    else
                    {
                        //if (Input.GetKeyDown(k.Value))
                        //    indicatorControl.HitButton(k.Key);
                        //else if (Input.GetKeyUp(k.Value))
                        //    indicatorControl.DeactiveButton(k.Key);
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
            if (beatTimer >= butInfo.TimeToHit + butInfo.BeatTime.OKStart && beatTimer <= butInfo.TimeToHit + butInfo.BeatTime.OKDuration)
            {
                for (int i = 0; i < arduinoInput.Length; ++i)
                {
                    //More arduino inputs
                    if (i >= buttonMapping.Count)
                        continue;
                    //the buttons to be pressed in this beat
                    if (DataUtility.HasElement(butInfo.BeatTime.ButtonIDs, i))
                    {
                        if (arduinoInput[i])
                        {
                            matchButton(i, butInfo.MatchedButtons);
                        }
                    }
                    //other buttons
                    else
                    {
                        //if (arduinoInput[i])
                        //    indicatorControl.HitButton(i);
                        //else 
                        //    indicatorControl.DeactiveButton(i);
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
        BeatInfo currentBeatInfo = beatQueue.Peek().BeatTime;
        float ReactTime = beatTimer - beatQueue.Peek().TimeToHit;
        if (ReactTime >= currentBeatInfo.PerfectStart && ReactTime <= currentBeatInfo.PerfectStart + currentBeatInfo.PerfectDuration)
            return HitResult.Perfect;
        else if (ReactTime >= currentBeatInfo.OKStart && ReactTime <= currentBeatInfo.OKDuration)
            return HitResult.Good;
        else
            return HitResult.Miss;
    }
}
