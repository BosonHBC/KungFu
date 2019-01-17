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
        fCollapseTime = GameManager.instance.fReactTime;
    }

    public void SetButtonID(int _id)
    {
        buttonID = _id;
    }
    // Update is called once per frame
    void Update()
    {
        if (!bHitCorrectly)
            fCollapseTime -= Time.deltaTime;

        if (GameManager.instance.GetUnoInput(buttonID))
        {
            // if it is true
            bHitCorrectly = true;
            if (fCollapseTime > GameManager.instance.fReactTime / 2)
            {
                // PERFECT
                Debug.Log("Perfect!");
            }
            else
            {
                // GOOD
                Debug.Log("Good!");
            }

        }

        if (fCollapseTime <= 0)
        {
            // MISS
            if (!bHitCorrectly)
            {
                Debug.Log("Miss!");
            }
            Destroy(this.gameObject);
        }
    }
}
