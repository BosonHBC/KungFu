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
    [SerializeField]
    Image ComboOutline;
    [SerializeField]
    GameObject HitLineHint;
    [SerializeField]
    Transform TooEarlyOrLate;
    [SerializeField]
    GameObject TooEarly, TooLate;
    bool IsTooLate = false;
    Coroutine comboOutlineShow;
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
    bool isDying = false;
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
        if (isDying)
            rectTr.localPosition += Vector3.left * moveSpeed * Time.deltaTime;
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
                    if (timer >= beatTiming.OKStart + beatTiming.OKDuration + timeBeforeHit)
                    {
                        if (!isDying)
                        {
                            hintGenerator.RemoveFirstHint();
                            if (beatTiming.IsCombo)
                                StartCoroutine(ComboDie());
                            else
                                gameObject.GetComponent<UIDestroyer>().GoDie();
                            isDying = true;
                            isMoving = false;
                        }
                    }
                    else if (timer >= beatTiming.PerfectStart + timeBeforeHit && timer < beatTiming.PerfectStart + beatTiming.PerfectDuration + timeBeforeHit)
                        ChangeToPerfect();

                    break;
                //Missed
                case HitResult.Perfect:
                    if (timer >= beatTiming.PerfectStart + beatTiming.PerfectDuration + timeBeforeHit)
                    {
                        IsTooLate = true;
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
        if(beatTiming.IsCombo)
        {
            for (int i = 0; i < ChildBodyParts.Length; ++i)
                ChildBodyParts[i].GetComponent<Image>().color = OKColor;
        }
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
        // hintGenerator.blommer.Blink(0.1f, PerfectColor);
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
        if(!beatTiming.IsCombo)
        {
            foreach (var pair in matchedButtons)
            {
                if (butID == pair.Key && !pair.Value)
                {
                    ChildBodyParts[butID].GetComponent<Image>().color = NormalColor;
                    matchedButtons[butID] = true;

                    //Show Too early or late
                    if(hintState == HitResult.Good)
                    {
                        if (IsTooLate)
                            ShowTooLate();
                        else
                            ShowTooEarly();
                    }
                    break;
                }
            }
            //check for if all is matched
            if (isAllMatched())
            {
                hintGenerator.RemoveFirstHint();
                isMoving = false;
                gameObject.GetComponent<UIDestroyer>().GoDie();
                GenerateHitLine();
            }
        }
    }

    void GenerateHitLine()
    {
        var go = Instantiate(HitLineHint, transform.position, transform.rotation, transform.parent);
        go.GetComponent<Image>().color = hintState == HitResult.Good ? OKColor : PerfectColor;
        go.GetComponent<Image>().CrossFadeAlpha(0.0f, 0.7f, false);
        Destroy(go, 0.7f);
    }
    void ShowTooLate()
    {
        var go = Instantiate(TooLate, TooEarlyOrLate.transform.position, Quaternion.identity, transform.parent);
        go.GetComponent<Image>().CrossFadeAlpha(0.0f, 1.0f, false);
        Destroy(go, 1.0f);
    }
    void ShowTooEarly()
    {
        var go = Instantiate(TooEarly, TooEarlyOrLate.transform.position, Quaternion.identity, transform.parent);
        go.GetComponent<Image>().CrossFadeAlpha(0.0f, 1.0f, false);
        Destroy(go, 1.0f);
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
        for (int i = 0; i < ChildBodyParts.Length; ++i)
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
        else
        {
            ActivateAllButtons();
            GetComponent<RectTransform>().sizeDelta.Set(beatTime.PerfectDuration * moveSpeed, 88);
            ComboOutline.GetComponent<RectTransform>().sizeDelta.Set(beatTime.PerfectDuration * moveSpeed, 88);
        }
    }

    public BeatInfo GetBeatTiming()
    {
        return beatTiming;
    }

    public void ChangeColor()
    {
        GetComponent<Image>().color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        if (comboOutlineShow != null)
            StopCoroutine(comboOutlineShow);
        comboOutlineShow = StartCoroutine(ComboOutlineShow(GetComponent<Image>().color));
    }

    IEnumerator ComboOutlineShow(Color newColor)
    {
        ComboOutline.color = newColor;
        ComboOutline.GetComponent<RectTransform>().localScale = Vector3.one;
        ComboOutline.canvasRenderer.SetAlpha(1.0f);
        var wait = new WaitForEndOfFrame();
        Vector3 step = new Vector3(Time.deltaTime, Time.deltaTime, 0.0f);
        ComboOutline.CrossFadeAlpha(0.0f, 0.5f, false);
        while(ComboOutline.GetComponent<RectTransform>().localScale.x <= 1.5f)
        {
            ComboOutline.GetComponent<RectTransform>().localScale += step;
            yield return wait;
        }
    }
    IEnumerator ComboDie()
    {
        GetComponent<Image>().CrossFadeAlpha(0.0f, 0.3f, false);
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
}
