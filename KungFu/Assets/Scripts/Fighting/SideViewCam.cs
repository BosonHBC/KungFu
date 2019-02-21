using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SideViewCam : MonoBehaviour
{
    [SerializeField] float defaultZValue;
    [SerializeField] float fFactor;
    [SerializeField] float fMaxZValue;
    private CinemachineVirtualCamera vCam;
    private PlayerCenter center;
    private CinemachineTransposer transposer;
    private float fStartDistance;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        float _zValue = defaultZValue + fFactor * (fStartDistance - center.distanceBetween);
        if (_zValue > -fMaxZValue)
            _zValue = -fMaxZValue;
        transposer.m_FollowOffset = new Vector3(transposer.m_FollowOffset.x, transposer.m_FollowOffset.y, _zValue);
    }

    public void SetData(Transform _lookAt)
    {
        center = _lookAt.GetComponent<PlayerCenter>();
        fStartDistance = Vector3.Distance(center.trP1.position, center.trP2.position);
        vCam = GetComponent<CinemachineVirtualCamera>();
        transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
        vCam.m_LookAt = _lookAt;
        vCam.m_Follow = _lookAt;
    }
}
