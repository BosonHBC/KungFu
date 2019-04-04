using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingIndicatorControl : MonoBehaviour
{
    Dictionary<int, Transform> JointTransMap;
    public GameObject RingIndicator;
    public Color PerfectColor = Color.red;
    public Color OKColor = Color.green;

    Vector3 FinalSize = new Vector3(0.1f, 0.1f, 0.1f);
    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetData(Transform _Enemy)
    {
        AttackJointID[] _attackArray = _Enemy.GetComponentsInChildren<AttackJointID>();
        JointTransMap = new Dictionary<int, Transform>();
        foreach (var item in _attackArray)
        {
            JointTransMap.Add(item.iJointID, item.transform);
            //Debug.Log(item.iJointID);
        }
    }

    //pending fix
    public void ShowRingIndicator(BeatInfo beatData)
    {
        foreach(int jointID in beatData.JointIDs)
        {
            GameObject ring = Instantiate(RingIndicator, JointTransMap[jointID].position, Quaternion.identity, JointTransMap[jointID]);
            StartCoroutine(StartShrinking(ring, beatData));
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
