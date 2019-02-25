using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
public class BeatGenerator : MonoBehaviour
{

    //The order in the children is important, should be the same with button mapping
    [SerializeField]
    GameObject Indicator;

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

    //current beat's start time and perfect timing etc.
    BeatTiming currentBeatTiming;
    //if a beat is played
    bool isInBeat = false;
    //the timer to get the result of the beat
    //Buttons in this beat that are supposed to be hit
    int[] activeButtons;
    //Hint Generator
    HintGenerator hintGenerator;
    //Result Control
    ResultControl resultControl;
    //SFX Control
    SFXControl sfxControl;
    //tmp way to check which beat is matched
    Dictionary<int, bool> matchedButtons;
    // Start is called before the first frame update
    void Start()
    {
        //BeatData = MyGameInstance.instance.GetComponent<TestDataLoader>().GetBeatDataByName("Kungfu");
        AnimationArray = MyGameInstance.instance.GetComponent<TestDataLoader>().GetAnimationArrayByName("Kungfu");
        animationData = MyGameInstance.instance.GetComponent<TestDataLoader>().GetAnimationInfos();
        beatData = MyGameInstance.instance.GetComponent<TestDataLoader>().GetBeatInfos();

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
        matchedButtons = new Dictionary<int, bool>();
        //indicatorControl = Indicator.GetComponent<IndicatorControl>();
        hintGenerator = Indicator.GetComponent<HintGenerator>();
        resultControl = FindObjectOfType<ResultControl>();
        sfxControl = GetComponent<SFXControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (AnimationArray[currentAnimationIndex]["timeToHit"].AsFloat <= beatTimer)
        {
            //song ends restart
            if (AnimationArray[currentAnimationIndex]["AnimationID"].AsInt == -1)
                MyGameInstance.instance.RestartGame();

            AnimationInfo currentAnimInfo = animationData[AnimationArray[currentAnimationIndex]["AnimationID"].AsInt];

            if(beatData[currentAnimInfo.BeatIDs[currentBeatIndex]].OKStart + AnimationArray[currentAnimationIndex]["timeToHit"].AsFloat <= beatTimer)
            {
                //get beat infos
                activeButtons = beatData[currentAnimInfo.BeatIDs[currentBeatIndex]].ButtonIDs;
                BeatInfo currentBeatInfo = beatData[currentAnimInfo.BeatIDs[currentBeatIndex]];


                if (currentBeatInfo == null)
                    Debug.Log("Error when getting beat info");

                //make sure matched buttons is clear
                matchedButtons.Clear();
                //Add active buttons to matched buttons for miss check
                foreach (int i in activeButtons)
                {
                    matchedButtons.Add(i, false);
                }
                //Beat ends
                StartCoroutine(beatEndInSecs(activeButtons, currentBeatInfo.OKStart + currentBeatInfo.OKDuration));

                GetComponentInChildren<EnemyAnimationControl>().PlayAnim(currentAnimInfo.AnimationID);
                currentBeatIndex++;
                if (currentBeatIndex >= currentAnimInfo.BeatIDs.Length)
                {
                    currentAnimationIndex++;
                    currentBeatIndex = 0;
                }
                isInBeat = true;
            }
            
        }
        checkInputFromKeyboard(activeButtons, isInBeat);
        //checkInputFromArduino(activeButtons, isInBeat);
        beatTimer += Time.deltaTime;
    }

    //there is a match hit
    void matchButton(int buttonID)
    {
        if(matchedButtons.ContainsKey(buttonID))
        {
            //if is not already matched
            if(!matchedButtons[buttonID])
            {
                sfxControl.PlayRandomMatchSFX();
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
            }
        }
        else
            Debug.Log("No such button in matchedButtons!!!!!");
    }

    void missCheck()
    {
        foreach (var matched in matchedButtons)
        {
            if (!matched.Value)
            {
                MyGameInstance.instance.Miss(1);
                //show miss image
                resultControl.ShowResult(HitResult.Miss); 
                //MyGameInstance.instance.ShowResultAt(ChildBodyParts[matched.Key].transform, HitResult.Miss);
                sfxControl.PlayRandomMissSFX();
            }
        }
    }

    //end of this beat, reset all, can be improved
    //Improved a little, need to add the check if the next beat comes before this beat ends
    IEnumerator beatEndInSecs(int[] ButtonIDs, float delay)
    {
        yield return new WaitForSeconds(delay);
        //indicatorControl.DeactivateButtons(ButtonIDs);
        isInBeat = false;
        missCheck();
        matchedButtons.Clear();
    }

    

    void checkInputFromKeyboard(int [] aButtons, bool inBeat)
    {
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    ToggleIndicator();
        //}
        foreach (var k in buttonMapping)
        {
            if(!inBeat)
            {
                //if (Input.GetKeyDown(k.Value))
                //    indicatorControl.HitButton(k.Key);
                //else if (Input.GetKeyUp(k.Value))
                //    indicatorControl.DeactiveButton(k.Key);
            }
            else
            {
                //the buttons to be pressed in this beat
                if (DataUtility.HasElement(aButtons, k.Key))
                {
                    if (Input.GetKeyDown(k.Value))
                    {
                        matchButton(k.Key);
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

    void checkInputFromArduino(int []aButtons, bool inBeat)
    {
        bool[] arduinoInput = MyGameInstance.instance.GetArduinoInput();
        for(int i = 0; i < arduinoInput.Length; ++ i)
        {
            //More arduino inputs
            if (i >= buttonMapping.Count)
                continue;
            if (!inBeat)
            {
                //if (arduinoInput[i])
                //    indicatorControl.HitButton(i);
                //else
                //    indicatorControl.DeactiveButton(i);
            }
            else
            {
                //the buttons to be pressed in this beat
                if (DataUtility.HasElement(aButtons, i))
                {
                    if (arduinoInput[i])
                    {
                        matchButton(i);
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
    //void ToggleIndicator()
    //{
    //    showIndicator = !showIndicator;
    //    Indicator.transform.localScale = showIndicator ? Vector3.one : Vector3.zero;
    //}

    

    HitResult GetResultFromInput()
    {
        BeatInfo currentBeatInfo = beatData[animationData[AnimationArray[currentAnimationIndex]["AnimationID"].AsInt].BeatIDs[currentBeatIndex]];
        float ReactTime = beatTimer - AnimationArray[currentAnimationIndex]["timeToHit"].AsFloat;

        if (ReactTime >= currentBeatInfo.PerfectStart && ReactTime <= currentBeatInfo.PerfectStart + currentBeatInfo.PerfectDuration)
            return HitResult.Perfect;
        else if (ReactTime >= currentBeatInfo.OKStart && ReactTime <= currentBeatInfo.OKDuration)
            return HitResult.Good;
        else
            return HitResult.Miss;
    }
}
