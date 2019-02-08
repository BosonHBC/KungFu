using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
public class BeatGenerator : MonoBehaviour
{
    public float ReactionTime = 1.0f;
    public Material activateMat;
    public Material deactivateMat;
    public Material matchMat;
    public Material hitMat;

    //The order in the children is important, should be the same with button mapping
    [SerializeField]
    private GameObject[] ChildBodyParts;

    JSONNode BeatData;
    
    Dictionary<int, KeyCode> buttonMapping;
    float timer = 0.0f;
    int currentBeatIndex = 0;
    bool isInBeat = false;
    float reactionTimer = 0.0f;
    int[] activeButtons;

    //tmp way to check which beat is matched
    Dictionary<int, bool> matchedButtons;
    // Start is called before the first frame update
    void Start()
    {
        BeatData = MyGameInstance.instance.GetComponent<TestDataLoader>().GetBeatDataByName("Kungfu");
        buttonMapping = new Dictionary<int, KeyCode>()
        {
            {0, KeyCode.Q },
            {1, KeyCode.A },
            {2, KeyCode.Z },
            {3, KeyCode.E },
            {4, KeyCode.D },
            {5, KeyCode.C },
            {6, KeyCode.W },
            {7, KeyCode.S }
        };
        matchedButtons = new Dictionary<int, bool>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (BeatData[currentBeatIndex]["timeToHit"].AsFloat <= timer)
        {
            //Debug.Log(BeatData[currentBeatIndex]["timeToHit"].AsFloat);
            activeButtons = getIntArrayFromJSONNode(BeatData[currentBeatIndex]["buttonID"]);
            matchedButtons.Clear();//make sure matched buttons is clear
            foreach(int i in activeButtons)
            {
                matchedButtons.Add(i, false);
            }
            activateButton(activeButtons);

            GetComponentInChildren<EnemyAnimationControl>().PlayAnim(BeatData[currentBeatIndex]["AnimationID"].AsInt);

            currentBeatIndex++;
            isInBeat = true;
            StartCoroutine( deactivateButtonInSecs(activeButtons, ReactionTime));
        }
        checkInput(activeButtons, isInBeat);
        if (isInBeat)
            reactionTimer += Time.deltaTime;
    }

    //hightlight the parts to hit
    void activateButton(int[] ButtonIDs)
    {
        foreach(int i in ButtonIDs)
        {
            ChildBodyParts[i].GetComponent<MeshRenderer>().material = activateMat;
        }
    }
    //there is a match hit
    void matchButton(int buttonID)
    {
        
        if(matchedButtons.ContainsKey(buttonID))
        {
            //if is not already matched
            if(!matchedButtons[buttonID])
            {
                ChildBodyParts[buttonID].GetComponent<MeshRenderer>().material = matchMat;
                //we can calculate the reacting time to give different scores (as a parameter to Score() function) here
                MyGameInstance.instance.Score();
                HitResult hr = GetComponentInChildren<EnemyAnimationControl>().CheckHit(BeatData[currentBeatIndex]["AnimationID"].AsInt, reactionTimer);
                MyGameInstance.instance.ShowResultAt(ChildBodyParts[buttonID].transform, hr);

                matchedButtons[buttonID] = true;
            }
        }
        else
            Debug.Log("No such button in matchedButtons!!!!!");
    }
    //there is a hit
    void hitButton(int buttonID)
    {
        ChildBodyParts[buttonID].GetComponent<MeshRenderer>().material = hitMat;
    }
    //hit released
    void deactivateButton(int buttonID)
    {
        ChildBodyParts[buttonID].GetComponent<MeshRenderer>().material = deactivateMat;
    }

    void missCheck()
    {
        foreach (var matched in matchedButtons)
        {
            if (!matched.Value)
            {
                MyGameInstance.instance.Miss(1);
                //show miss image
                MyGameInstance.instance.ShowResultAt(ChildBodyParts[matched.Key].transform, HitResult.Miss);
            }
        }
    }

    //end of this beat, reset all, can be improved
    IEnumerator deactivateButtonInSecs(int[] ButtonIDs, float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (int i in ButtonIDs)
        {
            ChildBodyParts[i].GetComponent<MeshRenderer>().material = deactivateMat;
        }
        isInBeat = false;
        reactionTimer = 0.0f;
        missCheck();
        matchedButtons.Clear();
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

    bool hasElement(int[] array, int element)
    {
        foreach(int i in array)
        {
            if (i == element)
                return true;
        }
        return false;
    }
    void checkInput(int [] aButtons, bool inBeat)
    {
        foreach(var k in buttonMapping)
        {
            if(!inBeat)
            {
                if (Input.GetKeyDown(k.Value))
                    hitButton(k.Key);
                else if (Input.GetKeyUp(k.Value))
                    deactivateButton(k.Key);
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
                    if (Input.GetKeyDown(k.Value))
                        hitButton(k.Key);
                    else if (Input.GetKeyUp(k.Value))
                        deactivateButton(k.Key);
                }
            }
        }
    }
}
