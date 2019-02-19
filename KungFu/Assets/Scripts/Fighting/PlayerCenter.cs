using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCenter : MonoBehaviour
{
    public Transform trP1;
    public Transform trP2;

    [SerializeField] float fLerpTime;
    [SerializeField] float fYOffset; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, CenterPoint() + new Vector3(0, fYOffset,0), fLerpTime);
    }

    Vector3 CenterPoint()
    {
        return (trP1.position + trP2.position) / 2;
    }
    
}
