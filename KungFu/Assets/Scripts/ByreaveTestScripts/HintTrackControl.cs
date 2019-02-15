using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintTrackControl : MonoBehaviour
{
    List<GameObject> ChildBodyParts;
    [SerializeField]
    Color OKColor;
    [SerializeField]
    Color PerfectColor;

    enum HintState
    {
        Perfect,
        OK,
        Normal
    }
    float OKStart, OKDuration, PerfectStart, PerfectDuration;
    float MoveSpeed, timer = 0.0f;
    bool isMoving = false;
    int[] buttonIDs;
    HintState hintState = HintState.Normal;
    // Start is called before the first frame update
    void Start()
    {
        ChildBodyParts = new List<GameObject>();
        for(int i = 0; i < transform.childCount; ++ i)
        {
            ChildBodyParts.Add(transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            timer += Time.deltaTime;
            transform.Translate(Vector3.left * MoveSpeed * Time.deltaTime);
            switch(hintState)
            {
                case HintState.Normal:
                    if (timer >= OKStart)
                        ChangeToOK(buttonIDs);
                    break;
                case HintState.OK:
                    if (timer >= PerfectStart)
                        ChangeToPerfect(buttonIDs);
                    break;
                case HintState.Perfect:
                    if (timer >= PerfectStart + PerfectDuration)
                        ChangeBack(buttonIDs);
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
            if(nodes[i] < ChildBodyParts.Count)
            {
                ChildBodyParts[nodes[i]].GetComponent<Image>().color = OKColor;
            }
        }
        hintState = HintState.OK;
    }

    void ChangeToPerfect(int[] nodes)
    {
        for (int i = 0; i < nodes.Length; ++i)
        {
            if (nodes[i] < ChildBodyParts.Count)
            {
                ChildBodyParts[nodes[i]].GetComponent<Image>().color = PerfectColor;
            }
        }
        hintState = HintState.Perfect;
    }

    void ChangeBack(int[] nodes)
    {
        for (int i = 0; i < nodes.Length; ++i)
        {
            if (nodes[i] < ChildBodyParts.Count)
            {
                ChildBodyParts[nodes[i]].GetComponent<Image>().color = Color.white;
            }
        }
        hintState = HintState.Normal;
    }

    void ActivateButtons(int[] nodes)
    {
        for(int i = 0; i < nodes.Length; ++ i)
        {
            ChildBodyParts[i].SetActive(true);
        }
    }

    public void StartMoving(float speed, float okStart, float okDuration, float perStart, float perDuration, int[] butIDs)
    {
        MoveSpeed = speed;
        OKStart = okStart;
        OKDuration = okDuration;
        PerfectStart = perStart;
        PerfectDuration = perDuration;
        buttonIDs = butIDs;

        isMoving = true;
        ActivateButtons(buttonIDs);
    }

    IEnumerator FadeOut(float time = 1.0f)
    {
        while(ChildBodyParts[0].GetComponent<Image>().color)
        yield return new WaitForSeconds(Time.deltaTime);
    }
}
