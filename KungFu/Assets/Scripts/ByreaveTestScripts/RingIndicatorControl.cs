using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingIndicatorControl : MonoBehaviour
{
    public Transform LeftWrist;
    public Transform RightWrist;
    public GameObject RingIndicator;
    public Color PerfectColor = Color.red;
    public Color OKColor = Color.green;
    Dictionary<int, Transform[]> RingPositionMap;

    Vector3 FinalSize = new Vector3(0.1f, 0.1f, 0.1f);
    // Start is called before the first frame update
    void Start()
    {
        RingPositionMap = new Dictionary<int, Transform[]>()
        {
            {0, new Transform[] { RightWrist } },
            {1, new Transform[] { RightWrist } },
            {2, new Transform[] { RightWrist } },
            {3, new Transform[] { LeftWrist, RightWrist } },

        };
    }

    //pending fix
    public void ShowRingIndicator(BeatInfo beatAnimation)
    {
        foreach(var map in RingPositionMap)
        {
            if(map.Key == beatAnimation.BeatID)
            {
                foreach(Transform trans in map.Value)
                {
                    GameObject ring = Instantiate(RingIndicator, trans.position, Quaternion.identity, trans);
                    StartCoroutine(StartShrinking(ring, beatAnimation.PerfectStart, beatAnimation.PerfectDuration));
                }
                break;
            }
        }
    }

    IEnumerator StartShrinking(GameObject ring, float perfectStart, float perfectDuration)
    {
        float timer = 0;
        ring.GetComponent<SpriteRenderer>().color = OKColor;
        Vector3 scalingSpeed = (FinalSize - ring.transform.localScale) / (perfectStart + perfectDuration);
        while(timer <= perfectStart + perfectDuration)
        {
            ring.transform.localScale += scalingSpeed * Time.deltaTime;
            if(timer >= perfectStart)
                ring.GetComponent<SpriteRenderer>().color = PerfectColor;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(ring);
    }
}
