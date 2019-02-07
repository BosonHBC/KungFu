using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIMover : MonoBehaviour
{
    [SerializeField]
    private float[] horizontalRestrition = { 0,0};

    private bool bMoving;
    private RectTransform trRect;
    private Vector3 destiation;
    private UnityAction onFinishedMoving;
    private float fMoveSpeed;
    private float fThreshold = 1f;
    // Start is called before the first frame update
    void Start()
    {
        trRect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bMoving)
        {
            trRect.localPosition = Vector3.Lerp(trRect.localPosition, destiation, fMoveSpeed);
            if (Vector3.Distance(trRect.localPosition, destiation) < fThreshold)
            {
                trRect.localPosition = destiation;
                bMoving = false;
                onFinishedMoving.Invoke();
            }

        }
    }

    public void UIMoveToPosition(Vector3 _dir, float _dist, float _moveSpeed, UnityAction _onFinishedMoving)
    {
        if (!bMoving)
        {
            destiation = trRect.localPosition + _dir * _dist;
            if (destiation.x < horizontalRestrition[0])
                destiation.x = horizontalRestrition[0];
            if(destiation.x > horizontalRestrition[1])
                destiation.x = horizontalRestrition[1];

            bMoving = true;
            fMoveSpeed = _moveSpeed;
            onFinishedMoving = _onFinishedMoving;
        }
        else
        {
            Debug.Log("Moving! Can not move");
        }

    }

    public void SetRestriction(int _min, int _max)
    {
        horizontalRestrition[0] = _min;
        horizontalRestrition[1] = _max;
    }
    public float GetRestriction(int _id)
    {
        return horizontalRestrition[_id];
    }
}
