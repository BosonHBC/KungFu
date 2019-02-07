using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SongSelectControl : MonoBehaviour
{
    // public and Serilizable
    [SerializeField] GameObject songPannelPrefab;
    [SerializeField] UIFader downPointer;
    // private
    private RectTransform trRect;
    private HorizontalLayoutGroup horiLayout;
    private int fPannelWidth;
    private int fPannelXDelta;
    private List<SongPannel> pannels = new List<SongPannel>();
    private int targetSongIndex;
    private UIMover moveTo;
    private bool bCanMove = true;
    private bool bSelecting;
    private float fExpandWidth = 500;
    private float fUIMoveSpeed = 0.1f;
    private float fUIExpandTime = 0.5f;

    [Header("Debug")]
    [SerializeField] private int DebugSongCount = 3;
    void Start()
    {
        moveTo = GetComponent<UIMover>();
        trRect = GetComponent<RectTransform>();
        horiLayout = GetComponent<HorizontalLayoutGroup>();
        fPannelWidth = (int)songPannelPrefab.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite.rect.width;
        targetSongIndex = (DebugSongCount - 1) / 2;
        Debug.Log("CurrentIndex: " + targetSongIndex);

        // Create game object
        for (int i = 0; i < DebugSongCount; i++)
        {
            GameObject go = Instantiate(songPannelPrefab);

            go.name = "SongPannel_" + (DebugSongCount - 1 - i).ToString();
            // Transform
            go.transform.SetParent(transform);
            go.transform.SetSiblingIndex(1);
            RectTransform _trRect = go.GetComponent<RectTransform>();
            _trRect.localScale = Vector3.one;
            _trRect.localPosition = new Vector3(_trRect.position.x, _trRect.position.y, 0);
            // List
            pannels.Add(go.GetComponent<SongPannel>());
        }
        pannels.Reverse();

        // set transform width
        trRect.sizeDelta = new Vector2(trRect.sizeDelta.x + DebugSongCount * fPannelWidth, trRect.sizeDelta.y);

        Invoke("SetData", 0.1f);
       
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            NextSong();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            PreviousSong();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SelectSong();
        }
    }

    public void SelectSong()
    {
        if (!bSelecting)
        {
            bSelecting = true;
            StartCoroutine(ExpandTargetPannel(targetSongIndex, fExpandWidth, delegate { }, fUIExpandTime));
        }
    }

    void SetData()
    {
        bCanMove = true;
        fPannelXDelta = (int)(pannels[1].GetComponent<RectTransform>().anchoredPosition.x - pannels[0].GetComponent<RectTransform>().anchoredPosition.x);
        moveTo.SetRestriction(-DebugSongCount * fPannelXDelta / 2, DebugSongCount * fPannelXDelta / 2);
    }
    public void NextSong()
    {
        if (bCanMove)
        {
            bCanMove = false;

            if (targetSongIndex < pannels.Count - 1)
            {
                downPointer.FadeIn();
                if (targetSongIndex >= 0 && bSelecting)
                {
                    // fade in previous one
                    StartCoroutine(ExpandTargetPannel(targetSongIndex, -fExpandWidth, delegate { // next rect transform
                        RectTransform d_rect = pannels[targetSongIndex + 1].GetComponent<RectTransform>();
                        // final moving distance
                        float d_destPos = (int)Mathf.Abs(d_rect.localPosition.x + trRect.anchoredPosition.x);
                        moveTo.UIMoveToPosition(Vector3.left, d_destPos, fUIMoveSpeed, delegate { targetSongIndex++; Debug.Log("Current: " + targetSongIndex); bCanMove = true;/*SelectSong();*/ });
                    }, fUIExpandTime));
                    bSelecting = false;
                    return;
                    
                }
                // next rect transform
                RectTransform _rect = pannels[targetSongIndex + 1].GetComponent<RectTransform>();
                // final moving distance
                float destPos = (int)Mathf.Abs(_rect.localPosition.x + trRect.anchoredPosition.x);
                moveTo.UIMoveToPosition(Vector3.left, destPos, fUIMoveSpeed, delegate { targetSongIndex++; Debug.Log("Current: " + targetSongIndex); bCanMove = true;/*SelectSong();*/ });

            }
            else
            {
                if (bSelecting)
                {
                    bSelecting = false;
                    StartCoroutine(ExpandTargetPannel(pannels.Count - 1, -fExpandWidth, delegate {
                        downPointer.FadeOut();
                        moveTo.UIMoveToPosition(Vector3.left, 1000f, fUIMoveSpeed, delegate { Debug.Log("Go to right boundary"); bCanMove = true; });
                        targetSongIndex = pannels.Count;
                    }, fUIExpandTime));
                    return;
                }
                downPointer.FadeOut();
                moveTo.UIMoveToPosition(Vector3.left, 1000f, fUIMoveSpeed, delegate { Debug.Log("Go to right boundary"); bCanMove = true; });
                targetSongIndex = pannels.Count;
            }
        }
    }

    public void PreviousSong()
    {
        if (bCanMove)
        {
            bCanMove = false;
            if (targetSongIndex > 0)
            {
                downPointer.FadeIn();
                if (targetSongIndex >= 0 && bSelecting)
                {
                    // fade in previous one
                    StartCoroutine(ExpandTargetPannel(targetSongIndex, -fExpandWidth, delegate {
                        RectTransform d_rect = pannels[targetSongIndex - 1].GetComponent<RectTransform>();
                        float d_destPos = (int)Mathf.Abs(d_rect.localPosition.x + trRect.anchoredPosition.x);
                        moveTo.UIMoveToPosition(Vector3.right, d_destPos, fUIMoveSpeed, delegate { targetSongIndex--; Debug.Log("Current: " + targetSongIndex); bCanMove = true;/*SelectSong();*/ });

                    }, fUIExpandTime));
                    bSelecting = false;
                    return;
                }
                RectTransform _rect = pannels[targetSongIndex - 1].GetComponent<RectTransform>();
                float destPos = (int)Mathf.Abs(_rect.localPosition.x + trRect.anchoredPosition.x);
                moveTo.UIMoveToPosition(Vector3.right, destPos, fUIMoveSpeed, delegate { targetSongIndex--; Debug.Log("Current: " + targetSongIndex); bCanMove = true;/*SelectSong();*/ });
            }
            else
            {
                if (bSelecting)
                {
                    bSelecting = false;
                    StartCoroutine(ExpandTargetPannel(0, -fExpandWidth, delegate {
                        downPointer.FadeOut();
                        moveTo.UIMoveToPosition(Vector3.right, 1000f, fUIMoveSpeed, delegate { Debug.Log("Go to left boundary"); bCanMove = true; });
                        targetSongIndex = -1;
                    }, fUIExpandTime));
                    return;
                }
                downPointer.FadeOut();
                moveTo.UIMoveToPosition(Vector3.right, 1000f, fUIMoveSpeed, delegate { Debug.Log("Go to left boundary"); bCanMove = true; });
                targetSongIndex = -1;
            }
        }
    }

    IEnumerator ExpandTargetPannel(int _id, float _delta, UnityAction _onFinishExpand, float _fadeTime = 0.5f)
    {
        float _timeStartFade = Time.time;
        float _timeSinceStart = Time.time - _timeStartFade;
        float _lerpPercentage = _timeSinceStart / _fadeTime;

        float pannel_Start = pannels[_id].GetComponent<RectTransform>().sizeDelta.x;
        float pannel_Dest = pannel_Start + _delta;

        float parent_Start = trRect.sizeDelta.x;
        float parent_Dest = parent_Start + 2 * _delta;
        while (true)
        {
            _timeSinceStart = Time.time - _timeStartFade;
            _lerpPercentage = _timeSinceStart / _fadeTime;

            float pannel_Current = Mathf.Lerp(pannel_Start, pannel_Dest, _lerpPercentage);
            float parent_Current = Mathf.Lerp(parent_Start, parent_Dest, _lerpPercentage);

            // pannel width increase 200
            // parent size increase 400
            RectTransform _rect = pannels[_id].GetComponent<RectTransform>();
            _rect.sizeDelta = new Vector2(pannel_Current, _rect.sizeDelta.y);
            trRect.sizeDelta = new Vector2(parent_Current, trRect.sizeDelta.y);

            if (_lerpPercentage >= 1) break;


            yield return new WaitForEndOfFrame();
        }
        if (_onFinishExpand != null)
            _onFinishExpand.Invoke();
        bCanMove = true;
    }

}
