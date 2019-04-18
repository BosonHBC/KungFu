using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TransitionControl : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera vCam;
    CinemachineTrackedDolly trackDolly;
    [SerializeField] Transform[] positions;
    [SerializeField] Transform ActualLookPoint;
    [SerializeField] float fLerpTime = 1f;
    private int currentId = 0;

    bool bMoving = false;
    // Start is called before the first frame update
    void Start()
    {
        trackDolly = vCam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    private void Update()
    {
    }

    public void MoveMenu(int _delta)
    {
        if (!bMoving)
        {
            bMoving = true;
            if (_delta > 0)
            {
                //Debug.Log("MoveRight");
                currentId++;
            }
            else if (_delta < 0)
            {
                //Debug.Log("MoveLeft");
                currentId--;
            }

            if (currentId < 0)
                currentId = positions.Length - 1;
            if (currentId > positions.Length -1)
                currentId = 0;
            LerpFloatPosition(currentId, fLerpTime);
            LerpV3Position(positions[currentId].position, fLerpTime);
        }

    }

    public void LerpFloatPosition(float _end, float _fadeTime)
    {

        StartCoroutine(SimleLerper_float(trackDolly.m_PathPosition, _end, _fadeTime));
    }
    public void LerpV3Position(Vector3 _end, float _fadeTime)
    {
        StartCoroutine(SimleLerper_v3(ActualLookPoint.transform.position, _end, _fadeTime));
    }

    IEnumerator SimleLerper_float(float _start, float _end, float _fadeTime = 0.5f)
    {
        float _timeStartFade = Time.time;
        float _timeSinceStart = Time.time - _timeStartFade;
        float _lerpPercentage = _timeSinceStart / _fadeTime;

        float _tempStart = _start;
        while (true)
        {
            _timeSinceStart = Time.time - _timeStartFade;
            _lerpPercentage = _timeSinceStart / _fadeTime;

            float currentValue = Mathf.Lerp(_tempStart, _end, _lerpPercentage);
            trackDolly.m_PathPosition = currentValue;

            if (_lerpPercentage >= 1) break;


            yield return new WaitForEndOfFrame();
        }
        bMoving = false;
    }

    IEnumerator SimleLerper_v3(Vector3 _start, Vector3 _end, float _fadeTime = 0.5f)
    {
        float _timeStartFade = Time.time;
        float _timeSinceStart = Time.time - _timeStartFade;
        float _lerpPercentage = _timeSinceStart / _fadeTime;

        Vector3 _tempStart = _start;
        while (true)
        {
            _timeSinceStart = Time.time - _timeStartFade;
            _lerpPercentage = _timeSinceStart / _fadeTime;

            Vector3 currentValue = Vector3.Lerp(_tempStart, _end, _lerpPercentage);
            ActualLookPoint.position = currentValue;

           // Debug.Log(_start);
            if (_lerpPercentage >= 1) break;


            yield return new WaitForEndOfFrame();
        }
        bMoving = false;
    }
}
