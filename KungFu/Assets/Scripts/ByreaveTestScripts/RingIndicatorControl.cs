using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingIndicatorControl : MonoBehaviour
{
    Transform LeftWrist;
    Transform RightWrist;
    Transform LeftKnee;
    Transform RightKnee;
    public GameObject RingIndicator;
    public Color PerfectColor = Color.red;
    public Color OKColor = Color.green;
    Dictionary<int, Transform[]> RingPositionMap;

    Vector3 FinalSize = new Vector3(0.1f, 0.1f, 0.1f);
    // Start is called before the first frame update
    void Start()
    {
        //RingPositionMap = new Dictionary<int, Transform[]>()
        //{
        //    {0, new Transform[] { RightWrist } },
        //    {1, new Transform[] { RightWrist } },
        //    {2, new Transform[] { RightWrist } },
        //    {3, new Transform[] { LeftWrist, RightWrist } },
        //    {4, new Transform[] { LeftWrist } },
        //    {5, new Transform[] { RightWrist } },
        //    {6, new Transform[] { RightWrist } }
        //};
    }

    public void SetData(Transform _Enemy)
    {
        AttackJointID[] _attackArray = _Enemy.GetComponentsInChildren<AttackJointID>();
        foreach (var item in _attackArray)
        {
            switch (item.iJointID)
            {
                case 1:
                    RightWrist = item.transform;
                    break;
                case 2:
                    LeftWrist = item.transform;
                    break;
                case 3:
                    RightKnee = item.transform;
                    break;
                case 4:
                    LeftKnee = item.transform;
                    break;
                default:
                    break;
            }
        }
        RingPositionMap = new Dictionary<int, Transform[]>()
        {
            {0, new Transform[] { RightWrist } },
            {1, new Transform[] { LeftWrist, RightWrist } },
            {2, new Transform[] { LeftWrist } },
            {3, new Transform[] { LeftWrist } },
            {4, new Transform[] { RightWrist } },
            {5, new Transform[] { LeftKnee } },
            {6, new Transform[] { LeftWrist } }
        };
    }

    //pending fix
    public void ShowRingIndicator(BeatInfo beatAnimation)
    {
        foreach (var map in RingPositionMap)
        {
            if (map.Key == beatAnimation.BeatID)
            {
                foreach (Transform trans in map.Value)
                {
                    GameObject ring = Instantiate(RingIndicator, trans.position, Quaternion.identity, trans);
                    StartCoroutine(StartShrinking(ring, beatAnimation));
                }
                break;
            }
        }
    }

    IEnumerator StartShrinking(GameObject ring, BeatInfo beatTiming)
    {
        float timer = 0;
        ring.GetComponent<SpriteRenderer>().color = OKColor;
        //MosicFader_Sprite _fader = ring.GetComponent<MosicFader_Sprite>();
        //_fader.SetColor(OKColor);
        //_fader.FadeTo(1, 0.5f);

        var wait = new WaitForEndOfFrame();
        Vector3 scalingSpeed = (FinalSize - ring.transform.localScale) / (beatTiming.PerfectStart + beatTiming.PerfectDuration);
        while (timer <= beatTiming.PerfectStart + beatTiming.PerfectDuration)
        {
            ring.transform.localScale += scalingSpeed * Time.deltaTime;
            if (timer >= beatTiming.PerfectStart - beatTiming.OKStart)
                ring.GetComponent<SpriteRenderer>().color = PerfectColor;

            timer += Time.deltaTime;
            yield return wait;
        }
        Destroy(ring);
    }
}
