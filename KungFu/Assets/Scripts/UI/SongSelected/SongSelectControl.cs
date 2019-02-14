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
    private Material blurMat;
    private RectTransform trRect;
    private HorizontalLayoutGroup horiLayout;
    private int fPannelWidth;
    private int fPannelXDelta;
    private List<SongPannel> pannels = new List<SongPannel>();
    private int targetSongIndex;
    private UIMover moveTo;
    // move to another song
    private bool bCanMove = true;
    private bool bSelecting;
    private float fExpandWidth = 500;
    private float fUIMoveSpeed = 0.1f;
    private float fUIExpandTime = 0.5f;

    private bool bCanRegister;
    private float fRegisterDelayTime = 0.2f;
    private float fDelayCollpaseTime;

    [Header("Debug")]
    [SerializeField] private int DebugSongCount = 3;
    void Start()
    {
        // Set blur mask
        blurMat = transform.parent.parent.GetChild(0).GetComponent<Image>().material;
        blurMat.SetFloat("_Size", 5.5f);
        moveTo = GetComponent<UIMover>();
        trRect = GetComponent<RectTransform>();
        horiLayout = GetComponent<HorizontalLayoutGroup>();
        fPannelWidth = (int)songPannelPrefab.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite.rect.width;
        targetSongIndex = (DebugSongCount - 1) / 2;
        Debug.Log("CurrentIndex: " + targetSongIndex);

        // Debug High Score
        TestDataLoader loader = GetComponent<TestDataLoader>();
        SimpleJSON.JSONNode allMusicData = loader.GetMusicData();

        // Create game object
        for (int i = 0; i < allMusicData.Count; i++)
        {
            // Debug High Score
            HighScoreManager._instance.ClearLeaderBoard(allMusicData[i]["name"]);
            HighScoreManager._instance.SaveHighScore(RandomString(3), Random.Range(80000, 100000), allMusicData[i]["name"]);
            GameObject go = Instantiate(songPannelPrefab);

            go.name = "SongPannel_" + (DebugSongCount - 1 - i).ToString();
            //go.transform.localRotation = Quaternion.identity;
            // Transform
            go.transform.SetParent(transform);
            go.transform.SetSiblingIndex(1);
            RectTransform _trRect = go.GetComponent<RectTransform>();
            _trRect.localScale = Vector3.one;
            _trRect.localEulerAngles = Vector3.zero;
            _trRect.localPosition = new Vector3(_trRect.position.x, _trRect.position.y, 0);
            // List
            pannels.Add(go.GetComponent<SongPannel>());
            pannels[i].iSongID = i;
            pannels[i].SetData(allMusicData[i]["charName"], allMusicData[i]["name"], allMusicData[i]["lengthInSeconds"]);
        }
        pannels.Reverse();

        // set transform width
        trRect.sizeDelta = new Vector2(trRect.sizeDelta.x + DebugSongCount * fPannelWidth, trRect.sizeDelta.y);
        Invoke("SetData", 0.1f);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D) && bCanRegister)
        {
            NextSong();
        }
        if (Input.GetKey(KeyCode.A) && bCanRegister)
        {
            PreviousSong();
        }
        if (Input.GetKey(KeyCode.E) && bCanRegister)
        {
            SelectSong();
        }
        if (Input.GetKey(KeyCode.Q) && bCanRegister)
        {
            GoBack();
        }

        if (!bCanRegister)
        {
            fDelayCollpaseTime += Time.deltaTime;
            if (fDelayCollpaseTime >= fRegisterDelayTime)
            {
                fDelayCollpaseTime = 0;
                bCanRegister = true;
            }
        }

    }

    public void SelectSong()
    {
        bCanRegister = false;
        if (!bSelecting && bCanMove && targetSongIndex >= 0 && targetSongIndex < pannels.Count)
        {
            bSelecting = true;
            bCanMove = false;
            StartCoroutine(ExpandTargetPannel(targetSongIndex, fExpandWidth, delegate { bCanMove = true; }, fUIExpandTime));
        }
    }
    public void GoBack()
    {
        bCanRegister = false;

        if (bCanMove && bSelecting && targetSongIndex >= 0 && targetSongIndex < pannels.Count)
        {

            bCanMove = false;
            bSelecting = false;
            StartCoroutine(ExpandTargetPannel(targetSongIndex, -fExpandWidth, delegate
            {
                // Recover offset of the tr Rect
                RectTransform _rect = pannels[targetSongIndex].GetComponent<RectTransform>();
                // final moving distance
                float destPos = (int)(_rect.localPosition.x + trRect.anchoredPosition.x);
                moveTo.UIMoveToPosition(Vector3.right, -destPos, fUIMoveSpeed, delegate { Debug.Log("Current: " + targetSongIndex); bCanMove = true; });

            }, fUIExpandTime));


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
        bCanRegister = false;

        if (bCanMove && !bSelecting)
        {
            bCanMove = false;

            if (targetSongIndex < pannels.Count - 1)
            {
                downPointer.FadeIn();
                // next rect transform
                RectTransform _rect = pannels[targetSongIndex + 1].GetComponent<RectTransform>();
                // final moving distance
                float destPos = (int)Mathf.Abs(_rect.localPosition.x + trRect.anchoredPosition.x);
                moveTo.UIMoveToPosition(Vector3.left, destPos, fUIMoveSpeed, delegate { targetSongIndex++; Debug.Log("Current: " + targetSongIndex); bCanMove = true;/*SelectSong();*/ });

            }
            else// Go to border
            {
                downPointer.FadeOut();
                moveTo.UIMoveToPosition(Vector3.left, 1000f, fUIMoveSpeed, delegate { Debug.Log("Go to right boundary"); bCanMove = true; });
                targetSongIndex = pannels.Count;
            }
        }

        if (bSelecting)
        {
            pannels[targetSongIndex].SwitchDifficult(1);
        }
    }

    public void PreviousSong()
    {
        bCanRegister = false;

        if (bCanMove && !bSelecting)
        {
            bCanMove = false;
            if (targetSongIndex > 0)
            {
                downPointer.FadeIn();
                RectTransform _rect = pannels[targetSongIndex - 1].GetComponent<RectTransform>();
                float destPos = (int)Mathf.Abs(_rect.localPosition.x + trRect.anchoredPosition.x);
                moveTo.UIMoveToPosition(Vector3.right, destPos, fUIMoveSpeed, delegate { targetSongIndex--; Debug.Log("Current: " + targetSongIndex); bCanMove = true;/*SelectSong();*/ });
            }
            else// Go to border
            {
                downPointer.FadeOut();
                moveTo.UIMoveToPosition(Vector3.right, 1000f, fUIMoveSpeed, delegate { Debug.Log("Go to left boundary"); bCanMove = true; });
                targetSongIndex = -1;
            }
        }
        if (bSelecting)
        {
            pannels[targetSongIndex].SwitchDifficult(0);
        }
    }

    IEnumerator ExpandTargetPannel(int _id, float _delta, UnityAction _onFinishExpand, float _fadeTime = 0.5f)
    {
        bSelecting = true;
        if (_id >= 0 && _id < pannels.Count)
        {
            if (_delta > 0)
            {
                pannels[_id].bSelecting = true;
                pannels[_id].ExpandPannel();
            }
            else if (_delta < 0)
            {
                pannels[_id].bSelecting = false;
                pannels[_id].FoldPannel();
            }
        }


        float _timeStartFade = Time.time;
        float _timeSinceStart = Time.time - _timeStartFade;
        float _lerpPercentage = _timeSinceStart / _fadeTime;

        float pannel_Start = pannels[_id].GetComponent<RectTransform>().sizeDelta.x;
        float pannel_Dest = pannel_Start + _delta;

        float parent_Start = trRect.sizeDelta.x;
        float parent_Dest = parent_Start + 2 * _delta;
        float parentX_Start = 0;
        float parentX_Dest = 0;

        int _preNextOffset = 0;
        if (targetSongIndex == 0)
            _preNextOffset = 1;
        if (targetSongIndex == 2)
            _preNextOffset = -1;

        if (targetSongIndex == 0 || targetSongIndex == 2)
        {
            parentX_Start = trRect.anchoredPosition.x;
            parentX_Dest = trRect.anchoredPosition.x + _preNextOffset * 90;
        }

        while (true)
        {
            _timeSinceStart = Time.time - _timeStartFade;
            _lerpPercentage = _timeSinceStart / _fadeTime;

            float pannel_Current = Mathf.Lerp(pannel_Start, pannel_Dest, _lerpPercentage);
            float parent_Current = Mathf.Lerp(parent_Start, parent_Dest, _lerpPercentage);

            RectTransform _rect = pannels[_id].GetComponent<RectTransform>();
            _rect.sizeDelta = new Vector2(pannel_Current, _rect.sizeDelta.y);
            trRect.sizeDelta = new Vector2(parent_Current, trRect.sizeDelta.y);
            if (targetSongIndex == 0 || targetSongIndex == 2)
            {
                float trX_Current = Mathf.Lerp(parentX_Start, parentX_Dest, _lerpPercentage);

                trRect.anchoredPosition = new Vector2(trX_Current, trRect.anchoredPosition.y);
            }
            if (_lerpPercentage >= 1) break;


            yield return new WaitForEndOfFrame();
        }
        if (_delta < 0)
            bSelecting = false;
        else
            bSelecting = true;

        if (_onFinishExpand != null)
            _onFinishExpand.Invoke();

    }

    private void OnDestroy()
    {
        blurMat.SetFloat("_Size", 0f);

    }

    public string RandomString(int length)
    {
        char[] stringChars = new char[length];
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        
        for (int i = 0; i < length; i++)
        {
            Random.InitState(i);
            stringChars[i] = chars[Random.Range(0, length)];
        }

        return stringChars.ToString();
    }
}
