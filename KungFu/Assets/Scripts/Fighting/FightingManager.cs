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

    public enum FightMode
    {
        Wait, Offense, Defense
    }
    public static FightingManager instance;
    private void Awake()
    {
        if (instance == null || instance != this)
        {
            instance = this;
        }
    }
    [Header("Prefab")]
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private GameObject playerCenterPrefab;
    [SerializeField] private Character[] playerPrefabs;
    [Header("Parents")]
    [SerializeField] private Transform playerReleventParent;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private PlayableDirector director;
    [SerializeField] private TimelineAsset timelines;

    [SerializeField] GameObject[] cameraList;

    [Header("Parameter")]
    private SideViewCam sideVCam;
    public int iFightingSceneID;
    [HideInInspector] public Character[] characters = new Character[2];
    [HideInInspector] public Canvas myCanvas;
    public UnityAction onPositioned;
    private float fThresholdOfTime = 0.02f;
    private float fTimeToPlayFightPrepare = 5f;
    public FightMode fightMode = FightMode.Wait;
    bool bPrepared = false;
    private ModeHint hint;
    private int[] randomID = { 0, 1, 2 };
    // Start is called before the first frame update
    void Start()
    {

        randomizeArray(randomID, randomID.Length);
        Debug.Log(randomID[0] + " " + randomID[1]);
        CreateObjects();
    }

    // Update is called once per frame
    void Update()
    {
        Debug_SwitchCamera();

        //Debug.Log(director.time);
        if (!bPrepared && director.time >= fTimeToPlayFightPrepare - fThresholdOfTime && director.time <= fTimeToPlayFightPrepare + fThresholdOfTime)
        {
            bPrepared = true;
            Debug.Log("Start to Prepare!");
            fightMode = FightMode.Defense;
            if (onPositioned != null)
                onPositioned.Invoke();
        }

    }

    void CreateObjects()
    {
        /// Canvas
        GameObject _canvasGo = Instantiate(playerUIPrefab);
        _canvasGo.GetComponent<Canvas>().worldCamera = Camera.main;
        // _canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        _canvasGo.name = "PlayerUI";
        myCanvas = _canvasGo.GetComponent<Canvas>();
        director.SetGenericBinding(timelines.GetOutputTrack(4), _canvasGo.GetComponent<Animator>());
        // Set UIs
        MyGameInstance.instance.SetScoreUI(_canvasGo.transform.Find("Combo").GetChild(0).GetComponent<Text>());
        hint = _canvasGo.transform.GetChild(8).GetComponent<ModeHint>();
        /// Characters 0-> player 1-> enemy

        for (int i = 0; i < 2; i++)
        {
            Character _character = Instantiate(playerPrefabs[i]).GetComponent<Character>();
            characters[i] = _character;
            _character.transform.SetParent(playerReleventParent);
            _character.transform.position = spawnPoints[i].position;

            Animator _anim = _character.GetComponent<Animator>();
            float _prepareID = randomID[i] / 2f;
            Debug.Log("characrer " + i + " play " + _prepareID);
            _anim.SetFloat("rd_PrepareAnim", _prepareID);
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
        onPositioned = new UnityAction(delegate
        {
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].GetComponent<BaseAnimController>().PlayPrepareFight();
            }
        });

        // Set Enemy attack joint and beat generator
        _canvasGo.GetComponentInChildren<RingIndicatorControl>().SetData(characters[1].transform);
        GetComponent<BeatGenerator>().SetData(characters[1].transform, _canvasGo.GetComponentInChildren<HintGenerator>(), _canvasGo.GetComponentInChildren<ResultControl>());

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
                //_shot.VirtualCamera.exposedName = System.Guid.NewGuid().ToString(); 
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

    public void SetFightMode(FightMode _fightMode)
    {
        if (fightMode != _fightMode)
        {
            fightMode = _fightMode;
            // Different mode
            // Switch between attack and defense
            // Changes in UI
            bool bIsPlayerAttack = false;
            if (_fightMode == FightMode.Offense)
                bIsPlayerAttack = true;
            else if (_fightMode == FightMode.Defense)
                bIsPlayerAttack = false;
            characters[1].GetComponent<Animator>().SetBool("PlayerAttacking_b", bIsPlayerAttack);
            hint.SwitchMode(bIsPlayerAttack);
        }
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

    public void PlayerGuard(int releativeAttackID = 0)
    {
        characters[0].GetComponent<PlayerAnimController>().PlayGuardAnimation(releativeAttackID);
    }

    public void FM_Score(HitResult hr, int _attackAnimationID = 0)
    {
        MyGameInstance.instance.Score(hr);
        switch (fightMode)
        {
            case FightMode.Wait:
                break;
            case FightMode.Offense:

                // Play Player Attack animation
                characters[0].GetComponent<PlayerAnimController>().PlayPlayerAttackAnimation(_attackAnimationID);
                // Give Damage to Enemy
                ApplyDamageToCharacter(1, 10f);
                break;
            case FightMode.Defense:
                PlayerGuard(_attackAnimationID);
                break;
        }
    }
    public void FM_Miss(int number)
    {
        MyGameInstance.instance.Miss(number);
        switch (fightMode)
        {
            case FightMode.Wait:
                break;
            case FightMode.Offense:

                break;
            case FightMode.Defense:
                ApplyDamageToCharacter(0, 10f);
                break;
        }
    }
    void randomizeArray(int[] arr, int n)
    {
        // Start from the last element and 
        // swap one by one. We don't need to 
        // run for the first element  
        // that's why i > 0 
        for (int i = n - 1; i > 0; i--)
        {

            // Pick a random index 
            // from 0 to i 
            int j = UnityEngine.Random.Range(0, i + 1);

            // Swap arr[i] with the 
            // element at random index 
            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }

    }
}
