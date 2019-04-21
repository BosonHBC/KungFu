using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioVisualizer : MonoBehaviour
{
    int sampleSize;
    [SerializeField] float fDistToCenter;
    RectTransform RectTr;

    // Start is called before the first frame update
    void Start()
    {
        RectTr = transform.GetChild(0).GetComponent<RectTransform>();
        sampleSize = AudioPeer.sampleSize;

    }
    float _max = 0;
    float _min = 1000000;
    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < sampleSize; i++)
        {
            if (AudioPeer.sample[i] < _min)
                _min = AudioPeer.sample[i];
            if (AudioPeer.sample[i] > _max)
                _max = AudioPeer.sample[i];
        }
        for (int i = 0; i < sampleSize; i++)
        {
            float _usefulSample = AudioPeer.sample[i] * 1000000f;
            //Debug.Log("Sample_" + i +": "+ _usefulSample);
            RectTr.localScale = new Vector3(_usefulSample + 0.5f, _usefulSample + 0.5f,1);
        }
    }
}
