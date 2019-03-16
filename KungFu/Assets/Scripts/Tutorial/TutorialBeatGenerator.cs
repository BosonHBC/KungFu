using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class TutorialBeatGenerator : MonoBehaviour
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
    TutorialHintGenerator hintGenerator;
    //Result Control
    ResultControl resultControl;
    AudioSource songPlaySource;
    EnemyAnimationControl enemyAnimCtrl;
    LevelLoader levelLoader;
    //Tutorial indicator
    IndicatorControl indicatorControl;
    [SerializeField]
    TutorialControl tutorialControl;

    bool animEvtsAdded = false;
    bool animPlayed = false;
    // Start is called before the first frame update
    void Start()
    {
        AnimationArray = MyGameInstance.instance.GetComponent<DataLoader>().GetAnimationArrayByName("Tutorial");
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
            {10, KeyCode.S }
        };
        beatQueue = new Queue<BeatHitObject>();
        levelLoader = FindObjectOfType<LevelLoader>();
        hintGenerator = FindObjectOfType<TutorialHintGenerator>();
        indicatorControl = FindObjectOfType<IndicatorControl>();
        resultControl = FindObjectOfType<ResultControl>();
        songPlaySource = FindObjectOfType<AudioSource>();
        tutorialControl = FindObjectOfType<TutorialControl>();
        //sfxControl = GetComponent<SFXControl>();
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
            //levelLoader.LoadScene("");
            //show result here.
            Debug.Log("Song ended");
        }
        if (bCanPlay)
        {
            AnimationInfo currentAnimInfo = animationData[AnimationArray[currentAnimationIndex]["AnimationID"].AsInt];
            BeatInfo currentBeatInfo = beatData[currentAnimInfo.BeatIDs[currentBeatIndex]];
            beatData[currentAnimInfo.BeatIDs[currentBeatIndex]].BeatID = 200;
            // Debug.Log(currentBeatInfo.BeatID);
            if (currentBeatInfo == null)
                Debug.Log("Error when getting beat info");
            if (AnimationArray[currentAnimationIndex]["timeToHit"].AsFloat - currentBeatInfo.PerfectStart <= beatTimer)
            {
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
                            MatchedButtons = matchedButtons
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
        if (beatHitObject.MatchedButtons.ContainsKey(buttonID))
        {
            //if is not already matched
            if (!beatHitObject.MatchedButtons[buttonID])
            {
                //sfxControl.PlayRandomMatchSFX();
                HitResult hr = GetResultFromInput();

                hintGenerator.MatchButton(buttonID);

                //indicatorControl.MatchButton(buttonID);
                //we can calculate the reacting time to give different scores (as a parameter to Score() function) here
                if (hr != HitResult.Miss)
                {
                    MyGameInstance.instance.Score(hr);
                    tutorialControl.ShowBeatHit();
                }
                else
                    MyGameInstance.instance.Miss(1);
                //Get JointID
                int index = DataUtility.IntArrayIndex(beatHitObject.BeatTime.ButtonIDs, buttonID);
                if (index == -1)
                    Debug.LogError("Button ID not in beat button id array");
                resultControl.ShowTutorialResult(hr, indicatorControl.GetChildTransform(buttonID));
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

    void missCheck(BeatHitObject beatHitObject)
    {

        //dequeue
        if (beatQueue.Count != 0 && beatHitObject == beatQueue.Peek())
        {
            if (beatHitObject.BeatTime.IsCombo)
            {
                //resultControl.ShowCombo(beatHitObject.comboCount);
            }
            else
            {
                foreach (var matched in beatHitObject.MatchedButtons)
                {
                    if (!matched.Value)
                    {
                        resultControl.ShowTutorialResult(HitResult.Miss, indicatorControl.GetChildTransform(matched.Key));
                        MyGameInstance.instance.Miss(1);
                    }
                }
            }
            beatQueue.Dequeue();
        }
    }

    //end of this beat, reset all, can be improved
    //Improved a little, need to add the check if the next beat comes before this beat ends
    IEnumerator beatEndInSecs(BeatHitObject beatHit, float delay)
    {
        yield return new WaitForSeconds(delay);
        //indicatorControl.DeactivateButtons(ButtonIDs);
        missCheck(beatHit);
        beatHit.MatchedButtons.Clear();
    }



    void checkInputFromKeyboard()
    {
        if (beatQueue.Count != 0)
        {
            //Debug.Log(beatTimer);

            var butInfo = beatQueue.Peek();
            if (beatTimer >= butInfo.TimeToHit - butInfo.BeatTime.PerfectStart + butInfo.BeatTime.OKStart && beatTimer <= butInfo.TimeToHit - butInfo.BeatTime.PerfectStart + butInfo.BeatTime.OKStart + butInfo.BeatTime.OKDuration)
            {
                //Debug.Log(beatTimer);
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
                                //resultControl.ShowCombo(beatQueue.Peek().comboCount);
                            }
                            else
                                matchButton(k.Key, butInfo);
                        }
                    }
                }
            }
        }
        foreach (var k in buttonMapping)
        {
            if (Input.GetKeyDown(k.Value))
                indicatorControl.HitButton(k.Key);
            else if (Input.GetKeyUp(k.Value))
                indicatorControl.DeactiveButton(k.Key);
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
                    //More arduino inputs
                    if (i >= buttonMapping.Count)
                        continue;
                    //the buttons to be pressed in this beat
                    if (DataUtility.HasElement(butInfo.BeatTime.ButtonIDs, i))
                    {
                        if (arduinoInput[i])
                        {
                            if (butInfo.BeatTime.IsCombo)
                            {
                                butInfo.comboCount++;
                                //resultControl.ShowCombo(beatQueue.Peek().comboCount);
                            }
                            else
                                matchButton(i, beatQueue.Peek());
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
        float ReactTime = beatTimer - beatQueue.Peek().TimeToHit + currentBeatInfo.PerfectStart;

        if (ReactTime >= currentBeatInfo.PerfectStart && ReactTime <= currentBeatInfo.PerfectStart + currentBeatInfo.PerfectDuration)
            return HitResult.Perfect;
        else if (ReactTime >= currentBeatInfo.OKStart && ReactTime <= currentBeatInfo.OKStart + currentBeatInfo.OKDuration)
            return HitResult.Good;
        else
            return HitResult.Miss;
    }
}
