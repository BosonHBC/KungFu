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
    [HideInInspector]
    public BeatInfo beatTiming;
    BeatMode beatMode;
    HitResult hintState = HitResult.Miss;
    HintGenerator hintGenerator;
    int[] buttonIDs;
    Dictionary<int, bool> matchedButtons;

    float timer = 0.0f;
    [HideInInspector]
    public bool isMoving = false;
    [HideInInspector]
    public float moveSpeed;
    float timeBeforeHit;
    RectTransform rectTr;
    private void Start()
    {
        rectTr = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            timer += Time.deltaTime;
            rectTr.localPosition += Vector3.left * moveSpeed * Time.deltaTime;
            //transform.Translate(Vector3.left * MoveSpeed * Time.deltaTime);
            switch (hintState)
            {
                case HitResult.Miss:
                    if (timer >= beatTiming.OKStart + timeBeforeHit)
                    {
                        ChangeToOK();
                    }
                    break;
                case HitResult.Good:
                    if(timer >= beatTiming.OKStart + beatTiming.OKDuration + timeBeforeHit)
                    {
                        hintGenerator.RemoveFirstHint();
                        isMoving = false;
                        gameObject.GetComponent<UIDestroyer>().GoDie();
                    }
                    else if (timer >= beatTiming.PerfectStart + timeBeforeHit && timer < beatTiming.PerfectStart + beatTiming.PerfectDuration + timeBeforeHit)
                        ChangeToPerfect();

                    break;
                //Missed
                case HitResult.Perfect:
                    if (timer >= beatTiming.PerfectStart + beatTiming.PerfectDuration + timeBeforeHit)
                    {
                        ChangeToOK();
                    }
                    break;
                default:
                    break;
            }

        }
    }

    void ChangeToOK()
    {
       // Debug.Log("Ok moving hint position: " + rectTr.localPosition.x);

      //  UnityEditor.EditorApplication.isPaused = true;
        //if (beatMode == BeatMode.Attack)
        //{
            if (beatTiming.IsCombo)
                gameObject.GetComponent<Image>().color = OKColor;
        //    else
        //    {
        //        for (int i = 0; i < ChildBodyParts.Length; ++i)
        //            ChildBodyParts[i].GetComponent<Image>().color = OKColor;
        //    }
        //}
        else
        {
            for (int i = 0; i < buttonIDs.Length; ++i)
            {
                if (buttonIDs[i] < ChildBodyParts.Length)
                {
                    ChildBodyParts[buttonIDs[i]].GetComponent<Image>().color = OKColor;
                }
            }
        }

        hintState = HitResult.Good;
    }

    void ChangeToPerfect()
    {
      //  Debug.Log("Perfect moving hint position: " + rectTr.localPosition.x);
      //  UnityEditor.EditorApplication.isPaused = true;

        //if (beatMode == BeatMode.Attack && !beatTiming.IsCombo)
        //{
        //    for (int i = 0; i < ChildBodyParts.Length; ++i)
        //        ChildBodyParts[i].GetComponent<Image>().color = PerfectColor;
        //}
        //else 
        if (!beatTiming.IsCombo)
        {
            for (int i = 0; i < buttonIDs.Length; ++i)
            {
                if (buttonIDs[i] < ChildBodyParts.Length)
                {
                    ChildBodyParts[buttonIDs[i]].GetComponent<Image>().color = PerfectColor;
                }
            }
        }
        hintState = HitResult.Perfect;
    }

    public void MatchButton(int butID)
    {
        foreach (var pair in matchedButtons)
        {
            if (butID == pair.Key && !pair.Value)
            {
                ChildBodyParts[butID].GetComponent<Image>().color = NormalColor;
                matchedButtons[butID] = true;
                break;
            }
        }
        //check for if all is matched
        if (isAllMatched())
        {
            hintGenerator.RemoveFirstHint();
            isMoving = false;
            gameObject.GetComponent<UIDestroyer>().GoDie();
        }
    }

    bool isAllMatched()
    {
        foreach (bool b in matchedButtons.Values)
        {
            if (!b)
                return false;
        }
        return true;
    }

    void ActivateButtons(int[] nodes)
    {
        for (int i = 0; i < nodes.Length; ++i)
        {
            ChildBodyParts[nodes[i]].SetActive(true);
            ChildBodyParts[nodes[i]].GetComponent<Image>().color = ActiveColor;
        }
    }

    void ActivateAllButtons()
    {
        for(int i = 0; i < ChildBodyParts.Length; ++ i)
        {
            ChildBodyParts[i].SetActive(true);
            ChildBodyParts[i].GetComponent<Image>().color = ActiveColor;
        }
    }

    public void StartMoving(BeatInfo beatTime, HintGenerator hintGen)
    {
        matchedButtons = new Dictionary<int, bool>();
        beatMode = beatTime.Mode;

        beatTiming = beatTime;
        buttonIDs = beatTime.ButtonIDs;
        //initialize matched buttons
        foreach (int i in buttonIDs)
        {
            matchedButtons.Add(i, false);
        }
        hintGenerator = hintGen;

        moveSpeed = hintGen.HintObjectSpeed;
        timeBeforeHit = hintGen.HintTimeBeforeHit;


        isMoving = true;
        if (!beatTime.IsCombo)
            ActivateButtons(buttonIDs);
        //else if (beatMode == BeatMode.Attack && !beatTime.IsCombo)
        //    ActivateAllButtons();
    }

    public BeatInfo GetBeatTiming()
    {
        return beatTiming;
    }

}
