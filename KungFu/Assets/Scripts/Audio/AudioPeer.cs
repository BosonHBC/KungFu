using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPeer : MonoBehaviour
{
    private AudioSource _source;
    public static int sampleSize = 256;
    public static float[] sample = new float[sampleSize];
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_source != null)
            _source.GetSpectrumData(sample, 0, FFTWindow.Hamming);
    }
    public void SetSource(AudioSource _src)
    {
        _source = _src;
    }
}
