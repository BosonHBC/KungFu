using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
public class BeatGenerator : MonoBehaviour
{
    public float ReactionTime = 1.0f;

    public AudioClip[] MissSFXs;
    public AudioClip[] MatchSFXs;
    //The order in the children is important, should be the same with button mapping
    [SerializeField]
    GameObject Indicator;

    JSONNode BeatData;
    
    Dictionary<int, KeyCode> buttonMapping;
    float timer = 0.0f;
    int currentBeatIndex = 0;
    int currentHintIndex = 0;
    bool isInBeat = false;
    float reactionTimer = 0.0f;
    int[] activeButtons;
    bool showIndicator = true;
    //IndicatorControl indicatorControl;
    HintGenerator hintGenerator;
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
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        //hint indicator
        if (BeatData[currentHintIndex]["timeToHit"].AsFloat <= timer + hintGenerator.HintTimeBeforeHit)
        {
            int[] tmpButtons = getIntArrayFromJSONNode(BeatData[currentHintIndex]["buttonID"]);

            //No active buttons, song ends restart
            if (tmpButtons == null)
                MyGameInstance.instance.RestartGame();
            BeatAnimation ba = getBeatAnimationFromAnimID(BeatData[currentHintIndex]["AnimationID"].AsInt);
            hintGenerator.NewHint(ba, tmpButtons);
            currentHintIndex++;
        }
        if (BeatData[currentBeatIndex]["timeToHit"].AsFloat <= timer)
        {
            activeButtons = getIntArrayFromJSONNode(BeatData[currentBeatIndex]["buttonID"]);
            //No active buttons, song ends restart
            if (activeButtons == null)
                MyGameInstance.instance.RestartGame();
            matchedButtons.Clear();//make sure matched buttons is clear
            foreach(int i in activeButtons)
            {
                matchedButtons.Add(i, false);
            }
            //indicatorControl.ActivateButton(activeButtons);
            StartCoroutine(deactivateButtonInSecs(activeButtons, ReactionTime));

            GetComponentInChildren<EnemyAnimationControl>().PlayAnim(BeatData[currentBeatIndex]["AnimationID"].AsInt);
            currentBeatIndex++;
            isInBeat = true;
        }
        checkInputFromKeyboard(activeButtons, isInBeat);
        checkInputFromArduino(activeButtons, isInBeat);
        if (isInBeat)
            reactionTimer += Time.deltaTime;
    }

    //there is a match hit
    void matchButton(int buttonID)
    {
        
        if(matchedButtons.ContainsKey(buttonID))
        {
            //if is not already matched
            if(!matchedButtons[buttonID])
            {
                PlayRandomMatchSFX();
                hintGenerator.MatchButton(buttonID);
                //indicatorControl.MatchButton(buttonID);
                //we can calculate the reacting time to give different scores (as a parameter to Score() function) here
                MyGameInstance.instance.Score();
                HitResult hr = GetComponentInChildren<EnemyAnimationControl>().CheckHit(BeatData[currentBeatIndex]["AnimationID"].AsInt, reactionTimer);
                //indicatorControl.ShowResultAt(buttonID, hr);
                hintGenerator.ShowResultAtFirstHint(hr);
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
                hintGenerator.ShowResultAtFirstHint(HitResult.Miss); //ShowResultAt(matched.Key, HitResult.Miss);
                //MyGameInstance.instance.ShowResultAt(ChildBodyParts[matched.Key].transform, HitResult.Miss);
                PlayRandomMissSFX();
            }
        }
    }

    //end of this beat, reset all, can be improved
    IEnumerator deactivateButtonInSecs(int[] ButtonIDs, float delay)
    {
        yield return new WaitForSeconds(delay);
        //indicatorControl.DeactivateButtons(ButtonIDs);
        isInBeat = false;
        reactionTimer = 0.0f;
        missCheck();
        matchedButtons.Clear();
        hintGenerator.RemoveFirstHint();
    }

    int[] getIntArrayFromJSONNode(JSONNode node)
    {
        if (node.Count == 0)
            return null;
        int[] retArr = new int[node.Count];
        for(int i = 0; i < node.Count; ++ i)
        {
            retArr[i] = node[i].AsInt;
        }
        return retArr;
    }

    BeatAnimation getBeatAnimationFromAnimID(int AnimID)
    {
        return MyGameInstance.instance.GetComponent<TestDataLoader>().GetBeatAnimationDataByID(AnimID);
    }
    bool hasElement(int[] array, int element)
    {
        foreach(int i in array)
        {
            if (i == element)
                return true;
        }
        return false;
    }
    void checkInputFromKeyboard(int [] aButtons, bool inBeat)
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleIndicator();
        }
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
                if (hasElement(aButtons, k.Key))
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
                if (hasElement(aButtons, i))
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
    void ToggleIndicator()
    {
        showIndicator = !showIndicator;
        Indicator.transform.localScale = showIndicator ? Vector3.one : Vector3.zero;
    }

    void PlayRandomMissSFX()
    {
        GetComponent<AudioSource>().PlayOneShot(MissSFXs[Random.Range(0, MissSFXs.Length)]);
    }

    void PlayRandomMatchSFX()
    {
        GetComponent<AudioSource>().PlayOneShot(MatchSFXs[Random.Range(0, MatchSFXs.Length)]);
        
    }
}
