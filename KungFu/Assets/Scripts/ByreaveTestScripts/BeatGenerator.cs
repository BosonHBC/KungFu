using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
public class BeatGenerator : MonoBehaviour
{

    //The order in the children is important, should be the same with button mapping
    [SerializeField]
    GameObject Indicator;
    JSONNode BeatData;
    Dictionary<int, KeyCode> buttonMapping;
    //Timer for beats
    public float beatTimer = 0.0f;
    //current beat that is or will be played
    int currentBeatIndex = 0;
    //current beat's start time and perfect timing etc.
    BeatAnimation currentBeatInfo;
    //if a beat is played
    bool isInBeat = false;
    //the timer to get the result of the beat
    float reactionTimer = 0.0f;
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
        BeatData = MyGameInstance.instance.GetComponent<TestDataLoader>().GetBeatDataByName("Kungfu");
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
        
        if (BeatData[currentBeatIndex]["timeToHit"].AsFloat <= beatTimer)
        {
            //get beat infos
            activeButtons = DataUtility.GetIntArrayFromJSONNode(BeatData[currentBeatIndex]["buttonID"]);
            //No active buttons, song ends restart
            if (activeButtons == null)
                MyGameInstance.instance.RestartGame();
            currentBeatInfo = getBeatAnimationFromAnimID(BeatData[currentBeatIndex]["AnimationID"].AsInt);
            if (currentBeatInfo == null)
                Debug.Log("Error when getting beat info");
            
            //make sure matched buttons is clear
            matchedButtons.Clear();
            //Add active buttons to matched buttons for miss check
            foreach(int i in activeButtons)
            {
                matchedButtons.Add(i, false);
            }
            //Beat ends
            StartCoroutine(beatEndInSecs(activeButtons, currentBeatInfo.OKStart + currentBeatInfo.OKDuration));

            GetComponentInChildren<EnemyAnimationControl>().PlayAnim(currentBeatInfo.ID);
            currentBeatIndex++;
            isInBeat = true;
        }
        checkInputFromKeyboard(activeButtons, isInBeat);
        //checkInputFromArduino(activeButtons, isInBeat);
        if (isInBeat)
            reactionTimer += Time.deltaTime;
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
                HitResult hr = GetResultFromInput(reactionTimer);

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
        reactionTimer = 0.0f;
        missCheck();
        matchedButtons.Clear();
    }

    

    BeatAnimation getBeatAnimationFromAnimID(int AnimID)
    {
        return MyGameInstance.instance.GetComponent<TestDataLoader>().GetBeatAnimationDataByID(AnimID);
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

    

    HitResult GetResultFromInput(float ReactTime)
    {
        if(currentBeatInfo == null)
        {
            Debug.Log("BeatInfo error");
        }

        if (ReactTime >= currentBeatInfo.PerfectStart && ReactTime <= currentBeatInfo.PerfectStart + currentBeatInfo.PerfectDuration)
            return HitResult.Perfect;
        else if (ReactTime >= currentBeatInfo.OKStart && ReactTime <= currentBeatInfo.OKDuration)
            return HitResult.Good;
        else
            return HitResult.Miss;
    }
}
