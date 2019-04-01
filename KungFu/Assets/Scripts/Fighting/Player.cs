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
    private bool bSwitching;
    private PlayerAnimController pAnimCtrl;
    private AttackJointID[] attackJoints;

    

    protected override void Start()
    {
        base.Start();
        iCurrentView = 1;
        bSwitching = false;
        trackedDolly = myCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        pAnimCtrl = GetComponent<PlayerAnimController>();
        attackJoints = transform.GetComponentsInChildren<AttackJointID>();
        System.Array.Sort(attackJoints, delegate (AttackJointID _a1, AttackJointID _a2) {
            return _a1.iJointID.CompareTo(_a2.iJointID);
        });
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        DebugMovement();
        if (Input.GetKeyDown(KeyCode.Alpha9))
            GetDamage(20, UnityEngine.Random.Range(0,2) == 0? true: false);
    }

    public override void GetDamage(float _dmg, bool _fromLeft)
    {
        pAnimCtrl.GetDamage(_fromLeft);
        base.GetDamage(_dmg, _fromLeft);

    }

    public Transform GetJointTransform(int _id)
    {
        return attackJoints[_id].transform;
    }

    public void SwitchPerspectiveView()
    {
       
        if (!bSwitching)
        {
            if (iCurrentView == 0)
            {
                // to third person
               StartCoroutine(IE_SwitchView(trackedDolly.m_PathPosition, 0, 0.2f));
                iCurrentView = 1;
            }
            else if (iCurrentView == 1)
            {
                // to first person
                StartCoroutine(IE_SwitchView(trackedDolly.m_PathPosition, 2f, 0.2f));
                iCurrentView = 0;
            }

        }
    }

    IEnumerator IE_SwitchView(float _start, float _end, float _fadeTime = 0.5f)
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
            float _fovValue = Mathf.Lerp(_start * 10 + 40, _end * 10 + 40, _lerpPercentage);
            trackedDolly.m_PathPosition = currentValue;
            myCamera.m_Lens.FieldOfView = _fovValue;
            if (_lerpPercentage >= 1) break;
            yield return new WaitForEndOfFrame();
        }
        bSwitching = false;
    }

    void DebugMovement()
    {
        //float hori = 0; Input.GetAxisRaw("Horizontal");
        // float vert = 0; Input.GetAxisRaw("Vertical");

        // Vector3 dir = (hori * transform.right + vert * transform.forward).normalized;

        // transform.position += fDebugMovespeed * dir * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Z))
            SwitchPerspectiveView();
        if (Input.GetKeyDown(KeyCode.A))
            GetComponent<BaseAnimController>().DashHorizontally(-1, 1, 1);
        if (Input.GetKeyDown(KeyCode.D))
            GetComponent<BaseAnimController>().DashHorizontally(1, 1, 1);
        if (Input.GetKeyDown(KeyCode.W))
            GetComponent<BaseAnimController>().DashVertically(1, 1, 1);
        if(Input.GetKeyDown(KeyCode.S))
            GetComponent<BaseAnimController>().DashVertically(-1, 1, 1);
        if (Input.GetKeyDown(KeyCode.P))
            GetComponent<BaseAnimController>().StopDash();

    }



    public void SetLookAt(Transform _lookAt)
    {
        myCamera.m_LookAt = _lookAt;
    }


}
