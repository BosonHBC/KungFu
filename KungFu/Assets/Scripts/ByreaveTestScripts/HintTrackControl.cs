using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintTrackControl : MonoBehaviour
{
    [SerializeField]
    GameObject[] ChildBodyParts;
    [SerializeField]
    Color OKColor;
    [SerializeField]
    Color PerfectColor;

    public BeatAnimation beatTiming;
    public HitResult hintState = HitResult.Miss;

    float MoveSpeed, timer = 0.0f;
    float TimeBeforeHit;
    bool isMoving = false;
    int[] buttonIDs;
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
            transform.localPosition += Vector3.left * MoveSpeed * Time.deltaTime;
            //transform.Translate(Vector3.left * MoveSpeed * Time.deltaTime);
            switch(hintState)
            {
                case HitResult.Miss:
                    if (timer >= beatTiming.OKStart + TimeBeforeHit)
                        ChangeToOK(buttonIDs);
                    break;
                case HitResult.Good:
                    if (timer >= beatTiming.PerfectStart + TimeBeforeHit)
                        ChangeToPerfect(buttonIDs);
                    break;
                default:
                    break;
            }
            
        }
    }

    void ChangeToOK(int[] nodes)
    {
        for (int i = 0; i < nodes.Length; ++ i)
        {
            if(nodes[i] < ChildBodyParts.Length)
            {
                ChildBodyParts[nodes[i]].GetComponent<Image>().color = OKColor;
            }
        }
        hintState = HitResult.Good;
    }

    void ChangeToPerfect(int[] nodes)
    {
        for (int i = 0; i < nodes.Length; ++i)
        {
            if (nodes[i] < ChildBodyParts.Length)
            {
                ChildBodyParts[nodes[i]].GetComponent<Image>().color = PerfectColor;
            }
        }
        hintState = HitResult.Perfect;
    }

    void ChangeBack(int[] nodes)
    {
        for (int i = 0; i < nodes.Length; ++i)
        {
            if (nodes[i] < ChildBodyParts.Length)
            {
                ChildBodyParts[nodes[i]].GetComponent<Image>().color = Color.white;
            }
        }
        hintState = HitResult.Miss;
    }

    public void MatchButton(int butID)
    {
        if(butID < ChildBodyParts.Length)
        {
            Debug.Log("asdsad");
            ChildBodyParts[butID].GetComponent<Image>().color = Color.white;
        }
    }
    void ActivateButtons(int[] nodes)
    {
        for(int i = 0; i < nodes.Length; ++ i)
        {
            ChildBodyParts[nodes[i]].SetActive(true);
        }
    }

    public void StartMoving(float speed, BeatAnimation beatTime, int[] butIDs, float timeBeforeBeat)
    {
        beatTiming = beatTime;
        MoveSpeed = speed;
        buttonIDs = butIDs;
        TimeBeforeHit = timeBeforeBeat;
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

    public void RemoveDDR(float fadingTime = 1.0f)
    {
        StartCoroutine(FadeOut(fadingTime));
    }
}
