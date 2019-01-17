using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitObjet : MonoBehaviour
{

    private float fCollapseTime;
    private int buttonID;
    private bool bHitCorrectly;
    // Start is called before the first frame update
    void Start()
    {
        fCollapseTime = GameManager.instance.fReactTime + 1;
    }

    public void SetButtonID(int _id)
    {
        buttonID = _id;
    }
    // Update is called once per frame
    void Update()
    {
        if (fCollapseTime > 0)
            fCollapseTime -= Time.deltaTime;

        transform.position -= Vector3.right * Time.deltaTime;

        if (!bHitCorrectly && GameManager.instance.GetUnoInput(buttonID))
        {
            // if it is true
            bHitCorrectly = true;
            if (fCollapseTime > 0.7f && fCollapseTime < 1.3f)
            {
                // PERFECT
                Debug.Log("Perfect!");
                UIController.instance.ShowResult(0);
            }
            else if ((fCollapseTime <= 0.7f && fCollapseTime > 0) || (fCollapseTime > 1.3f && fCollapseTime <= 2f))
            {
                // GOOD
                Debug.Log("Good!");
                UIController.instance.ShowResult(1);

            }

        }

        if (fCollapseTime <= 0)
        {
            // MISS
            if (!bHitCorrectly)
            {
                Debug.Log("Miss!");
                UIController.instance.ShowResult(2);

            }
            Destroy(this.gameObject);
        }
    }
}
