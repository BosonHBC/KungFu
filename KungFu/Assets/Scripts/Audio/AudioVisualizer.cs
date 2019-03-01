using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioVisualizer : MonoBehaviour
{
    [SerializeField] GameObject ImagePrefab;
    Image[] images = new Image[AudioPeer.sampleSize];

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < images.Length / 2; i++)
        {
            GameObject go = Instantiate(ImagePrefab);
            go.transform.SetParent(this.transform);
            go.transform.localPosition = new Vector3(i * -28, 0, 0);
            go.transform.localScale = Vector3.one;
            go.transform.localEulerAngles = Vector3.zero;
            images[i] = go.GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float _max = 0;
        float _min = 1000000;
        for (int i = 0; i < images.Length / 2; i++)
        {
            if (AudioPeer.sample[i] < _min)
                _min = AudioPeer.sample[i];
            if (AudioPeer.sample[i] > _max)
                _max = AudioPeer.sample[i];
        }
        for (int i = 0; i < images.Length / 2; i++)
        {
            images[i].fillAmount = AudioPeer.sample[i] / (_max);
        }
    }
}
