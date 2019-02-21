using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : Character
{
    [Header("Player")]
    [SerializeField] private CinemachineVirtualCamera myCamera;
    private CinemachineTrackedDolly trackedDolly;
    private int iCurrentView;   // 0 -> first person, 1 -> third person
    [SerializeField] private float fDebugMovespeed;
    private Animator anim;
    private bool bSwitching;

    private int iPlayingAnimationID;

    protected override void Start()
    {
        base.Start();
        iCurrentView = 0;
        bSwitching = false;
        trackedDolly = myCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        DebugMovement();
        DebugAnimator();
    }

    public void SwitchPerspectiveView()
    {
       
        if (!bSwitching)
        {
            if (iCurrentView == 0)
            {
               StartCoroutine(SwitchView(trackedDolly.m_PathPosition, 0, 0.2f));
                iCurrentView = 1;
            }
            else if (iCurrentView == 1)
            {
                StartCoroutine(SwitchView(trackedDolly.m_PathPosition, 2f, 0.2f));
                iCurrentView = 0;
            }

        }
    }

    IEnumerator SwitchView(float _start, float _end, float _fadeTime = 0.5f)
    {
        bSwitching = true;
        float _timeStartFade = Time.time;
        float _timeSinceStart = Time.time - _timeStartFade;
        float _lerpPercentage = _timeSinceStart / _fadeTime;

        Debug.Log("Start: " +  _start + " Endf: " + _end);

        while (true)
        {
            _timeSinceStart = Time.time - _timeStartFade;
            _lerpPercentage = _timeSinceStart / _fadeTime;
            float currentValue = Mathf.Lerp(_start, _end, _lerpPercentage);
            trackedDolly.m_PathPosition = currentValue;
            if (_lerpPercentage >= 1) break;
            yield return new WaitForEndOfFrame();
        }
        bSwitching = false;
    }

    void DebugMovement()
    {
        float hori = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        Vector3 dir = (hori * transform.right + vert * transform.forward).normalized;

        transform.position += fDebugMovespeed * dir * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
            SwitchPerspectiveView();
    }

    void DebugAnimator()
    {
        // Reset to 0 to prevent keep playing one animation
        iPlayingAnimationID = 0;
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown((KeyCode)(i + 49)))
            {
                iPlayingAnimationID = i + 1;
            }
        }
        anim.SetInteger("AttackID_i", iPlayingAnimationID);

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            GetDamage(20);
        }
    }

    public void SetLookAt(Transform _lookAt)
    {
        myCamera.m_LookAt = _lookAt;
    }
}
