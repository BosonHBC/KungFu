using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasRotator : MonoBehaviour
{
    public float RotateInterval = 2.0f;
    public float RoateAngle = 90.0f;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(RotateCanvas(-90.0f, RotateInterval));
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(RotateCanvas(90.0f, RotateInterval));
        }
    }

    IEnumerator RotateCanvas(float angle, float time_interval)
    {
        float current_rot = 0.0f;
        float dest_rot = angle;
        float speed = dest_rot / time_interval;
        var oldRot = transform.localEulerAngles;
        var wait = new WaitForEndOfFrame();
        while(dest_rot >= current_rot)
        {
            current_rot += speed * Time.deltaTime;
            transform.localEulerAngles += new Vector3(0.0f, speed * Time.deltaTime);
            yield return wait;
        }
        transform.localEulerAngles = oldRot + new Vector3(0.0f, angle, 0.0f);
    }
}
