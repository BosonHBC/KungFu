﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultControl : MonoBehaviour
{
    public GameObject ResultImageShow;
    private GameObject ComboObject;
    private RectTransform imgPosRef;

    float duration = 1.0f;
    //bool firstSpawned = false;
    Coroutine comboCoroutine;

    float fYOffset = 100f;
    // Start is called before the first frame update
    void Start()
    {
        ComboObject = transform.GetChild(0).gameObject;
        imgPosRef = transform.GetChild(1).GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void ShowResultAt(HitResult hitResult, int _attackJointID = 0)
    {
        GameObject ri = Instantiate(ResultImageShow);
        RectTransform goTr = ri.GetComponent<RectTransform>();
        goTr.SetParent(imgPosRef);
        goTr.position = imgPosRef.position + new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0) + imgPosRef.childCount * new Vector3(0, fYOffset, 0);
        #region SetPosition By Player Position
        /*

        Player _player = (Player)FightingManager.instance.characters[0];
        Vector3 WorldObject = _player.GetJointPositionByJointID(_attackJointID).position;
       
        RectTransform CanvasRect = FightingManager.instance.myCanvas.GetComponent<RectTransform>();
        // Calculate The relative position according to the canvas
        Vector3 ViewportPosition = Camera.main.WorldToViewportPoint(WorldObject);
        
        Vector3 WorldObject_ScreenPosition = new Vector3(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        WorldObject_ScreenPosition.y -= GetComponent<RectTransform>().anchoredPosition.y;
        WorldObject_ScreenPosition.x -= GetComponent<RectTransform>().anchoredPosition.x;
        if (WorldObject_ScreenPosition.y <= -CanvasRect.sizeDelta.y * 0.4f)
        {
            WorldObject_ScreenPosition.y = - (CanvasRect.sizeDelta.y * 0.4f + GetComponent<RectTransform>().anchoredPosition.y);
            //Debug.Log("asdasdasd");
        }
        //now you can set the position of the ui element
        goTr.anchoredPosition = WorldObject_ScreenPosition;
        goTr.localPosition = new Vector3(goTr.localPosition.x, goTr.localPosition.y, 0f);
        goTr.localRotation = Quaternion.identity;
        goTr.localScale = new Vector3(1, 1, 1);
      
        */
        #endregion
        ri.GetComponent<ResultImageControl>().ShowResult(hitResult);
    }
    public void ShowResult(HitResult hitResult, int _attackJointID = 0)
    {
        ShowResultAt(hitResult, _attackJointID);
    }
    //IEnumerator CanSpawnAnother()
    //{
    //    yield return new WaitForSeconds(duration);
    //    firstSpawned = false;
    //}

    public void ShowCombo(int countNumber)
    {
        ComboObject.GetComponentInChildren<Text>().text = countNumber.ToString();
        if (comboCoroutine != null)
            StopCoroutine(comboCoroutine);
        comboCoroutine = StartCoroutine(ComboShow());
    }
    IEnumerator ComboShow(float time = 1.0f)
    {
        Text ComboText = ComboObject.GetComponentInChildren<Text>();
        Image ComboImage = ComboObject.GetComponentInChildren<Image>();
        RectTransform combotextTrans = ComboText.GetComponent<RectTransform>();
        RectTransform comboimageTrans = ComboImage.GetComponent<RectTransform>();
        combotextTrans.anchoredPosition = Vector3.zero;
        comboimageTrans.anchoredPosition = Vector3.zero;
        Vector2 offset = new Vector3(0.0f, 100.0f);
        //Text comboText = ComboText.GetComponent<Text>();
        ComboText.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        ComboImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        //Debug.Log("asdasd");
        //UnityEditor.EditorApplication.isPaused = true;
        ComboText.canvasRenderer.SetAlpha(1.0f);
        ComboImage.canvasRenderer.SetAlpha(1.0f);
        ComboText.CrossFadeAlpha(0.0f, time, false);
        ComboImage.CrossFadeAlpha(0.0f, time, false);
        var waitforendofframe = new WaitForEndOfFrame();
        while (combotextTrans.anchoredPosition.y <= offset.y)
        {
            ComboText.GetComponent<RectTransform>().anchoredPosition += offset * time * Time.deltaTime;
            ComboImage.GetComponent<RectTransform>().anchoredPosition += offset * time * Time.deltaTime;
            yield return waitforendofframe;
        }
        ComboText.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        ComboText.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
        ComboImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    public void ShowResultAtWorldPos(HitResult hr, Vector3 pos)
    {
        GameObject ri = Instantiate(ResultImageShow, pos, Quaternion.identity, transform);
        ri.GetComponent<ResultImageControl>().ShowResult(hr);
    }

    public void ShowTutorialResult(HitResult hr, Transform trans)
    {
        GameObject ri = Instantiate(ResultImageShow);
        RectTransform goTr = ri.GetComponent<RectTransform>();
        goTr.SetParent(transform);

        Vector3 WorldObject = trans.position;

        RectTransform CanvasRect = gameObject.GetComponent<RectTransform>();
        // Calculate The relative position according to the canvas
        Vector3 ViewportPosition = Camera.main.WorldToViewportPoint(WorldObject);

        Vector3 WorldObject_ScreenPosition = new Vector3(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));
        //Debug.Log(WorldObject_ScreenPosition);
        //now you can set the position of the ui element
        goTr.anchoredPosition = WorldObject_ScreenPosition;
        goTr.localPosition = new Vector3(goTr.localPosition.x, goTr.localPosition.y, 0f);
        goTr.localRotation = Quaternion.identity;
        goTr.localScale = new Vector3(1, 1, 1);
        ri.GetComponent<ResultImageControl>().ShowResult(hr);
    }
}
