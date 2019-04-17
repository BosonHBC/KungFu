using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsScroller : MonoBehaviour
{
    public Transform TipParent;
    [SerializeField]
    float TipShowTime = 3.0f;
    Text[] Tips;
    int currentTip = 0;
    float timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        Tips = TipParent.GetComponentsInChildren<Text>();
        for(int i = 1; i < Tips.Length; ++ i)
        {
            Tips[i].canvasRenderer.SetAlpha(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(timer >= TipShowTime)
        {
            ShowNextTip();
            timer = 0.0f;
        }
        timer += Time.deltaTime;
    }

    void ShowNextTip()
    {
        int nextTipIndex = (currentTip + 1) % Tips.Length;
        StartCoroutine(TipFadeOut(currentTip));
        Tips[nextTipIndex].CrossFadeAlpha(1.0f, 1.0f, false);
        currentTip = nextTipIndex;
    }

    IEnumerator TipFadeOut(int index, float fadeTime = 1.0f)
    {
        Text tipToFade = Tips[index];
        Vector2 destination = tipToFade.rectTransform.anchoredPosition + new Vector2(0, 100.0f);
        Vector2 speed = (destination - tipToFade.rectTransform.anchoredPosition) / fadeTime;
        tipToFade.CrossFadeAlpha(0.0f, fadeTime, false);
        var wait = new WaitForEndOfFrame();
        while(tipToFade.rectTransform.anchoredPosition.y < destination.y)
        {
            tipToFade.rectTransform.anchoredPosition += speed * Time.deltaTime;
            yield return wait;
        }
        tipToFade.canvasRenderer.SetAlpha(0.0f);
        tipToFade.rectTransform.anchoredPosition = Vector2.zero;
    }
}
