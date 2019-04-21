using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioVisualizer : MonoBehaviour
{
    RectTransform[] childList = new RectTransform[AudioPeer.sampleSize];
    [SerializeField] float fDistToCenter;
    LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = AudioPeer.sampleSize;
        int segments = lr.positionCount;

        float angle = 0;
        for (int i = 0; i < (segments + 1); i++)
        {
          float  x = Mathf.Sin(Mathf.Deg2Rad * angle) * fDistToCenter;
           float z = Mathf.Cos(Mathf.Deg2Rad * angle) * fDistToCenter;

            lr.SetPosition(i, new Vector3(x, 0, z));

            angle += (360f / segments);
        }
    }
    float _max = 0;
    float _min = 1000000;
    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < childList.Length; i++)
        {
            if (AudioPeer.sample[i] < _min)
                _min = AudioPeer.sample[i];
            if (AudioPeer.sample[i] > _max)
                _max = AudioPeer.sample[i];
        }
        for (int i = 0; i < childList.Length; i++)
        {
            //childList[i].localScale = new Vector3(1, AudioPeer.sample[i] /(_max) + 0.5f,1);
        }
    }
}
