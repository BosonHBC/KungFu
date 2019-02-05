using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    #region UIData
    [SerializeField] Text perfect;
    [SerializeField] Text ok;
    [SerializeField] Text miss;

    [SerializeField] Text title;

    [SerializeField] Sprite[] ABS;
    [SerializeField] Text score;
    [SerializeField] Text highCombo;
    [SerializeField] Image result;
    #endregion

    Material blurMat;
    RectTransform maskRect;

    [SerializeField]
    private float showTime = 1;
    private float collpaseTime = 0;
    private bool ending;
    private bool ended;


    // Start is called before the first frame update
    void Start()
    {
        maskRect = transform.GetChild(1).GetComponent<RectTransform>();
        blurMat = transform.GetChild(0).GetComponent<Image>().material;
        Debug.Log(blurMat);
        blurMat.SetFloat("_Size", 0);
        maskRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

    }

    public void SetEndData(int _score, int _combo, int _prefect, int _ok, int _miss, string _title)
    {
        StartCoroutine("DelayShow");
        score.text = _score.ToString();
        highCombo.text = _combo.ToString();
        perfect.text = _prefect.ToString();
        ok.text = _ok.ToString();
        miss.text = _miss.ToString();
        title.text = _title;

        int _id = 0;
        if (_score >= 95000)
            _id = 0;
        else if (_score >= 85000 && _score < 95000)
            _id = 1;
        else if (_score < 85000)
            _id = 2;
        result.sprite = ABS[_id];

        HighScoreManager._instance.CheckIfHighScore(_score);
    }

    IEnumerator DelayShow()
    {
        yield return new WaitForSeconds(2f);
        ending = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (ending && !ended)
        {
            collpaseTime += Time.deltaTime;
            float per = collpaseTime / showTime;

            blurMat.SetFloat("_Size", per*7.5f);
            maskRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, per*300);

            if (collpaseTime >= showTime)
            {
                ended = true;
            }
        }
    }

    private void OnDestroy()
    {
        blurMat.SetFloat("_Size", 0);

    }
}
