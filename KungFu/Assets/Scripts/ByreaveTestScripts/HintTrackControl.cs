using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintTrackControl : MonoBehaviour
{
    [SerializeField]
    GameObject[] ChildBodyParts;
    [SerializeField]
    Color OKColor = Color.green;
    [SerializeField]
    Color PerfectColor = Color.red;
    [SerializeField]
    Color NormalColor = Color.white;
    [SerializeField]
    Color ActiveColor = Color.yellow;

    BeatAnimation beatTiming;
    HitResult hintState = HitResult.Miss;
    HintGenerator hintGenerator;
    int[] buttonIDs;
    Dictionary<int, bool> matchedButtons;

    float timer = 0.0f;
    bool isMoving = false;

    float moveSpeed;
    float timeBeforeHit;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            timer += Time.deltaTime;
            transform.localPosition += Vector3.left * moveSpeed * Time.deltaTime;
            //transform.Translate(Vector3.left * MoveSpeed * Time.deltaTime);
            switch(hintState)
            {
                case HitResult.Miss:
                    if (timer >= beatTiming.OKStart + timeBeforeHit)
                        ChangeToOK();
                    break;
                case HitResult.Good:
                    if (timer >= beatTiming.PerfectStart + timeBeforeHit)
                        ChangeToPerfect();
                    break;
                //Missed
                case HitResult.Perfect:
                    if (timer >= beatTiming.PerfectStart + beatTiming.PerfectDuration + timeBeforeHit)
                    {
                        hintGenerator.RemoveFirstHint();
                        StartCoroutine(FadeOut());
                    }
                    break;
                default:
                    break;
            }
            
        }
    }

    void ChangeToOK()
    {
        for (int i = 0; i < buttonIDs.Length; ++ i)
        {
            if(buttonIDs[i] < ChildBodyParts.Length)
            {
                ChildBodyParts[buttonIDs[i]].GetComponent<Image>().color = OKColor;
            }
        }
        hintState = HitResult.Good;
    }

    void ChangeToPerfect()
    {
        for (int i = 0; i < buttonIDs.Length; ++i)
        {
            if (buttonIDs[i] < ChildBodyParts.Length)
            {
                ChildBodyParts[buttonIDs[i]].GetComponent<Image>().color = PerfectColor;
            }
        }
        hintState = HitResult.Perfect;
    }

    public void MatchButton(int butID)
    {
        foreach(var pair in matchedButtons)
        {
            if(butID == pair.Key && !pair.Value)
            {
                ChildBodyParts[butID].GetComponent<Image>().color = NormalColor;
            }
        }
        //check for if all is matched
        if(isAllMatched())
        {
            hintGenerator.RemoveFirstHint();
            StartCoroutine(FadeOut());
        }
    }

    bool isAllMatched()
    {
        foreach(bool b in matchedButtons.Values)
        {
            if (!b)
                return false;
        }
        return true;
    }
    
    void ActivateButtons(int[] nodes)
    {
        for(int i = 0; i < nodes.Length; ++ i)
        {
            ChildBodyParts[nodes[i]].SetActive(true);
            ChildBodyParts[nodes[i]].GetComponent<Image>().color = ActiveColor;
        }
    }

    public void StartMoving(BeatAnimation beatTime, int[] butIDs, HintGenerator hintGen)
    {
        matchedButtons = new Dictionary<int, bool>();
        beatTiming = beatTime;
        buttonIDs = butIDs;
        //initialize matched buttons
        foreach(int i in buttonIDs)
        {
            matchedButtons.Add(i, false);
        }
        hintGenerator = hintGen;

        moveSpeed = hintGen.HintObjectSpeed;
        timeBeforeHit = hintGen.HintTimeBeforeHit;

        isMoving = true;

        ActivateButtons(buttonIDs);
    }

    IEnumerator FadeOut(float time = 1.0f)
    {
        isMoving = false;
        while(ChildBodyParts[0].GetComponent<Image>().color.a >= 0)
        {
            for(int i = 0; i < ChildBodyParts.Length; ++ i)
            {
                Color tmp = ChildBodyParts[0].GetComponent<Image>().color;
                ChildBodyParts[0].GetComponent<Image>().color = new Color(tmp.r, tmp.g, tmp.b, tmp.a - Time.deltaTime / time);
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        Destroy(gameObject);
    }

    public BeatAnimation GetBeatTiming()
    {
        return beatTiming;
    }
}
