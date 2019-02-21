using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultControl : MonoBehaviour
{
    public GameObject ResultImageShow;
    public Vector3 Offset = new Vector3(0.3f, 0.0f, 0.0f);

    //two result at same time, can be improved
    float duration = 1.0f;
    bool firstSpawned = false;
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
}
