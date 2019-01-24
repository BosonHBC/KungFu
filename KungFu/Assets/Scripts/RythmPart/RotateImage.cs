using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateImage : MonoBehaviour
{

    private float targetYRot;
    private bool isRotating;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            // float diff = Mathf.Abs( transform.localEulerAngles.y - targetYRot);
            float diff = Vector3.Distance(transform.localEulerAngles, new Vector3(0, targetYRot, 0));
            
            Vector3 rot = Vector3.Lerp(transform.localEulerAngles, new Vector3(0, targetYRot, 0),5* Time.deltaTime);
            transform.localEulerAngles = rot;
            if (diff <= 3f)
            {
                isRotating = false;
                if (Mathf.Abs(transform.localEulerAngles.y - 90) < 5)
                    gameObject.SetActive(false);
            }
                
        }
    }

    public void RotateRight()
    {
        targetYRot = 0;
        isRotating = true;
    }

    public void RotateLeft()
    {
        targetYRot = 90;
        isRotating = true;
    }
}
