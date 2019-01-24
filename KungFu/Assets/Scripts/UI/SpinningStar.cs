using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningStar : MonoBehaviour
{
    private Vector3 rot;
    private float speed;
    public float minSpeed;
    public float maxSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rot = new Vector3(0,0,Random.Range(0, 360));
        speed = Random.Range(minSpeed, maxSpeed);

        int rand = Random.Range(0, 2);

        if (rand == 0)
        {
            speed = speed * -1;
        }

        gameObject.transform.rotation = Quaternion.Euler(rot);
    }

    // Update is called once per frame
    void Update()
    {
        rot += new Vector3(0,0,speed);
        gameObject.transform.rotation = Quaternion.Euler(rot);
    }
}
