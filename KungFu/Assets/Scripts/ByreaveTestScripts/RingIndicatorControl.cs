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
    Dictionary<string, Transform[]> RingPositionMap;

    Vector3 FinalSize = new Vector3(0.1f, 0.1f, 0.1f);
    // Start is called before the first frame update
    void Start()
    {
        RingPositionMap = new Dictionary<string, Transform[]>()
        {
            {"Hook", new Transform[] { RightWrist } },
            {"Hammer", new Transform[] { RightWrist } },
            {"Uppercut", new Transform[] { RightWrist } },
            {"handssors", new Transform[] { LeftWrist, RightWrist } },

        };
    }

    public void ShowRingIndicator(BeatAnimation beatAnimation)
    {
        foreach(var map in RingPositionMap)
        {
            if(map.Key == beatAnimation.Name)
            {
                foreach(Transform trans in map.Value)
                {
                    GameObject ring = Instantiate(RingIndicator, trans.position, Quaternion.identity, trans);
                    StartCoroutine(StartShrinking(ring, beatAnimation.PerfectStart, beatAnimation.PerfectDuration));
                    break;
                }
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
