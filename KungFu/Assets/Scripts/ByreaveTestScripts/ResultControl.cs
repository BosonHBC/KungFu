using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultControl : MonoBehaviour
{
    public GameObject ResultImageShow;
    public GameObject ComboText;

    //two result at same time, can be improved
    float duration = 1.0f;
    bool firstSpawned = false;
    Coroutine comboCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void ShowResultAt(HitResult hitResult, int _attackJointID = 0)
    {
        GameObject ri = Instantiate(ResultImageShow);
        RectTransform goTr = ri.GetComponent<RectTransform>();
        goTr.SetParent(transform);
        Player _player = (Player)FightingManager.instance.characters[0];
        Vector3 WorldObject = _player.GetJointTransform(_attackJointID).position;
       
        RectTransform CanvasRect = FightingManager.instance.myCanvas.GetComponent<RectTransform>();
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
        ri.GetComponent<ResultImageControl>().ShowResult(hitResult);
    }
    public void ShowResult(HitResult hitResult, int _attackJointID = 0)
    {
        if(!firstSpawned)
        {
            ShowResultAt(hitResult);
            firstSpawned = true;
            StartCoroutine(CanSpawnAnother());
        }
        else
        {
            ShowResultAt(hitResult,0);
            //firstSpawned = false;
        }
    }
    IEnumerator CanSpawnAnother()
    {
        yield return new WaitForSeconds(duration);
        firstSpawned = false;
    }

    public void ShowCombo(int countNumber)
    {
        ComboText.GetComponent<Text>().text = countNumber.ToString();
        //if(comboCoroutine == null)
            comboCoroutine = StartCoroutine(ComboShow());
        //else
        //{
        //    StopCoroutine(comboCoroutine);
        //    comboCoroutine = StartCoroutine(ComboShow());
        //}
    }
    IEnumerator ComboShow(float time = 1.0f)
    {
        ComboText.GetComponent<RectTransform>().localPosition = Vector3.zero;
        Vector3 offset = new Vector3(0.0f, 100.0f, 0.0f);
        ComboText.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        //Debug.Log("asdasd");
        //UnityEditor.EditorApplication.isPaused = true;

        while (ComboText.GetComponent<RectTransform>().localPosition.y <= offset.y)
        {
            ComboText.transform.Translate(Vector3.up / 3 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        ComboText.GetComponent<RectTransform>().localPosition = Vector3.zero;
        ComboText.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }
}
