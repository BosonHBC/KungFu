using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    // Start is called before the first frame update
    public bool StartReact = false;
    public bool InZone = false;
    int direction = 0;
    Rigidbody rb;
    void Start()
    {
        direction = Random.Range(0, 4);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
