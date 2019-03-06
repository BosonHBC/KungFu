using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.Events;
using Cinemachine;


public class FightingManager : MonoBehaviour
{
    public static FightingManager instance;
    private void Awake()
    {
        if (instance == null || instance != this)
        {
            instance = this;
        }
    }
    [Header("Prefab")]
    [SerializeField] GameObject playerUIPrefab;
    [SerializeField] GameObject playerCenterPrefab;
    [SerializeField] private Character[] playerPrefabs;
    [Header("Parents")]
    [SerializeField] Transform playerReleventParent;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] PlayableDirector director;
    [SerializeField] TimelineAsset timelines;
    
    [SerializeField] GameObject[] cameraList;

    [Header("Parameter")]
    private SideViewCam sideVCam;
    public int iFightingSceneID;
    [HideInInspector] public Character[] characters = new Character[2];
    [HideInInspector] public Canvas myCanvas;
    private int currentCamera;
    public UnityAction onPositioned;
    private float fThresholdOfTime = 0.02f;
    private float fTimeToPlayFightPrepare = 5.30f;

    bool bPrepared = false;
    // Start is called before the first frame update
    void Start()
    {
        currentCamera = 0;
        CreateObjects();
    }

    // Update is called once per frame
    void Update()
    {
        Debug_SwitchCamera();

        //Debug.Log(director.time);
        if (!bPrepared&&director.time >= fTimeToPlayFightPrepare - fThresholdOfTime && director.time <= fTimeToPlayFightPrepare + fThresholdOfTime)
        {
            bPrepared = true;
            Debug.Log("Invoke!");
            if (onPositioned != null)
                onPositioned.Invoke();
        }

    }

    void CreateObjects()
    {
        /// Canvas
        GameObject _canvasGo = Instantiate(playerUIPrefab);
        _canvasGo.GetComponent<Canvas>().worldCamera = Camera.main;
        _canvasGo.name = "PlayerUI";
        myCanvas = _canvasGo.GetComponent<Canvas>();
        director.SetGenericBinding(timelines.GetOutputTrack(4), _canvasGo.GetComponent<Animator>());
        // Set UIs
        MyGameInstance.instance.SetScoreUI(_canvasGo.transform.Find("Combo").GetChild(0).GetComponent<Text>());

        /// Characters 0-> player 1-> enemy
        for (int i = 0; i < 2; i++)
        {
            Character _character = Instantiate(playerPrefabs[i]).GetComponent<Character>();
            characters[i] = _character;
            _character.transform.SetParent(playerReleventParent);
            _character.transform.position = spawnPoints[i].position;
        }
        for (int i = 0; i < characters.Length; i++)
        {
            Image _fillbar = _canvasGo.transform.Find("HpBar").GetChild(i).GetChild((i + 1) % 2).GetChild(1).GetChild(0).GetComponent<Image>();

            Transform _opponentTr = characters[(i + 1) % characters.Length].transform;
            characters[i].SetData(_fillbar,
               _opponentTr);
        }
        /// Player center
        PlayerCenter center = Instantiate(playerCenterPrefab).GetComponent<PlayerCenter>();
        center.name = "PlayerCenter";
        center.transform.SetParent(playerReleventParent);
        center.trP1 = characters[0].transform;
        center.trP2 = characters[1].transform;

        // Player Specialize
        Player thePlayer = (Player)characters[0];
        thePlayer.SetLookAt(center.transform);

        // Set Audio Source
        transform.GetChild(0).GetComponent<AudioPeer>().SetSource(characters[1].GetComponent<AudioSource>());

        // Player animation
        onPositioned = new UnityAction( delegate {
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].GetComponent<BaseAnimController>().PlayPrepareFight();
            }
        });

        // Set Enemy attack joint and beat generator
        _canvasGo.GetComponentInChildren<RingIndicatorControl>().SetData(characters[1].transform);
        MyGameInstance.instance.GetComponent<BeatGenerator>().SetData(characters[1].transform, _canvasGo.GetComponentInChildren<HintGenerator>(), _canvasGo.GetComponentInChildren<ResultControl>());

        /// Set Camera
        IEnumerable<TimelineClip> clips = timelines.GetOutputTrack(1).GetClips();
        IEnumerator iEnumeratorOfClips = clips.GetEnumerator();
        while (iEnumeratorOfClips.MoveNext())
        {
            TimelineClip currClip = iEnumeratorOfClips.Current as TimelineClip;
            // 1. Set the player camera to the timeline
            if (currClip.displayName == "PlayerCamera")
            {
                Cinemachine.Timeline.CinemachineShot _shot = (Cinemachine.Timeline.CinemachineShot)currClip.asset;
                _shot.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
                CinemachineVirtualCameraBase _playerVCam = characters[0].transform.GetChild(characters[0].transform.childCount - 1).GetComponent<CinemachineVirtualCameraBase>();
                cameraList[1] = _playerVCam.gameObject;
                director.SetReferenceValue(_shot.VirtualCamera.exposedName, _playerVCam);
            }
        }

        // 2. Set side virtual camera to
        sideVCam = cameraList[0].GetComponent<SideViewCam>();
        sideVCam.SetData(center.transform);
    }

    public void SwitchCamera(int _switchTo)
    {

        for (int i = 0; i < cameraList.Length; i++)
        {
            cameraList[i].SetActive(false);
        }
        cameraList[_switchTo].SetActive(true);
    }

    void Debug_SwitchCamera()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchCamera(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SwitchCamera(1);
        }
    }

    public void ApplyDamageToCharacter(int _characterID, float _dmgAmount)
    {
        // 0 -> player, 1 -> enemy
        characters[_characterID].GetDamage(_dmgAmount, Random.Range(0, 2) == 0 ? true : false);
    }

    public void PlayerGuard()
    {
        characters[0].GetComponent<PlayerAnimController>().GuardSucceed();
    }

}
