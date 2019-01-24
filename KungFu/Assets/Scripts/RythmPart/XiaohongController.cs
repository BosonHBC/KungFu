using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XiaohongController : MonoBehaviour
{
    public static XiaohongController instance;

    private void Awake()
    {
        if (instance == null || instance != this)
            instance = this;
    }

    RotateImage[] images;
    // Start is called before the first frame update
    void Start()
    {
        images = new RotateImage[transform.childCount];
        for (int i = 0; i < images.Length; i++)
        {
            images[i] = transform.GetChild(i).GetComponent<RotateImage>();
            Debug.Log(images[i].name);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NextPose(int beats)
    {
        StartCoroutine(waitForFilpLeft(beats));
    }

    IEnumerator waitForFilpLeft(int _beat)
    {

        yield return new WaitForSeconds(GameManager.instance.fReactTime - 2.5f);
        if (_beat != 0)
            images[_beat - 1].RotateLeft();
        yield return new WaitForSeconds(1f);
        images[_beat].RotateRight();
    }
}
