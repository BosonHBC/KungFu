using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCenter : MonoBehaviour
{
    public Transform trP1;
    public Transform trP2;
    public float distanceBetween;

    [SerializeField] float fLerpTime;
    [SerializeField] float fYOffset;

    [SerializeField] float DebugExtend;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, CenterPoint() + new Vector3(0, fYOffset,0), fLerpTime);
        distanceBetween = Vector3.Distance(trP1.position, trP2.position);
        transform.LookAt(transform.position - trP1.right);
        DebugDraw();
    }

    Vector3 CenterPoint()
    {
        return (trP1.position + trP2.position) / 2;
    }

    void DebugDraw()
    {
        Debug.DrawLine(trP1.position + new Vector3(0f, fYOffset, 0f), trP2.position + new Vector3(0f, fYOffset, 0f), Color.blue);
        Debug.DrawRay(transform.position, transform.forward, Color.red);

        Debug.DrawRay(trP2.position, -trP2.forward * DebugExtend, Color.green);

    }

}
