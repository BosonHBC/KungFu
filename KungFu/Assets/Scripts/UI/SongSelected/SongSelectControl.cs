using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SongSelectControl : MonoBehaviour
{
    // public and Serilizable
    [SerializeField] GameObject songPannelPrefab;
    [SerializeField] Transform detailPannel;

    // private
    private RectTransform trRect;
    private HorizontalLayoutGroup horiLayout;
    private int fPannelWidth;
    private int fPannelXDelta;
    private List<SongPannel> pannels = new List<SongPannel>();
    private int targetSongIndex;
    private UIMover moveTo;
    private bool bCanMove = true;

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
    }

    public void SelectSong()
    {
        StartCoroutine(IESelectSong());
    }
    IEnumerator IESelectSong()
    {
        float fadeTime = 0.5f;
        pannels[targetSongIndex].transform.GetChild(0).GetComponent<UIFader>().FadeOut(fadeTime);
        // detailPannel.GetComponent<UIFader>().FadeIn(fadeTime);
        yield return new WaitForSeconds(fadeTime);
        bCanMove = true;
    }
    void SetData()
    {
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
                if (targetSongIndex >= 0)   // fade in previous one
                    pannels[targetSongIndex].transform.GetChild(0).GetComponent<UIFader>().FadeIn();
                // next rect transform
                RectTransform _rect = pannels[targetSongIndex + 1].GetComponent<RectTransform>();
                // final moving distance
                float destPos = (int)Mathf.Abs(_rect.localPosition.x + trRect.anchoredPosition.x);
                moveTo.UIMoveToPosition(Vector3.left, destPos, 0.2f, delegate { targetSongIndex++; Debug.Log("Current: " + targetSongIndex); SelectSong(); });

            }
            else
            {
                pannels[pannels.Count - 1].transform.GetChild(0).GetComponent<UIFader>().FadeIn();
                moveTo.UIMoveToPosition(Vector3.left, 1000f, 0.2f, delegate { Debug.Log("Go to right boundary"); bCanMove = true; });
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
                if (targetSongIndex <= pannels.Count-1)   // fade in previous one
                    pannels[targetSongIndex].transform.GetChild(0).GetComponent<UIFader>().FadeIn();
                RectTransform _rect = pannels[targetSongIndex - 1].GetComponent<RectTransform>();
                float destPos = (int)Mathf.Abs(_rect.localPosition.x + trRect.anchoredPosition.x);
                moveTo.UIMoveToPosition(Vector3.right, destPos, 0.2f, delegate { targetSongIndex--; Debug.Log("Current: " + targetSongIndex); SelectSong(); });
            }
            else
            {
                pannels[0].transform.GetChild(0).GetComponent<UIFader>().FadeIn();
                moveTo.UIMoveToPosition(Vector3.right, 1000f, 0.2f, delegate { Debug.Log("Go to left boundary"); bCanMove = true; });
                targetSongIndex = -1;
            }
        }



    }


}
