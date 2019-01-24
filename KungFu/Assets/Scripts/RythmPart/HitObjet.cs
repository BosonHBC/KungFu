using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitObjet : MonoBehaviour
{

    private float fCollapseTime;
    private int buttonID;
    private bool bHitCorrectly;
    private RectTransform rect;
    private float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        fCollapseTime = GameManager.instance.fReactTime + 1;
        GameManager.instance.i_ExistingHitObject++;
        rect = GetComponent<RectTransform>();
        moveSpeed = rect.localPosition.x / (GameManager.instance.fReactTime) ;
    }

    public void SetButtonID(int _id, int beatID)
    {
        buttonID = _id;
        Debug.Log("Now ButtonID to press:" + buttonID);
        transform.GetChild(0).GetComponent<Image>().sprite = UIController.instance.GetReference(beatID);
    }
    // Update is called once per frame
    void Update()
    {
        if (fCollapseTime > 0)
            fCollapseTime -= Time.deltaTime;

        rect.localPosition -= Vector3.right * moveSpeed * Time.deltaTime;

        if (!bHitCorrectly && GameManager.instance.GetUnoInput(buttonID) && fCollapseTime <= 2f)
        {
            // if it is true
            bHitCorrectly = true;
            if (fCollapseTime > 0.7f && fCollapseTime < 1.3f)
            {
                // PERFECT
                Debug.Log("Perfect!");
                UIController.instance.ShowResult(0);
                GameManager.instance.HitResult(0);
                UIController.instance.PlayPandaNodHead();
                FadeOutAndDestroy();
            }
            else if ((fCollapseTime <= 0.7f && fCollapseTime > 0) || (fCollapseTime > 1.3f && fCollapseTime <= 2f))
            {
                // GOOD
                Debug.Log("Good!");
                UIController.instance.ShowResult(1);
                GameManager.instance.HitResult(1);
                UIController.instance.PlayPandaNodHead();

                FadeOutAndDestroy();

            }

        }

        if (fCollapseTime <= 0)
        {
            // MISS
            if (!bHitCorrectly)
            {
                Debug.Log("Miss!");
                UIController.instance.ShowResult(2);
                GameManager.instance.HitResult(2);

            }
            FadeOutAndDestroy();
        }
    }

    public void FadeOutAndDestroy()
    {
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        GameManager.instance.i_ExistingHitObject--;
    }
}
