using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultControl : MonoBehaviour
{
    public GameObject ResultImageShow;
    public GameObject ComboText;
    public Vector3 Offset = new Vector3(0.3f, 0.0f, 0.0f);

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
    void ShowResultAt(HitResult hitResult, Vector3 locPos)
    {
        GameObject ri = Instantiate(ResultImageShow, locPos, transform.rotation, transform);
        ri.GetComponent<ResultImageControl>().ShowResult(hitResult);
    }
    public void ShowResult(HitResult hitResult)
    {
        if(!firstSpawned)
        {
            ShowResultAt(hitResult, transform.position);
            firstSpawned = true;
            StartCoroutine(CanSpawnAnother());
        }
        else
        {
            ShowResultAt(hitResult, transform.position + Offset);
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
